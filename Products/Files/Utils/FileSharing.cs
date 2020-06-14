/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ektai Solutions LTD expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ektai Solutions LTD by email at sales@lexic.xyz
 *
 * The interactive user interfaces in modified source and object code versions of LEXIC must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original LEXIC logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by LEXIC" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.NotifyService;
using ASC.Web.Files.Services.WCFService;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.Utils
{
    public static class FileSharing
    {
        public static bool CanSetAccess(FileEntry entry)
        {
            return
                entry != null
                && (entry.RootFolderType == FolderType.COMMON && Global.IsAdministrator
                    || entry.RootFolderType == FolderType.USER
                    && (Equals(entry.RootFolderId, Global.FolderMy) || Global.GetFilesSecurity().CanEdit(entry))
                    && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor());
        }

        public static List<AceWrapper> GetSharedInfo(FileEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(FilesCommonResource.ErrorMassage_BadRequest);
            if (!CanSetAccess(entry))
            {
                Global.Logger.ErrorFormat("User {0} can't get shared info for {1} {2}", SecurityContext.CurrentAccount.ID, (entry.FileEntryType == FileEntryType.File ? "file" : "folder"), entry.ID);
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
            }

            var linkAccess = FileShare.Restrict;
            var result = new List<AceWrapper>();

            var fileSecurity = Global.GetFilesSecurity();

            var records = fileSecurity
                .GetShares(entry)
                .GroupBy(r => r.Subject)
                .Select(g => g.OrderBy(r => r.Level)
                              .ThenBy(r => r.Level)
                              .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer()).FirstOrDefault());

            foreach (var r in records)
            {
                if (r.Subject == FileConstant.ShareLinkId)
                {
                    linkAccess = r.Share;
                    continue;
                }

                var u = CoreContext.UserManager.GetUsers(r.Subject);
                var isgroup = false;
                var title = u.DisplayUserName(false);

                if (u.ID == Constants.LostUser.ID)
                {
                    var g = CoreContext.UserManager.GetGroupInfo(r.Subject);
                    isgroup = true;
                    title = g.Name;

                    if (g.ID == Constants.GroupAdmin.ID)
                        title = FilesCommonResource.Admin;
                    if (g.ID == Constants.GroupEveryone.ID)
                        title = FilesCommonResource.Everyone;

                    if (g.ID == Constants.LostGroupInfo.ID)
                    {
                        fileSecurity.RemoveSubject(r.Subject);
                        continue;
                    }
                }

                var w = new AceWrapper
                    {
                        SubjectId = r.Subject,
                        SubjectName = title,
                        SubjectGroup = isgroup,
                        Share = r.Share,
                        Owner =
                            entry.RootFolderType == FolderType.USER
                                ? entry.RootFolderCreator == r.Subject
                                : entry.CreateBy == r.Subject,
                        LockedRights = r.Subject == SecurityContext.CurrentAccount.ID
                    };
                result.Add(w);
            }

            if (entry.FileEntryType == FileEntryType.File && result.All(w => w.SubjectId != FileConstant.ShareLinkId)
                && entry.FileEntryType == FileEntryType.File
                && !((File)entry).Encrypted)
            {
                var w = new AceWrapper
                    {
                        SubjectId = FileConstant.ShareLinkId,
                        Link = FileShareLink.GetLink((File)entry),
                        SubjectGroup = true,
                        Share = linkAccess,
                        Owner = false
                    };
                result.Add(w);
            }

            if (!result.Any(w => w.Owner))
            {
                var ownerId = entry.RootFolderType == FolderType.USER ? entry.RootFolderCreator : entry.CreateBy;
                var w = new AceWrapper
                    {
                        SubjectId = ownerId,
                        SubjectName = Global.GetUserName(ownerId),
                        SubjectGroup = false,
                        Share = FileShare.ReadWrite,
                        Owner = true
                    };
                result.Add(w);
            }

            if (result.Any(w => w.SubjectId == SecurityContext.CurrentAccount.ID))
            {
                result.Single(w => w.SubjectId == SecurityContext.CurrentAccount.ID).LockedRights = true;
            }

            if (entry.RootFolderType == FolderType.COMMON)
            {
                if (result.All(w => w.SubjectId != Constants.GroupAdmin.ID))
                {
                    var w = new AceWrapper
                        {
                            SubjectId = Constants.GroupAdmin.ID,
                            SubjectName = FilesCommonResource.Admin,
                            SubjectGroup = true,
                            Share = FileShare.ReadWrite,
                            Owner = false,
                            LockedRights = true,
                        };
                    result.Add(w);
                }
                if (result.All(w => w.SubjectId != Constants.GroupEveryone.ID))
                {
                    var w = new AceWrapper
                        {
                            SubjectId = Constants.GroupEveryone.ID,
                            SubjectName = FilesCommonResource.Everyone,
                            SubjectGroup = true,
                            Share = fileSecurity.DefaultCommonShare,
                            Owner = false,
                            DisableRemove = true
                        };
                    result.Add(w);
                }
            }

            return result;
        }

        public static bool SetAceObject(List<AceWrapper> aceWrappers, FileEntry entry, bool notify, string message)
        {
            if (entry == null) throw new ArgumentNullException(FilesCommonResource.ErrorMassage_BadRequest);
            if (!CanSetAccess(entry)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            var fileSecurity = Global.GetFilesSecurity();

            var entryType = entry.FileEntryType;
            var recipients = new Dictionary<Guid, FileShare>();
            var usersWithoutRight = new List<Guid>();
            var changed = false;

            foreach (var w in aceWrappers.OrderByDescending(ace => ace.SubjectGroup))
            {
                var subjects = fileSecurity.GetUserSubjects(w.SubjectId);

                var ownerId = entry.RootFolderType == FolderType.USER ? entry.RootFolderCreator : entry.CreateBy;
                if (entry.RootFolderType == FolderType.COMMON && subjects.Contains(Constants.GroupAdmin.ID)
                    || ownerId == w.SubjectId)
                    continue;

                var share = w.Share;

                if (w.SubjectId == FileConstant.ShareLinkId)
                {
                    if (w.Share == FileShare.ReadWrite && CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor()) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                    share = w.Share == FileShare.Restrict ? FileShare.None : w.Share;
                }

                fileSecurity.Share(entry.ID, entryType, w.SubjectId, share);
                changed = true;

                if (w.SubjectId == FileConstant.ShareLinkId)
                    continue;

                entry.Access = share;

                var listUsersId = new List<Guid>();

                if (w.SubjectGroup)
                    listUsersId = CoreContext.UserManager.GetUsersByGroup(w.SubjectId).Select(ui => ui.ID).ToList();
                else
                    listUsersId.Add(w.SubjectId);
                listUsersId.Remove(SecurityContext.CurrentAccount.ID);

                if (entryType == FileEntryType.File)
                {
                    listUsersId.ForEach(uid => FileTracker.ChangeRight(entry.ID, uid, true));
                }

                var addRecipient = share == FileShare.Read
                                   || share == FileShare.ReadWrite
                                   || share == FileShare.Review
                                   || share == FileShare.FillForms
                                   || share == FileShare.Comment
                                   || share == FileShare.None && entry.RootFolderType == FolderType.COMMON;
                var removeNew = share == FileShare.None && entry.RootFolderType == FolderType.USER
                                || share == FileShare.Restrict;
                listUsersId.ForEach(id =>
                                        {
                                            recipients.Remove(id);
                                            if (addRecipient)
                                            {
                                                recipients.Add(id, share);
                                            }
                                            else if (removeNew)
                                            {
                                                usersWithoutRight.Add(id);
                                            }
                                        });
            }

            if (entryType == FileEntryType.File)
            {
                DocumentServiceHelper.CheckUsersForDrop((File) entry);
            }

            if (recipients.Any())
            {
                if (entryType == FileEntryType.File
                    || ((Folder)entry).TotalSubFolders + ((Folder)entry).TotalFiles > 0
                    || entry.ProviderEntry)
                {
                    FileMarker.MarkAsNew(entry, recipients.Keys.ToList());
                }

                if (entry.RootFolderType == FolderType.USER
                    && notify)
                {
                    NotifyClient.SendShareNotice(entry, recipients, message);
                }
            }

            usersWithoutRight.ForEach(userId => FileMarker.RemoveMarkAsNew(entry, userId));

            return changed;
        }

        public static void RemoveAce(List<FileEntry> entries)
        {
            var fileSecurity = Global.GetFilesSecurity();

            entries.ForEach(
                entry =>
                    {
                        if (entry.RootFolderType != FolderType.USER || Equals(entry.RootFolderId, Global.FolderMy))
                            return;

                        var entryType = entry.FileEntryType;
                        fileSecurity.Share(entry.ID, entryType, SecurityContext.CurrentAccount.ID, fileSecurity.DefaultMyShare);

                        if (entryType == FileEntryType.File)
                        {
                            DocumentServiceHelper.CheckUsersForDrop((File)entry);
                        }

                        FileMarker.RemoveMarkAsNew(entry);
                    });
        }
    }
}