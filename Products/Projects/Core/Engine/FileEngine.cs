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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using ASC.Files.Core;
using ASC.Web.Files.Api;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService;
using File = ASC.Files.Core.File;
using FileShare = ASC.Files.Core.Security.FileShare;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Projects.Engine
{
    public class FileEngine
    {
        private const string Module = "projects";
        private const string Bunch = "project";
        private readonly SecurityAdapterProvider securityAdapterProvider = new SecurityAdapterProvider();

        public object GetRoot(int projectId)
        {
            return FilesIntegration.RegisterBunch(Module, Bunch, projectId.ToString(CultureInfo.InvariantCulture));
        }

        public IEnumerable<object> GetRoots(IEnumerable<int> projectIds)
        {
            return FilesIntegration.RegisterBunchFolders(Module, Bunch, projectIds.Select(id => id.ToString(CultureInfo.InvariantCulture)));
        }

        public File GetFile(object id, int version = 1)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                var file = 0 < version ? dao.GetFile(id, version) : dao.GetFile(id);
                return file;
            }
        }

        public IEnumerable<File> GetFiles(object[] id)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                return dao.GetFiles(id);
            }
        }

        public File SaveFile(File file, Stream stream)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                return dao.SaveFile(file, stream);
            }
        }

        public void RemoveRoot(int projectId)
        {
            var folderId = GetRoot(projectId);

            //requet long operation
            try
            {
                Global.FileStorageService.DeleteItems("delete", new ItemList<string> {"folder_" + folderId}, true);
            }
            catch (Exception)
            {
                
            }
        }

        public void MoveToTrash(object id)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                dao.MoveFile(id, Global.FolderTrash);
            }
        }

        public static void RegisterFileSecurityProvider()
        {
            FilesIntegration.RegisterFileSecurityProvider(Module, Bunch, new SecurityAdapterProvider());
        }

        internal static List<Tuple<string, string>> GetFileListInfoHashtable(IEnumerable<File> uploadedFiles)
        {
            if (uploadedFiles == null) return new List<Tuple<string, string>>();

            var fileListInfoHashtable = new List<Tuple<string, string>>();

            foreach (var file in uploadedFiles)
            {
                var fileInfo = string.Format("{0} ({1})", file.Title, Path.GetExtension(file.Title).ToUpper());
                fileListInfoHashtable.Add(new Tuple<string, string>(fileInfo, file.DownloadUrl));
            }

            return fileListInfoHashtable;
        }

        internal FileShare GetFileShare(FileEntry file, int projectId)
        {
            var fileSecurity = securityAdapterProvider.GetFileSecurity(projectId);
            var currentUserId = SecurityContext.CurrentAccount.ID;
            if (!fileSecurity.CanRead(file, currentUserId)) return FileShare.Restrict;
            if (!fileSecurity.CanCreate(file, currentUserId) || !fileSecurity.CanEdit(file, currentUserId)) return FileShare.Read;
            if (!fileSecurity.CanDelete(file, currentUserId)) return FileShare.ReadWrite;

            return FileShare.None;
        }
    }
}