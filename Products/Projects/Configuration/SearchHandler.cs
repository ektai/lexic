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
using System.Linq;

using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;

using ASC.Projects.Engine;
using ASC.Web.Projects.Core;
using Autofac;

namespace ASC.Web.Projects.Configuration
{
    public class SearchHandler : BaseSearchHandlerEx
    {
        public override Guid ProductID
        {
            get { return ProductEntryPoint.ID; }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "common_search_icon.png" }; }
        }

        public override Guid ModuleID
        {
            get { return ProductID; }
        }

        public override string SearchName
        {
            get { return ProjectsCommonResource.SearchText; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }

        public override SearchResultItem[] Search(string text)
        {
            using (var scope = DIHelper.Resolve())
            {
                return scope.Resolve<EngineFactory>().SearchEngine.Search(text).Select(GetSearchResultItem).ToArray();
            }
        }

        public SearchResultItem GetSearchResultItem(SearchItem searchResultItem)
        {
            return new SearchResultItem
                {
                    Name = searchResultItem.Title,
                    Additional = searchResultItem.GetAdditional(),
                    URL = searchResultItem.ItemPath,
                    Date = searchResultItem.CreateOn,
                    Description = searchResultItem.Description
                };
        }
    }
}