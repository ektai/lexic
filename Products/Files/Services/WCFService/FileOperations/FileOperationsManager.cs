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
using System.Diagnostics;
using System.Linq;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Web.Files.Resources;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    class FileOperationsManager
    {
        private DistributedTaskQueue tasks = new DistributedTaskQueue("fileOperations", 10);
        

        public ItemList<FileOperationResult> GetOperationResults()
        {
            var operations = tasks.GetTasks();
            var processlist = Process.GetProcesses();

            foreach (var o in operations.Where(o => string.IsNullOrEmpty(o.InstanseId)
                                                    || processlist.All(p => p.Id != int.Parse(o.InstanseId))))
            {
                o.SetProperty(FileOperation.PROGRESS, 100);
                tasks.RemoveTask(o.Id);
            }

            operations = operations.Where(t => t.GetProperty<Guid>(FileOperation.OWNER) == SecurityContext.CurrentAccount.ID);
            foreach (var o in operations.Where(o => DistributedTaskStatus.Running < o.Status))
            {
                o.SetProperty(FileOperation.PROGRESS, 100);
                tasks.RemoveTask(o.Id);
            }

            var results = operations
                .Where(o => o.GetProperty<bool>(FileOperation.HOLD) || o.GetProperty<int>(FileOperation.PROGRESS) != 100)
                .Select(o => new FileOperationResult
                    {
                        Id = o.Id,
                        OperationType = o.GetProperty<FileOperationType>(FileOperation.OPERATION_TYPE),
                        Source = o.GetProperty<string>(FileOperation.SOURCE),
                        Progress = o.GetProperty<int>(FileOperation.PROGRESS),
                        Processed = o.GetProperty<int>(FileOperation.PROCESSED).ToString(),
                        Result = o.GetProperty<string>(FileOperation.RESULT),
                        Error = o.GetProperty<string>(FileOperation.ERROR),
                        Finished = o.GetProperty<bool>(FileOperation.FINISHED),
                    });

            return new ItemList<FileOperationResult>(results);
        }

        public ItemList<FileOperationResult> CancelOperations()
        {
            var operations = tasks.GetTasks()
                .Where(t => t.GetProperty<Guid>(FileOperation.OWNER) == SecurityContext.CurrentAccount.ID);

            foreach (var o in operations)
            {
                tasks.CancelTask(o.Id);
            }

            return GetOperationResults();
        }


        public ItemList<FileOperationResult> MarkAsRead(List<object> folderIds, List<object> fileIds)
        {
            var op = new FileMarkAsReadOperation(folderIds, fileIds);
            return QueueTask(op);
        }

        public ItemList<FileOperationResult> Download(Dictionary<object, string> folders, Dictionary<object, string> files, Dictionary<string, string> headers)
        {
            var operations = tasks.GetTasks()
                .Where(t => t.GetProperty<Guid>(FileOperation.OWNER) == SecurityContext.CurrentAccount.ID)
                .Where(t => t.GetProperty<FileOperationType>(FileOperation.OPERATION_TYPE) == FileOperationType.Download);

            if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
            {
                throw new InvalidOperationException(FilesCommonResource.ErrorMassage_ManyDownloads);
            }

            var op = new FileDownloadOperation(folders, files, headers);
            return QueueTask(op);
        }

        public ItemList<FileOperationResult> MoveOrCopy(List<object> folders, List<object> files, string destFolderId, bool copy, FileConflictResolveType resolveType, bool holdResult, Dictionary<string, string> headers)
        {
            var op = new FileMoveCopyOperation(folders, files, destFolderId, copy, resolveType, holdResult, headers);
            return QueueTask(op);
        }

        public ItemList<FileOperationResult> Delete(List<object> folders, List<object> files, bool ignoreException, bool holdResult, bool immediately, Dictionary<string, string> headers)
        {
            var op = new FileDeleteOperation(folders, files, ignoreException, holdResult, immediately, headers);
            return QueueTask(op);
        }


        private ItemList<FileOperationResult> QueueTask(FileOperation op)
        {
            tasks.QueueTask(op.RunJob, op.GetDistributedTask());
            return GetOperationResults();
        }
    }
}