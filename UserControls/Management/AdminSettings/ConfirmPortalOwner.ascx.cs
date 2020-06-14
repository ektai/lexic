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
using System.Web.UI;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Users;
using ASC.MessagingSystem;
using Resources;

namespace ASC.Web.Studio.UserControls.Management
{
    partial class ConfirmPortalOwner : UserControl
    {
        public static readonly string Location = "~/UserControls/Management/AdminSettings/ConfirmPortalOwner.ascx";

        protected string _newOwnerName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var newOwner = Constants.LostUser;
                try
                {
                    newOwner = CoreContext.UserManager.GetUsers(new Guid(Request["uid"]));
                }
                catch
                {
                }
                if (Constants.LostUser.Equals(newOwner))
                {
                    throw new Exception(Resource.ErrorUserNotFound);
                }
                _newOwnerName = newOwner.DisplayUserName();

                if (IsPostBack)
                {
                    if (CoreContext.UserManager.IsUserInGroup(newOwner.ID, Constants.GroupVisitor.ID))
                    {
                        throw new Exception(Resource.ErrorUserNotFound);
                    }

                    var curTenant = CoreContext.TenantManager.GetCurrentTenant();
                    curTenant.OwnerId = newOwner.ID;
                    CoreContext.TenantManager.SaveTenant(curTenant);

                    MessageService.Send(HttpContext.Current.Request, MessageAction.OwnerUpdated, newOwner.DisplayUserName(false));

                    _messageHolder.Visible = true;
                    _confirmContentHolder.Visible = false;
                }
                else
                {
                    _messageHolder.Visible = false;
                    _confirmContentHolder.Visible = true;
                }
            }
            catch(Exception err)
            {
                ((Confirm)Page).ErrorMessage = err.Message.HtmlEncode();
                _messageHolder.Visible = true;
                _confirmContentHolder.Visible = false;
                LogManager.GetLogger("ASC.Web").Error(err);
            }
        }
    }
}