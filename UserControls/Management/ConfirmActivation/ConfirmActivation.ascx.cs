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
using System.Collections.Specialized;
using System.Threading;
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using Resources;
using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("ConfirmActivation")]
    public partial class ConfirmActivation : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/ConfirmActivation/ConfirmActivation.ascx"; }
        }

        protected UserInfo User { get; set; }
        protected ConfirmType Type { get; set; }

        protected bool isPersonal
        {
            get { return CoreContext.Configuration.Personal; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(ConfirmActivation));
            Page.RegisterBodyScripts("~/UserControls/Management/ConfirmActivation/js/confirmactivation.js")
                .RegisterStyle("~/UserControls/Management/ConfirmActivation/css/confirmactivation.less");
            Page.Title = HeaderStringHelper.GetPageTitle(Resource.Authorization);
            ButtonEmailAndPasswordOK.Text = Resource.EmailAndPasswordOK;
            btChangeEmail.Text = Resource.ChangeEmail;

            var email = (Request["email"] ?? "").Trim();
            if (string.IsNullOrEmpty(email))
            {
                ShowError(Resource.ErrorNotCorrectEmail);
                return;
            }

            Type = typeof(ConfirmType).TryParseEnum(Request["type"] ?? "", ConfirmType.EmpInvite);

            try
            {
                if (Type == ConfirmType.EmailChange)
                {
                    User = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                    User.Email = email;
                    CoreContext.UserManager.SaveUserInfo(User);
                    MessageService.Send(Request, MessageAction.UserUpdatedEmail);

                    ActivateMail(User);
                    const string successParam = "email_change=success";
                    if(User.IsVisitor())
                    {
                        Response.Redirect(CommonLinkUtility.ToAbsolute("~/my.aspx?" + successParam));
                    }
                    Response.Redirect(string.Format("{0}&{1}", User.GetUserProfilePageURL(), successParam));
                }
                else
                {
                    User = CoreContext.UserManager.GetUserByEmail(email);
                    if (User.ID.Equals(Constants.LostUser.ID))
                    {
                        ShowError(string.Format(Resource.ErrorUserNotFoundByEmail, email));
                        return;
                    }

                    UserAuth(User);
                    ActivateMail(User);
                }

                if (Type == ConfirmType.PasswordChange)
                {
                    passwordSetter.Visible = true;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void UserAuth(UserInfo user)
        {
            if (SecurityContext.IsAuthenticated) return;

            if (StudioSmsNotificationSettings.IsVisibleSettings && StudioSmsNotificationSettings.Enable)
            {
                Session["refererURL"] = Request.GetUrlRewriter().AbsoluteUri;
                Response.Redirect(Confirm.SmsConfirmUrl(user), true);
                return;
            }

            var cookiesKey = SecurityContext.AuthenticateMe(user.ID);
            CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
            MessageService.Send(Request, MessageAction.LoginSuccess);
        }

        private void ActivateMail(UserInfo user)
        {
            if (user.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated)) return;

            user.ActivationStatus = EmployeeActivationStatus.Activated;
            CoreContext.UserManager.SaveUserInfo(user);

            MessageService.Send(Request,
                                user.IsVisitor() ? MessageAction.GuestActivated : MessageAction.UserActivated,
                                MessageTarget.Create(user.ID),
                                user.DisplayUserName(false));
        }

        private void ShowError(string message, bool redirect = true)
        {
            var confirm = Page as Confirm;
            if (confirm != null)
                confirm.ErrorMessage = HttpUtility.HtmlEncode(message);

            Auth.ProcessLogout();

            if (redirect)
                RegisterRedirect();
        }

        private void RegisterRedirect()
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "redirect",
                                                    string.Format("setTimeout('location.href = \"{0}\";',10000);", CommonLinkUtility.GetDefault()),
                                                    true);
        }

        protected void LoginToPortal(object sender, EventArgs e)
        {
            try
            {
                var pwd = Request.Form["pwdInput"];

                UserManagerWrapper.CheckPasswordPolicy(pwd);

                SecurityContext.SetUserPassword(User.ID, pwd);
                MessageService.Send(Request, MessageAction.UserUpdatedPassword);

                CookiesManager.ResetUserCookie();
                MessageService.Send(Request, MessageAction.CookieSettingsUpdated);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, false);
                return;
            }

            Response.Redirect(CommonLinkUtility.GetDefault());
        }

        [AjaxMethod]
        public AjaxResponse SendEmailActivationInstructionsOnChange(string newEmail, string queryString)
        {
            const string StatusSuccess = "success";
            const string StatusError = "error";
            const string StatusFatalError = "fatalerror";

            var response = new AjaxResponse { status = StatusSuccess };

            if (String.IsNullOrEmpty(queryString))
            {
                response.status = StatusFatalError;
                response.message = Resource.ErrorConfirmURLError;
                return response;
            }

            if (String.IsNullOrEmpty(newEmail))
            {
                response.status = StatusError;
                response.message = Resource.ErrorEmailEmpty;
                return response;
            }

            try
            {
                var result = CheckValidationKey(queryString.Substring(1));
                if (result != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    response.status = StatusFatalError;
                    switch (result)
                    {
                        case EmailValidationKeyProvider.ValidationResult.Invalid:
                            response.message = Resource.ErrorInvalidActivationLink;
                            break;
                        case EmailValidationKeyProvider.ValidationResult.Expired:
                            response.message = Resource.ErrorExpiredActivationLink;
                            break;
                        default:
                            response.message = Resource.ErrorConfirmURLError;
                            break;
                    }
                    return response;
                }

                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                if (user == null)
                {
                    response.status = StatusFatalError;
                    response.message = Resource.ErrorUserNotFound;
                    return response;
                }

                var existentUser = CoreContext.UserManager.GetUserByEmail(newEmail);
                if (existentUser != null && existentUser.ID == user.ID)
                {
                    response.status = StatusError;
                    response.message = Resource.ErrorEmailsAreTheSame;
                    return response;
                }

                if (existentUser != null && existentUser.ID != Constants.LostUser.ID)
                {
                    response.status = StatusError;
                    response.message = CustomNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists");
                    return response;
                }

                user.Email = newEmail;
                user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                CoreContext.UserManager.SaveUserInfo(user);

                StudioNotifyService.Instance.SendEmailActivationInstructions(user, newEmail);
                MessageService.Send(Request, MessageAction.UserSentActivationInstructions, MessageTarget.Create(user.ID), user.DisplayUserName(false));

                response.message = String.Format(Resource.MessageEmailActivationInstuctionsSentOnEmail, "<b>" + newEmail + "</b>");
                return response;
            }
            catch (Exception)
            {
                response.status = StatusFatalError;
                response.message = Resource.UnknownError;
                return response;
            }
        }

        private static EmailValidationKeyProvider.ValidationResult CheckValidationKey(string queryString)
        {
            var request = BuildRequestFromQueryString(queryString);

            var type = request["type"];
            if (String.IsNullOrEmpty(type) || type.ToLowerInvariant() != ConfirmType.EmailChange.ToString().ToLowerInvariant())
            {
                return EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            var email = (request["email"] ?? "").Trim();
            if (String.IsNullOrEmpty(email) || !email.TestEmailRegex())
            {
                return EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            var key = request["key"];
            if (String.IsNullOrEmpty(key))
            {
                return EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            return EmailValidationKeyProvider.ValidateEmailKey(email + type, key, SetupInfo.ValidEmailKeyInterval);
        }

        private static NameValueCollection BuildRequestFromQueryString(string queryString)
        {
            return HttpUtility.ParseQueryString(queryString);
        }
    }
}