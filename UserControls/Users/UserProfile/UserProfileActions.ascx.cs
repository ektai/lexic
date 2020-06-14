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
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.PublicResources;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using System.Web;
using Resources;

using LdapMapping = ASC.ActiveDirectory.Base.Settings.LdapSettings.MappingFields;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class UserProfileActions : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/UserProfileActions.ascx"; }
        }

        public ProfileHelper ProfileHelper { get; set; }
        protected List<LdapMapping> LdapFields { get; set; }

        protected AllowedActions Actions;
        protected string ProfileEditLink;
        protected string ReassignDataLink;
        protected bool MyStaff;
        protected bool HasActions;
        protected bool IsAdmin;
        protected string SubscribeBtnText;

        protected void Page_Load(object sender, EventArgs e)
        {
            Actions = new AllowedActions(ProfileHelper.UserInfo);
            MyStaff = ProfileHelper.UserInfo.IsMe();
            HasActions = Actions.AllowEdit || Actions.AllowAddOrDelete;

            var currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            IsAdmin = currentUser.IsAdmin();

            SubscribeBtnText =
                StudioNotifyHelper.IsSubscribedToNotify(currentUser, Core.Notify.Actions.PeriodicNotify)
                    ? ResourceJS.TipsAndTricksUnsubscribeBtn
                    : ResourceJS.TipsAndTricksSubscribeBtn;

            if (HasActions && Actions.AllowAddOrDelete)
            {
                _phConfirmationDeleteUser.Controls.Add(LoadControl(ConfirmationDeleteUser.Location));
            }

            ProfileEditLink =
                Page is MyStaff
                    ? "/my.aspx?action=edit"
                    : "profileaction.aspx?action=edit&user=" + HttpUtility.UrlEncode(ProfileHelper.UserInfo.UserName);

            ReassignDataLink = "reassigns.aspx?user=" + HttpUtility.UrlEncode(ProfileHelper.UserInfo.UserName);

            LdapFields = ASC.ActiveDirectory.Base.Settings.LdapSettings.GetImportedFields;

            if (Request.Params["email_change"] == "success")
            {
                Page.RegisterInlineScript(string.Format("toastr.success(\"{0}\");", Resource.ChangeEmailSuccess));
            }
        }
    }
}