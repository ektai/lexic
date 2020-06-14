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
using System.Globalization;
using System.Linq;
using System.Web;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Files.Core;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Files;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Core;
using Autofac;
using IDaoFactory = ASC.Projects.Core.DataInterfaces.IDaoFactory;

namespace ASC.Projects.Engine
{
    [DebuggerDisplay("SearchItem: EntityType = {EntityType}, ID = {ID}, Title = {Title}")]
    public class SearchItem
    {
        public EntityType EntityType { get; private set; }
        public string ItemPath { get; private set; }
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreateOn { get; private set; }
        public SearchItem Container { get; set; }

        public SearchItem(EntityType entityType, string id, string title, DateTime createOn, SearchItem container = null, string desc = "", string itemPath = "")
        {
            Container = container;
            EntityType = entityType;
            ID = id;
            Title = title;
            Description = desc;
            CreateOn = createOn;
            
            if (!string.IsNullOrEmpty(itemPath))
            {
                ItemPath = ItemPathToAbsolute(itemPath);
            }
            else if(container != null)
            {
                ItemPath = container.ItemPath;
            }
        }

        public SearchItem(ProjectEntity entity)
            : this(entity.EntityType, entity.ID.ToString(CultureInfo.InvariantCulture), entity.Title, entity.CreateOn, new SearchItem(entity.Project), entity.Description, entity.ItemPath)
        {
        }

        public SearchItem(Project entity)
            : this(entity.EntityType, entity.ID.ToString(CultureInfo.InvariantCulture), entity.Title, entity.CreateOn, desc: entity.Description, itemPath: entity.ItemPath)
        {
        }

        public Dictionary<string, object> GetAdditional()
        {
            var result = new Dictionary<string, object>
                             {
                                 { "Type", EntityType },
                                 { "Hint", LocalizedEnumConverter.ConvertToString(EntityType) }
                             };

            if (Container != null)
            {
                result.Add("ContainerValue", Container.Title);
                result.Add("ContainerTitle", LocalizedEnumConverter.ConvertToString(Container.EntityType));
                result.Add("ContainerPath", Container.ItemPath);
            }

            return result;
        }

        private string ItemPathToAbsolute(string itemPath)
        {
            var projectID = ID;
            var container = Container;

            while (true)
            {
                if(container == null) break;

                projectID = container.ID;

                container = container.Container;
            }

            return string.Format(itemPath, VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath), projectID, ID);
        }
    }

    public class SearchEngine
    {
        public IDaoFactory DaoFactory { get; set; }
        public ProjectSecurity ProjectSecurity { get; set; }

        public EngineFactory EngineFactory { get; set; }
        public CommentEngine CommentEngine { get { return EngineFactory.CommentEngine; } }
        public TaskEngine TaskEngine { get { return EngineFactory.TaskEngine; } }

        private readonly List<SearchItem> searchItems;

        public SearchEngine()
        {
            searchItems = new List<SearchItem>();
        }

        public IEnumerable<SearchItem> Search(string searchText, int projectId = 0)
        {
            var queryResult = DaoFactory.SearchDao.Search(searchText, projectId);
            using (var scope = DIHelper.Resolve())
            {
                var projectSecurity = scope.Resolve<ProjectSecurity>();

                foreach (var r in queryResult)
                {
                    switch (r.EntityType)
                    {
                        case EntityType.Project:
                            var project = (Project) r;
                            if (projectSecurity.CanRead(project))
                            {
                                searchItems.Add(new SearchItem(project));
                            }
                            continue;
                        case EntityType.Milestone:
                            var milestone = (Milestone) r;
                            if (projectSecurity.CanRead(milestone))
                            {
                                searchItems.Add(new SearchItem(milestone));
                            }
                            continue;
                        case EntityType.Message:
                            var message = (Message) r;
                            if (projectSecurity.CanRead(message))
                            {
                                searchItems.Add(new SearchItem(message));
                            }
                            continue;
                        case EntityType.Task:
                            var task = (Task) r;
                            if (projectSecurity.CanRead(task))
                            {
                                searchItems.Add(new SearchItem(task));
                            }
                            continue;
                        case EntityType.Comment:
                            var comment = (Comment) r;
                            var entity = CommentEngine.GetEntityByTargetUniqId(comment);
                            if (entity == null) continue;

                            searchItems.Add(new SearchItem(comment.EntityType,
                                comment.ID.ToString(CultureInfo.InvariantCulture), HtmlUtil.GetText(comment.Content),
                                comment.CreateOn, new SearchItem(entity)));
                            continue;
                        case EntityType.SubTask:
                            var subtask = (Subtask) r;
                            var parentTask = TaskEngine.GetByID(subtask.Task);
                            if (parentTask == null) continue;

                            searchItems.Add(new SearchItem(subtask.EntityType,
                                subtask.ID.ToString(CultureInfo.InvariantCulture), subtask.Title, subtask.CreateOn,
                                new SearchItem(parentTask)));
                            continue;
                    }
                }
            }

            try
            {
                // search in files
                var fileEntries = new List<FileEntry>();
                using (var folderDao = FilesIntegration.GetFolderDao())
                using (var fileDao = FilesIntegration.GetFileDao())
                {
                    fileEntries.AddRange(folderDao.Search(searchText, true));
                    fileEntries.AddRange(fileDao.Search(searchText, true));

                    var projectIds = projectId != 0
                                         ? new List<int> {projectId}
                                         : fileEntries.GroupBy(f => f.RootFolderId)
                                               .Select(g => folderDao.GetFolder(g.Key))
                                               .Select(f => f != null ? folderDao.GetBunchObjectID(f.RootFolderId).Split('/').Last() : null)
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .Select(int.Parse);

                    var rootProject = projectIds.ToDictionary(id => FilesIntegration.RegisterBunch("projects", "project", id.ToString(CultureInfo.InvariantCulture)));
                    fileEntries.RemoveAll(f => !rootProject.ContainsKey(f.RootFolderId));

                    var security = FilesIntegration.GetFileSecurity();
                    fileEntries.RemoveAll(f => !security.CanRead(f));

                    foreach (var f in fileEntries)
                    {
                        var id = rootProject[f.RootFolderId];
                        var project = DaoFactory.ProjectDao.GetById(id);

                        if (ProjectSecurity.CanReadFiles(project))
                        {
                            var itemId = f.FileEntryType == FileEntryType.File
                                             ? FilesLinkUtility.GetFileWebPreviewUrl(f.Title, f.ID)
                                             : Web.Files.Classes.PathProvider.GetFolderUrl((Folder) f, project.ID);
                            searchItems.Add(new SearchItem(EntityType.File, itemId, f.Title, f.CreateOn, new SearchItem(project), itemPath: "{2}"));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                LogManager.GetLogger("ASC").Error(err);
            }
            return searchItems;
        }
    }
}