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
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Files.Api;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core.Security
{
    public class FileSecurity : IFileSecurity
    {
        private readonly IDaoFactory daoFactory;

        public static bool IsAdministrator(Guid userId)
        {
            return CoreContext.UserManager.IsUserInGroup(userId, Constants.GroupAdmin.ID) ||
                   WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, userId);
        }

        public FileShare DefaultMyShare
        {
            get { return FileShare.Restrict; }
        }

        public FileShare DefaultProjectsShare
        {
            get { return FileShare.ReadWrite; }
        }

        public FileShare DefaultCommonShare
        {
            get { return FileShare.Read; }
        }

        public FileSecurity(IDaoFactory daoFactory)
        {
            this.daoFactory = daoFactory;
        }

        public List<Tuple<FileEntry, bool>> CanRead(IEnumerable<FileEntry> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Read);
        }

        public bool CanRead(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Read);
        }

        public bool CanComment(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Comment);
        }

        public bool CanFillForms(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.FillForms);
        }

        public bool CanReview(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Review);
        }

        public bool CanCreate(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Create);
        }

        public bool CanEdit(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Edit);
        }

        public bool CanDelete(FileEntry entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Delete);
        }

        public bool CanRead(FileEntry entry)
        {
            return CanRead(entry, SecurityContext.CurrentAccount.ID);
        }

        public bool CanComment(FileEntry entry)
        {
            return CanComment(entry, SecurityContext.CurrentAccount.ID);
        }

        public bool CanFillForms(FileEntry entry)
        {
            return CanFillForms(entry, SecurityContext.CurrentAccount.ID);
        }

        public bool CanReview(FileEntry entry)
        {
            return CanReview(entry, SecurityContext.CurrentAccount.ID);
        }

        public bool CanCreate(FileEntry entry)
        {
            return CanCreate(entry, SecurityContext.CurrentAccount.ID);
        }

        public bool CanEdit(FileEntry entry)
        {
            return CanEdit(entry, SecurityContext.CurrentAccount.ID);
        }

        public bool CanDelete(FileEntry entry)
        {
            return CanDelete(entry, SecurityContext.CurrentAccount.ID);
        }

        public IEnumerable<Guid> WhoCanRead(FileEntry entry)
        {
            return WhoCan(entry, FilesSecurityActions.Read);
        }

        private IEnumerable<Guid> WhoCan(FileEntry entry, FilesSecurityActions action)
        {
            var shares = GetShares(entry);

            FileShareRecord defaultShareRecord;

            switch (entry.RootFolderType)
            {
                case FolderType.COMMON:
                    defaultShareRecord = new FileShareRecord
                        {
                            Level = int.MaxValue,
                            EntryId = entry.ID,
                            EntryType = entry.FileEntryType,
                            Share = DefaultCommonShare,
                            Subject = Constants.GroupEveryone.ID,
                            Tenant = TenantProvider.CurrentTenantID,
                            Owner = SecurityContext.CurrentAccount.ID
                        };

                    if (!shares.Any())
                    {
                        if ((defaultShareRecord.Share == FileShare.Read && action == FilesSecurityActions.Read) ||
                            (defaultShareRecord.Share == FileShare.ReadWrite))
                            return CoreContext.UserManager.GetUsersByGroup(defaultShareRecord.Subject)
                                              .Where(x => x.Status == EmployeeStatus.Active).Select(y => y.ID).Distinct();

                        return Enumerable.Empty<Guid>();
                    }

                    break;

                case FolderType.USER:
                    defaultShareRecord = new FileShareRecord
                        {
                            Level = int.MaxValue,
                            EntryId = entry.ID,
                            EntryType = entry.FileEntryType,
                            Share = DefaultMyShare,
                            Subject = entry.RootFolderCreator,
                            Tenant = TenantProvider.CurrentTenantID,
                            Owner = entry.RootFolderCreator
                        };

                    if (!shares.Any())
                        return new List<Guid>
                            {
                                entry.RootFolderCreator
                            };

                    break;

                case FolderType.BUNCH:
                    if (action == FilesSecurityActions.Read)
                    {
                        using (var folderDao = daoFactory.GetFolderDao())
                        {
                            var root = folderDao.GetFolder(entry.RootFolderId);
                            if (root != null)
                            {
                                var path = folderDao.GetBunchObjectID(root.ID);

                                var adapter = FilesIntegration.GetFileSecurity(path);

                                if (adapter != null)
                                {
                                    return adapter.WhoCanRead(entry);
                                }
                            }
                        }
                    }

                    // TODO: For Projects and other
                    defaultShareRecord = null;
                    break;

                default:
                    defaultShareRecord = null;
                    break;
            }

            if (defaultShareRecord != null)
                shares = shares.Concat(new[] {defaultShareRecord});

            return shares.SelectMany(x =>
                                         {
                                             var groupInfo = CoreContext.UserManager.GetGroupInfo(x.Subject);

                                             if (groupInfo.ID != Constants.LostGroupInfo.ID)
                                                 return
                                                     CoreContext.UserManager.GetUsersByGroup(groupInfo.ID)
                                                                .Where(p => p.Status == EmployeeStatus.Active)
                                                                .Select(y => y.ID);

                                             return new[] {x.Subject};
                                         })
                         .Distinct()
                         .Where(x => Can(entry, x, action))
                         .ToList();
        }

        public IEnumerable<FileEntry> FilterRead(IEnumerable<FileEntry> entries)
        {
            return Filter(entries, FilesSecurityActions.Read, SecurityContext.CurrentAccount.ID);
        }

        public IEnumerable<File> FilterRead(IEnumerable<File> entries)
        {
            return Filter(entries.Cast<FileEntry>(), FilesSecurityActions.Read, SecurityContext.CurrentAccount.ID).Cast<File>();
        }

        public IEnumerable<Folder> FilterRead(IEnumerable<Folder> entries)
        {
            return Filter(entries.Cast<FileEntry>(), FilesSecurityActions.Read, SecurityContext.CurrentAccount.ID).Cast<Folder>();
        }

        public IEnumerable<File> FilterEdit(IEnumerable<File> entries)
        {
            return Filter(entries.Cast<FileEntry>(), FilesSecurityActions.Edit, SecurityContext.CurrentAccount.ID).Cast<File>();
        }

        public IEnumerable<Folder> FilterEdit(IEnumerable<Folder> entries)
        {
            return Filter(entries.Cast<FileEntry>(), FilesSecurityActions.Edit, SecurityContext.CurrentAccount.ID).Cast<Folder>();
        }

        private bool Can(FileEntry entry, Guid userId, FilesSecurityActions action)
        {
            return Filter(new[] {entry}, action, userId).Any();
        }

        private List<Tuple<FileEntry, bool>> Can(IEnumerable<FileEntry> entry, Guid userId, FilesSecurityActions action)
        {
            var filtres = Filter(entry, action, userId);
            return entry.Select(r=> new Tuple<FileEntry, bool>(r, filtres.Any(a=> a.ID.Equals(r.ID)))).ToList();
        }

        private IEnumerable<FileEntry> Filter(IEnumerable<FileEntry> entries, FilesSecurityActions action, Guid userId)
        {
            if (entries == null || !entries.Any()) return Enumerable.Empty<FileEntry>();

            var user = CoreContext.UserManager.GetUsers(userId);
            var isOutsider = user.IsOutsider();

            if (isOutsider && action != FilesSecurityActions.Read) return Enumerable.Empty<FileEntry>();

            entries = entries.Where(f => f != null).ToList();
            var result = new List<FileEntry>(entries.Count());

            // save entries order
            var order = entries.Select((f, i) => new {Id = f.UniqID, Pos = i}).ToDictionary(e => e.Id, e => e.Pos);

            // common or my files
            Func<FileEntry, bool> filter =
                f => f.RootFolderType == FolderType.COMMON ||
                     f.RootFolderType == FolderType.USER ||
                     f.RootFolderType == FolderType.SHARE ||
                     f.RootFolderType == FolderType.Projects;

            var isVisitor = user.IsVisitor();

            if (entries.Any(filter))
            {
                var subjects = GetUserSubjects(userId);
                List<FileShareRecord> shares = null;
                foreach (var e in entries.Where(filter))
                {
                    if (!CoreContext.Authentication.GetAccountByID(userId).IsAuthenticated && userId != FileConstant.ShareLinkId)
                    {
                        continue;
                    }

                    if (isOutsider && (e.RootFolderType == FolderType.USER
                                       || e.RootFolderType == FolderType.SHARE
                                       || e.RootFolderType == FolderType.TRASH))
                    {
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((Folder) e).FolderType == FolderType.Projects)
                    {
                        // Root Projects folder read-only
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((Folder) e).FolderType == FolderType.SHARE)
                    {
                        // Root Share folder read-only
                        continue;
                    }

                    if (isVisitor && e.ProviderEntry)
                    {
                        continue;
                    }

                    if (e.RootFolderType == FolderType.USER && e.RootFolderCreator == userId && !isVisitor)
                    {
                        // user has all right in his folder
                        result.Add(e);
                        continue;
                    }

                    if (DefaultCommonShare == FileShare.Read && action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((Folder) e).FolderType == FolderType.COMMON)
                    {
                        // all can read Common folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((Folder) e).FolderType == FolderType.SHARE)
                    {
                        // all can read Share folder
                        result.Add(e);
                        continue;
                    }

                    if (e.RootFolderType == FolderType.COMMON && IsAdministrator(userId))
                    {
                        // administrator in Common has all right
                        result.Add(e);
                        continue;
                    }

                    if (shares == null)
                    {
                        shares = GetShares(entries).Join(subjects, r => r.Subject, s => s, (r, s) => r).ToList();
                        // shares ordered by level
                    }

                    FileShareRecord ace;
                    if (e.FileEntryType == FileEntryType.File)
                    {
                        ace = shares
                            .OrderBy(r => r, new SubjectComparer(subjects))
                            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                            .FirstOrDefault(r => Equals(r.EntryId, e.ID) && r.EntryType == FileEntryType.File);
                        if (ace == null)
                        {
                            // share on parent folders
                            ace = shares.Where(r => Equals(r.EntryId, ((File) e).FolderID) && r.EntryType == FileEntryType.Folder)
                                        .OrderBy(r => r, new SubjectComparer(subjects))
                                        .ThenBy(r => r.Level)
                                        .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                                        .FirstOrDefault();
                        }
                    }
                    else
                    {
                        ace = shares.Where(r => Equals(r.EntryId, e.ID) && r.EntryType == FileEntryType.Folder)
                                    .OrderBy(r => r, new SubjectComparer(subjects))
                                    .ThenBy(r => r.Level)
                                    .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                                    .FirstOrDefault();
                    }
                    var defaultShare = e.RootFolderType == FolderType.USER ? DefaultMyShare : DefaultCommonShare;
                    e.Access = ace != null ? ace.Share : defaultShare;

                    if (action == FilesSecurityActions.Read && e.Access != FileShare.Restrict) result.Add(e);
                    else if (action == FilesSecurityActions.Comment && (e.Access == FileShare.Comment || e.Access == FileShare.Review || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.FillForms && (e.Access == FileShare.FillForms || e.Access == FileShare.Review || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.Review && (e.Access == FileShare.Review || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.Edit && e.Access == FileShare.ReadWrite) result.Add(e);
                    else if (action == FilesSecurityActions.Create && e.Access == FileShare.ReadWrite) result.Add(e);
                    else if (e.Access != FileShare.Restrict && e.CreateBy == userId && (e.FileEntryType == FileEntryType.File || ((Folder)e).FolderType != FolderType.COMMON)) result.Add(e);

                    if (e.CreateBy == userId) e.Access = FileShare.None; //HACK: for client
                }
            }

            // files in bunch
            filter = f => f.RootFolderType == FolderType.BUNCH;
            if (entries.Any(filter))
            {
                using (var folderDao = daoFactory.GetFolderDao())
                {
                    var filteredEntries = entries.Where(filter).ToList();
                    var roots = filteredEntries
                            .Select(r => r.RootFolderId)
                            .ToArray();

                    var rootsFolders = folderDao.GetFolders(roots);
                    var bunches = folderDao.GetBunchObjectIDs(rootsFolders.Select(r => r.ID).ToList());
                    var findedAdapters = FilesIntegration.GetFileSecurity(bunches);

                    foreach (var e in filteredEntries)
                    {
                        var adapter = findedAdapters[e.RootFolderId.ToString()];

                        if (adapter == null) continue;

                        if (adapter.CanRead(e, userId) &&
                            adapter.CanCreate(e, userId) &&
                            adapter.CanEdit(e, userId) &&
                            adapter.CanDelete(e, userId))
                        {
                            e.Access = FileShare.None;
                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.Comment && adapter.CanComment(e, userId))
                        {
                            e.Access = FileShare.Comment;
                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.FillForms && adapter.CanFillForms(e, userId))
                        {
                            e.Access = FileShare.FillForms;
                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.Review && adapter.CanReview(e, userId))
                        {
                            e.Access = FileShare.Review;
                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.Create && adapter.CanCreate(e, userId))
                        {
                            e.Access = FileShare.ReadWrite;
                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.Delete && adapter.CanDelete(e, userId))
                        {
                            e.Access = FileShare.ReadWrite;
                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.Read && adapter.CanRead(e, userId))
                        {
                            if (adapter.CanCreate(e, userId) ||
                                adapter.CanDelete(e, userId) ||
                                adapter.CanEdit(e, userId))
                                e.Access = FileShare.ReadWrite;
                            else
                                e.Access = FileShare.Read;

                            result.Add(e);
                        }
                        else if (action == FilesSecurityActions.Edit && adapter.CanEdit(e, userId))
                        {
                            e.Access = FileShare.ReadWrite;

                            result.Add(e);
                        }
                    }
                }
            }

            // files in trash
            filter = f => f.RootFolderType == FolderType.TRASH;
            if ((action == FilesSecurityActions.Read || action == FilesSecurityActions.Delete) && entries.Any(filter))
            {
                using (var folderDao = daoFactory.GetFolderDao())
                {
                    var mytrashId = folderDao.GetFolderIDTrash(false, userId);
                    if (!Equals(mytrashId, 0))
                    {
                        result.AddRange(entries.Where(filter).Where(e => Equals(e.RootFolderId, mytrashId)));
                    }
                }
            }

            if (IsAdministrator(userId))
            {
                // administrator can work with crashed entries (crash in files_folder_tree)
                filter = f => f.RootFolderType == FolderType.DEFAULT;
                result.AddRange(entries.Where(filter));
            }

            // restore entries order
            result.Sort((x, y) => order[x.UniqID].CompareTo(order[y.UniqID]));
            return result;
        }

        public void Share(object entryId, FileEntryType entryType, Guid @for, FileShare share)
        {
            using (var securityDao = daoFactory.GetSecurityDao())
            {
                var r = new FileShareRecord
                    {
                        Tenant = TenantProvider.CurrentTenantID,
                        EntryId = entryId,
                        EntryType = entryType,
                        Subject = @for,
                        Owner = SecurityContext.CurrentAccount.ID,
                        Share = share,
                    };
                securityDao.SetShare(r);
            }
        }

        public IEnumerable<FileShareRecord> GetShares(IEnumerable<FileEntry> entries)
        {
            using (var securityDao = daoFactory.GetSecurityDao())
            {
                return securityDao.GetShares(entries);
            }
        }

        public IEnumerable<FileShareRecord> GetShares(FileEntry entry)
        {
            using (var securityDao = daoFactory.GetSecurityDao())
            {
                return securityDao.GetShares(entry);
            }
        }

        public List<FileEntry> GetSharesForMe(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            using (var folderDao = daoFactory.GetFolderDao())
            using (var fileDao = daoFactory.GetFileDao())
            using (var securityDao = daoFactory.GetSecurityDao())
            {
                var subjects = GetUserSubjects(SecurityContext.CurrentAccount.ID);

                var records = securityDao.GetShares(subjects);

                var fileIds = new Dictionary<object, FileShare>();
                var folderIds = new Dictionary<object, FileShare>();

                var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
                {
                    firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
                        .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                        .First()
                });

                foreach (var r in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
                {
                    if (r.firstRecord.EntryType == FileEntryType.Folder)
                    {
                        if (!folderIds.ContainsKey(r.firstRecord.EntryId))
                            folderIds.Add(r.firstRecord.EntryId, r.firstRecord.Share);
                    }
                    else
                    {
                        if (!fileIds.ContainsKey(r.firstRecord.EntryId))
                            fileIds.Add(r.firstRecord.EntryId, r.firstRecord.Share);
                    }
                }

                var entries = new List<FileEntry>();

                if (filterType != FilterType.FoldersOnly)
                {
                    var files = fileDao.GetFilesForShare(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);

                    files.ForEach(x =>
                        {
                            if (fileIds.ContainsKey(x.ID))
                            {
                                x.Access = fileIds[x.ID];
                                x.FolderIdDisplay = Global.FolderShare;
                            }
                        });

                    entries.AddRange(files);
                }

                if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
                {
                    var folders = folderDao.GetFolders(folderIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

                    if (withSubfolders)
                    {
                        folders = FilterRead(folders).ToList();
                    }
                    folders.ForEach(x =>
                        {
                            if (folderIds.ContainsKey(x.ID))
                            {
                                x.Access = folderIds[x.ID];
                                x.FolderIdDisplay = Global.FolderShare;
                            }
                        });

                    entries.AddRange(folders.Cast<FileEntry>());
                }

                if (filterType != FilterType.FoldersOnly && withSubfolders)
                {
                    var filesInSharedFolders = fileDao.GetFiles(folderIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);
                    filesInSharedFolders = FilterRead(filesInSharedFolders).ToList();
                    entries.AddRange(filesInSharedFolders);
                    entries = entries.Distinct().ToList();
                }

                entries = entries.Where(f =>
                                        f.RootFolderType == FolderType.USER // show users files
                                        && f.RootFolderCreator != SecurityContext.CurrentAccount.ID // don't show my files
                    ).ToList();

                if (CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor())
                {
                    entries = entries.Where(r => !r.ProviderEntry).ToList();
                }

                var failedEntries = entries.Where(x => !String.IsNullOrEmpty(x.Error));
                var failedRecords = new List<FileShareRecord>();

                foreach (var failedEntry in failedEntries)
                {
                    var entryType = failedEntry.FileEntryType;

                    var failedRecord = records.First(x => x.EntryId.Equals(failedEntry.ID) && x.EntryType == entryType);

                    failedRecord.Share = FileShare.None;

                    failedRecords.Add(failedRecord);
                }

                if (failedRecords.Any())
                {
                    securityDao.DeleteShareRecords(failedRecords);
                }

                return entries.Where(x => String.IsNullOrEmpty(x.Error)).ToList();
            }
        }

        public void RemoveSubject(Guid subject)
        {
            using (var securityDao = daoFactory.GetSecurityDao())
            {
                securityDao.RemoveSubject(subject);
            }
        }

        public List<Guid> GetUserSubjects(Guid userId)
        {
            // priority order
            // User, Departments, admin, everyone

            var result = new List<Guid> { userId };
            if (userId == FileConstant.ShareLinkId)
                return result;

            result.AddRange(CoreContext.UserManager.GetUserGroups(userId).Select(g => g.ID));
            if (IsAdministrator(userId)) result.Add(Constants.GroupAdmin.ID);
            result.Add(Constants.GroupEveryone.ID);

            return result;
        }

        private class SubjectComparer : IComparer<FileShareRecord>
        {
            private readonly List<Guid> _subjects;

            public SubjectComparer(List<Guid> subjects)
            {
                _subjects = subjects;
            }

            public int Compare(FileShareRecord x, FileShareRecord y)
            {
                if (x.Subject == y.Subject)
                {
                    return 0;
                }

                var index1 = _subjects.IndexOf(x.Subject);
                var index2 = _subjects.IndexOf(y.Subject);
                if (index1 == 0 || index2 == 0 // UserId
                    || Constants.BuildinGroups.Any(g => g.ID == x.Subject) || Constants.BuildinGroups.Any(g => g.ID == y.Subject)) // System Groups
                {
                    return index1.CompareTo(index2);
                }

                // Departments are equal.
                return 0;
            }
        }

        private enum FilesSecurityActions
        {
            Read,
            Comment,
            FillForms,
            Review,
            Create,
            Edit,
            Delete,
        }
    }
}