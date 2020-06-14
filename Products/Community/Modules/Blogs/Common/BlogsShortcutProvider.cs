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
using ASC.Blogs.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Core;
using ASC.Blogs.Core.Security;

namespace ASC.Web.Community.Blogs
{    
    public class BlogsShortcutProvider : IShortcutProvider
    {
        public static string GetCreateContentPageUrl()
        {
            if (SecurityContext.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(
                    SecurityContext.CurrentAccount.ID)), Constants.Action_AddPost))

                return VirtualPathUtility.ToAbsolute(Constants.BaseVirtualPath + "addblog.aspx");

            return null;
        }

        public string GetAbsoluteWebPathForShortcut(Guid shortcutID, string currentUrl)
        {
            return "";
        }

        public bool CheckPermissions(Guid shortcutID, string currentUrl)
        {
            if (shortcutID.Equals(new Guid("98DB8D88-EDF2-4f82-B3AF-B95E87E3EE5C")) || 
                shortcutID.Equals(new Guid("20673DF0-665E-4fc8-9B44-D48B2A783508")))
            {
                return SecurityContext.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(
                    SecurityContext.CurrentAccount.ID)), Constants.Action_AddPost);
            }            
            
            return false;
        }
    }
}
