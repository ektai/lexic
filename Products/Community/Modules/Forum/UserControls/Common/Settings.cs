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
using System.Web;
using System.Collections.Generic;

namespace ASC.Web.UserControls.Forum.Common
{
    public class Settings
    {
        public ForumManager ForumManager { get; private set; }

        public LinkProvider LinkProvider { get; private set; }

        public Guid ID { get; private set; }

        public Guid ModuleID { get; private set; }

        public Guid ImageItemID { get; private set; }

        public int TopicCountOnPage { get; private set; }

        public int PostCountOnPage { get; private set; }

        public string UserControlsVirtualPath { get; private set; }

        public string FileStoreModuleID { get; private set; }

        public string ThreadParamName{ get; private set;}

        public string TopicParamName { get; private set; }

        public string TagParamName { get; private set; }

        public string ActionParamName { get; private set; }
        public string PostParamName { get; private set; }

        public string NewPostPageVirtualPath { get; private set; }
        public string NewPostPageAbsolutePath { get { return GetAbsolutePathWithParams(NewPostPageVirtualPath); } }

        public string SearchPageVirtualPath { get; private set; }
        public string SearchPageAbsolutePath { get { return GetAbsolutePathWithParams(SearchPageVirtualPath); } }
        
        public string StartPageVirtualPath { get; private set; }
        public string StartPageAbsolutePath { get { return GetAbsolutePathWithParams(StartPageVirtualPath); } }

        public string TopicPageVirtualPath { get; private set; }
        public string TopicPageAbsolutePath { get { return GetAbsolutePathWithParams(TopicPageVirtualPath); } }

        public string EditTopicPageVirtualPath { get; private set; }
        public string EditTopicPageAbsolutePath { get { return GetAbsolutePathWithParams(EditTopicPageVirtualPath); } }

        public string PostPageVirtualPath { get; private set; }
        public string PostPageAbsolutePath { get { return GetAbsolutePathWithParams(PostPageVirtualPath); } }

        public Settings()
        {
            ID = Guid.NewGuid();
            TopicCountOnPage = 20;
            PostCountOnPage = 20;

            ThreadParamName = "f";
            TopicParamName = "t";
            TagParamName = "tag";
            ActionParamName = "a";
            PostParamName = "m";

            ModuleID = new Guid("853B6EB9-73EE-438d-9B09-8FFEEDF36234");
            ImageItemID = new Guid("853B6EB9-73EE-438d-9B09-8FFEEDF36234");
            UserControlsVirtualPath = "~/Products/Community/Modules/Forum/UserControls";
            StartPageVirtualPath = "~/Products/Community/Modules/Forum/Default.aspx";
            TopicPageVirtualPath = "~/Products/Community/Modules/Forum/Topics.aspx";
            PostPageVirtualPath = "~/Products/Community/Modules/Forum/Posts.aspx";
            SearchPageVirtualPath = "~/Products/Community/Modules/Forum/Search.aspx";
            NewPostPageVirtualPath = "~/Products/Community/Modules/Forum/NewPost.aspx";
            EditTopicPageVirtualPath = "~/Products/Community/Modules/Forum/EditTopic.aspx";
            FileStoreModuleID = "forum";

            LinkProvider = new LinkProvider(this);
            //registry
            ForumManager = new ForumManager(this);
        }

        private static string GetAbsolutePathWithParams(string virtualPath)
        {
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if(virtualPath.IndexOf("?")!=-1)
                    return VirtualPathUtility.ToAbsolute(virtualPath.Split('?')[0]) + "?" + virtualPath.Split('?')[1];
                
                return VirtualPathUtility.ToAbsolute(virtualPath) + "?";
            }

            return string.Empty;
        }

        internal string[] GetAllPageAdditionalParams()
        {
            var result = new List<string>();

            result.AddRange(GetPageAdditionalParams(PostPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(TopicPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(SearchPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(SearchPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(NewPostPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(EditTopicPageVirtualPath));

            return result.ToArray();
        }

        private static string[] GetPageAdditionalParams(string url)
        {
            if (string.IsNullOrEmpty(url) || url.IndexOf("?") == -1) return new string[0];

            var query = url.Split('?')[1];
            if (!string.IsNullOrEmpty(query))
            {
                var result = new List<string>();
                foreach (var param in query.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(param.Split('=')[0]);
                }

                return result.ToArray();
            }

            return new string[0];
        }
    }
}
