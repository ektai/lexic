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
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.People.Resources;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;

namespace ASC.Web.People
{
    public partial class Reassigns : MainPage
    {
        protected UserInfo UserInfo { get; private set; }

        protected string PageTitle { get; private set; }

        protected string HelpLink { get; private set; }

        protected string ProfileLink { get; private set; }

        protected bool RemoveData { get; private set; }

        protected bool DeleteProfile { get; private set; }

        protected bool IsAdmin()
        {
            return WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var username = Request["user"];

            if (!IsAdmin() || string.IsNullOrEmpty(username))
            {
                Response.Redirect("~/products/people/", true);
            }

            UserInfo = CoreContext.UserManager.GetUserByUserName(Request["user"]);

            if(UserInfo.Status != EmployeeStatus.Terminated)
            {
                Response.Redirect("~/products/people/", true);
            }

            RemoveData = string.Equals(Request["remove"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase);

            DeleteProfile = string.Equals(Request["delete"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase);

            PageTitle = UserInfo.DisplayUserName(false) + " - " + (RemoveData ? PeopleResource.RemovingData : PeopleResource.ReassignmentData);

            Title = HeaderStringHelper.GetPageTitle(PageTitle);

            PageTitle = HttpUtility.HtmlEncode(PageTitle);

            HelpLink = CommonLinkUtility.GetHelpLink();

            ProfileLink = CommonLinkUtility.GetUserProfile(UserInfo.ID);

            Page.RegisterInlineScript(string.Format("ASC.People.Reassigns.init(\"{0}\", {1});", UserInfo.ID, RemoveData.ToString().ToLowerInvariant()));
        }
    }
}