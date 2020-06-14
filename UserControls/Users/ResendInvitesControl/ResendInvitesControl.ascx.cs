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
using System.Web;
using System.Web.UI;
using ASC.MessagingSystem;
using AjaxPro;
using ASC.Web.Studio.Core.Notify;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Users
{
    [AjaxNamespace("InviteResender")]
    public partial class ResendInvitesControl : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/ResendInvitesControl/ResendInvitesControl.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts("~/UserControls/Users/ResendInvitesControl/js/resendinvitescontrol.js");

            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            _invitesResenderContainer.Options.IsPopup = true;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object Resend()
        {
            try
            {
                foreach (var user in new List<UserInfo>(CoreContext.UserManager.GetUsers())
                    .FindAll(u => u.ActivationStatus == EmployeeActivationStatus.Pending))
                {
                    if (user.IsVisitor())
                    {
                        StudioNotifyService.Instance.GuestInfoActivation(user);
                    }
                    else
                    {
                        StudioNotifyService.Instance.UserInfoActivation(user);
                    }
                    MessageService.Send(HttpContext.Current.Request, MessageAction.UserSentActivationInstructions, MessageTarget.Create(user.ID), user.DisplayUserName(false));
                }

                return new {status = 1, message = Resources.Resource.SuccessResendInvitesText};
            }
            catch(Exception e)
            {
                return new {status = 0, message = e.Message.HtmlEncode()};
            }
        }

        public static string GetHrefAction()
        {
            return "javascript:InvitesResender.Show();";
        }
    }
}