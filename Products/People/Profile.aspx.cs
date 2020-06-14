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
using System.Text;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Studio;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.TipsSettings;
using ASC.Web.Studio.Utility;

namespace ASC.Web.People
{
    public partial class Profile : MainPage
    {
        public ProfileHelper ProfileHelper;

        protected bool IsAdmin()
        {
            return WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ProfileHelper = new ProfileHelper(Request["user"]);

            Title = HeaderStringHelper.GetPageTitle(ProfileHelper.UserInfo.DisplayUserName(false));

            var control = (UserProfileControl)LoadControl(UserProfileControl.Location);
            control.UserProfileHelper = ProfileHelper;

            CommonContainerHolder.Controls.Add(control);

            var actions = (UserProfileActions)LoadControl(UserProfileActions.Location);
            actions.ProfileHelper = ProfileHelper;
            actionsHolder.Controls.Add(actions);

            if (ProfileHelper.UserInfo.IsMe())
            {
                InitSubscriptionView();
                InitTipsSettingsView();

                var script = new StringBuilder();
                script.Append("jq('#switcherSubscriptionButton').one('click',");
                script.Append("function() {");
                script.Append("if (!jq('#subscriptionBlockContainer').hasClass('subsLoaded') &&");
                script.Append("typeof (window.CommonSubscriptionManager) != 'undefined' &&");
                script.Append("typeof (window.CommonSubscriptionManager.LoadSubscriptions) === 'function') {");
                script.Append("window.CommonSubscriptionManager.LoadSubscriptions();");
                script.Append("jq('#subscriptionBlockContainer').addClass('subsLoaded');");
                script.Append("}});");

                Page.RegisterInlineScript(script.ToString());
            }
        }

        private void InitSubscriptionView()
        {
            _phSubscriptionView.Controls.Add(LoadControl(UserSubscriptions.Location));
        }

        private void InitTipsSettingsView()
        {
            if (!string.IsNullOrEmpty(Studio.Core.SetupInfo.TipsAddress))
                _phTipsSettingsView.Controls.Add(LoadControl(TipsSettings.Location));
        }
    }
}