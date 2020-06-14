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
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.MessagingSystem;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class ConfirmInviteActivation : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/ConfirmInviteActivation/ConfirmInviteActivation.ascx"; }
        }

        protected string _errorMessage;

        protected string _userName;
        protected string _userPost;
        protected string _userAvatar;

        protected string _email
        {
            get { return (Request["email"] ?? String.Empty).Trim(); }
        }

        protected ConfirmType _type
        {
            get { return typeof (ConfirmType).TryParseEnum(Request["type"] ?? "", ConfirmType.EmpInvite); }
        }

        protected EmployeeType _employeeType
        {
            get { return typeof(EmployeeType).TryParseEnum(Request["emplType"] ?? "", EmployeeType.User); }
        }

        protected string GetEmailAddress()
        {
            if (!String.IsNullOrEmpty(Request["emailInput"]))
                return Request["emailInput"].Trim();

            if (!String.IsNullOrEmpty(Request["email"]))
                return Request["email"].Trim();

            return String.Empty;
        }

        private string GetEmailAddress(LoginProfile account)
        {
            var value = GetEmailAddress();
            return String.IsNullOrEmpty(value) ? account.EMail : value;
        }

        protected string GetFirstName()
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(Request["firstname"])) value = Request["firstname"].Trim();
            if (!string.IsNullOrEmpty(Request["firstnameInput"])) value = Request["firstnameInput"].Trim();
            return HtmlUtil.GetText(value);
        }

        private string GetFirstName(LoginProfile account)
        {
            var value = GetFirstName();
            return String.IsNullOrEmpty(value) ? account.FirstName : value;
        }

        protected string GetLastName()
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(Request["lastname"])) value = Request["lastname"].Trim();
            if (!string.IsNullOrEmpty(Request["lastnameInput"])) value = Request["lastnameInput"].Trim();
            return HtmlUtil.GetText(value);
        }

        private string GetLastName(LoginProfile account)
        {
            var value = GetLastName();
            return String.IsNullOrEmpty(value) ? account.LastName : value;
        }

        protected bool isPersonal {
            get { return CoreContext.Configuration.Personal; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts("~/js/third-party/xregexp.js", "~/UserControls/Management/ConfirmInviteActivation/js/confirm_invite_activation.js")
                .RegisterStyle("~/UserControls/Management/ConfirmInviteActivation/css/confirm_invite_activation.less");

            var uid = Guid.Empty;
            try
            {
                uid = new Guid(Request["uid"]);
            }
            catch
            {
            }

            var email = GetEmailAddress();

            if (_type != ConfirmType.Activation && AccountLinkControl.IsNotEmpty && !CoreContext.Configuration.Personal)
            {
                var thrd = (AccountLinkControl) LoadControl(AccountLinkControl.Location);
                thrd.InviteView = true;
                thrd.ClientCallback = "loginJoinCallback";
                thrdParty.Visible = true;
                thrdParty.Controls.Add(thrd);
            }

            Page.Title = HeaderStringHelper.GetPageTitle(Resource.Authorization);

            UserInfo user;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

                user = CoreContext.UserManager.GetUserByEmail(email);
                var usr = CoreContext.UserManager.GetUsers(uid);
                if (usr.ID.Equals(ASC.Core.Users.Constants.LostUser.ID) || usr.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID))
                    usr = CoreContext.UserManager.GetUsers(CoreContext.TenantManager.GetCurrentTenant().OwnerId);

                var photoData = UserPhotoManager.GetUserPhotoData(usr.ID, UserPhotoManager.MediumFotoSize);

                _userAvatar = photoData == null ? usr.GetMediumPhotoURL() : "data:image/png;base64," + Convert.ToBase64String(photoData);
                _userName = usr.DisplayUserName(true);
                _userPost = (usr.Title ?? "").HtmlEncode();
            }
            finally
            {
                SecurityContext.Logout();
            }

            if (_type == ConfirmType.LinkInvite || _type == ConfirmType.EmpInvite)
            {
                if (TenantStatisticsProvider.GetUsersCount() >= TenantExtra.GetTenantQuota().ActiveUsers && _employeeType == EmployeeType.User)
                {
                    ShowError(UserControlsCommonResource.TariffUserLimitReason);
                    return;
                }

                if (!user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    ShowError(CustomNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
                    return;
                }
            }

            else if (_type == ConfirmType.Activation)
            {
                if (user.IsActive)
                {
                    Response.Redirect(CommonLinkUtility.GetDefault());
                    return;
                }

                if (user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID) || user.Status == EmployeeStatus.Terminated)
                {
                    ShowError(string.Format(Resource.ErrorUserNotFoundByEmail, email));
                    return;
                }
            }

            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            if (tenant != null)
            {
                var settings = IPRestrictionsSettings.Load();
                if (settings.Enable && !IPSecurity.IPSecurity.Verify(tenant))
                {
                    ShowError(Resource.ErrorAccessRestricted);
                    return;
                }
            }

            if (!IsPostBack)
                return;

            var firstName = GetFirstName();
            var lastName = GetLastName();
            var pwd = (Request["pwdInput"] ?? "").Trim();
            var analytics = (Request["analytics"] ?? "").Trim() == "True"; 
            var mustChangePassword = false;
            LoginProfile thirdPartyProfile;

            //thirdPartyLogin confirmInvite
            if (Request["__EVENTTARGET"] == "thirdPartyLogin")
            {
                var valueRequest = Request["__EVENTARGUMENT"];
                thirdPartyProfile = new LoginProfile(valueRequest);

                if (!string.IsNullOrEmpty(thirdPartyProfile.AuthorizationError))
                {
                    // ignore cancellation
                    if (thirdPartyProfile.AuthorizationError != "Canceled at provider")
                        ShowError(HttpUtility.HtmlEncode(thirdPartyProfile.AuthorizationError));
                    return;
                }

                if (string.IsNullOrEmpty(thirdPartyProfile.EMail))
                {
                    ShowError(HttpUtility.HtmlEncode(Resource.ErrorNotCorrectEmail));
                    return;
                }
            }

            if (Request["__EVENTTARGET"] == "confirmInvite")
            {
                if (String.IsNullOrEmpty(email))
                {
                    _errorMessage = Resource.ErrorEmptyUserEmail;
                    return;
                }

                if (!email.TestEmailRegex())
                {
                    _errorMessage = Resource.ErrorNotCorrectEmail;
                    return;
                }

                if (String.IsNullOrEmpty(firstName))
                {
                    _errorMessage = Resource.ErrorEmptyUserFirstName;
                    return;
                }

                if (String.IsNullOrEmpty(lastName))
                {
                    _errorMessage = Resource.ErrorEmptyUserLastName;
                    return;
                }

                var checkPassResult = CheckPassword(pwd);
                if (!String.IsNullOrEmpty(checkPassResult))
                {
                    _errorMessage = checkPassResult;
                    return;
                }
            }
            var userID = Guid.Empty;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                if (_type == ConfirmType.EmpInvite || _type == ConfirmType.LinkInvite)
                {
                    if (TenantStatisticsProvider.GetUsersCount() >= TenantExtra.GetTenantQuota().ActiveUsers && _employeeType == EmployeeType.User)
                    {
                        ShowError(UserControlsCommonResource.TariffUserLimitReason);
                        return;
                    }

                    UserInfo newUser;
                    if (Request["__EVENTTARGET"] == "confirmInvite")
                    {
                        var fromInviteLink = _type == ConfirmType.LinkInvite;
                        newUser = CreateNewUser(firstName, lastName, email, pwd, _employeeType, fromInviteLink);

                        var messageAction = _employeeType == EmployeeType.User ? MessageAction.UserCreatedViaInvite : MessageAction.GuestCreatedViaInvite;
                        MessageService.Send(HttpContext.Current.Request, MessageInitiator.System, messageAction, MessageTarget.Create(newUser.ID), newUser.DisplayUserName(false));
                        
                        userID = newUser.ID;

                        var settings = TenantAnalyticsSettings.LoadForCurrentUser();
                        settings.Analytics = analytics;

                        settings.SaveForCurrentUser();

                    }

                    if (Request["__EVENTTARGET"] == "thirdPartyLogin")
                    {
                        if (!String.IsNullOrEmpty(CheckPassword(pwd)))
                        {
                            pwd = UserManagerWrapper.GeneratePassword();
                            mustChangePassword = true;
                        }
                        var valueRequest = Request["__EVENTARGUMENT"];
                        thirdPartyProfile = new LoginProfile(valueRequest);
                        newUser = CreateNewUser(GetFirstName(thirdPartyProfile), GetLastName(thirdPartyProfile), GetEmailAddress(thirdPartyProfile), pwd, _employeeType, false);

                        var messageAction = _employeeType == EmployeeType.User ? MessageAction.UserCreatedViaInvite : MessageAction.GuestCreatedViaInvite;
                        MessageService.Send(HttpContext.Current.Request, MessageInitiator.System, messageAction, MessageTarget.Create(newUser.ID), newUser.DisplayUserName(false));
                        
                        userID = newUser.ID;
                        if (!String.IsNullOrEmpty(thirdPartyProfile.Avatar))
                        {
                            SaveContactImage(userID, thirdPartyProfile.Avatar);
                        }

                        var linker = new AccountLinker("webstudio");
                        linker.AddLink(userID.ToString(), thirdPartyProfile);
                    }
                }
                else if (_type == ConfirmType.Activation)
                {
                    if (!UserFormatter.IsValidUserName(firstName, lastName))
                        throw new Exception(Resource.ErrorIncorrectUserName);

                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    CoreContext.UserManager.SaveUserInfo(user);
                    SecurityContext.SetUserPassword(user.ID, pwd);

                    userID = user.ID;

                    //notify
                    if (user.IsVisitor()) { 
                        StudioNotifyService.Instance.GuestInfoAddedAfterInvite(user);
                        MessageService.Send(HttpContext.Current.Request, MessageInitiator.System, MessageAction.GuestActivated, MessageTarget.Create(user.ID), user.DisplayUserName(false));
                    }
                    else
                    {
                        StudioNotifyService.Instance.UserInfoAddedAfterInvite(user);
                        MessageService.Send(HttpContext.Current.Request, MessageInitiator.System, MessageAction.UserActivated, MessageTarget.Create(user.ID), user.DisplayUserName(false));
                    }
                }
            }
            catch (Exception exception)
            {
                _errorMessage = HttpUtility.HtmlEncode(exception.Message);
                return;
            }
            finally
            {
                SecurityContext.Logout();
            }

            user = CoreContext.UserManager.GetUsers(userID);
            try
            {
                var cookiesKey = SecurityContext.AuthenticateMe(user.Email, pwd);
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                MessageService.Send(HttpContext.Current.Request, MessageAction.LoginSuccess);
                StudioNotifyService.Instance.UserHasJoin();

                if (mustChangePassword)
                {
                    StudioNotifyService.Instance.UserPasswordChange(user);
                }
            }
            catch (Exception exception)
            {
                (Page as Confirm).ErrorMessage = HttpUtility.HtmlEncode(exception.Message);
                return;
            }

            UserHelpTourHelper.IsNewUser = true;
            if (CoreContext.Configuration.Personal)
                PersonalSettings.IsNewUser = true;
            Response.Redirect(CommonLinkUtility.GetDefault());
        }

        private static void SaveContactImage(Guid userID, string url)
        {
            using (var memstream = new MemoryStream())
            {
                var req = WebRequest.Create(url);
                using (var response = req.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var buffer = new byte[512];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        memstream.Write(buffer, 0, bytesRead);
                    var bytes = memstream.ToArray();

                    UserPhotoManager.SaveOrUpdatePhoto(userID, bytes);
                }
            }
        }

        private void ShowError(string message)
        {
            (Page as Confirm).ErrorMessage = message.Trim();
            (Page as Confirm)._confirmHolder2.Visible = false;
            (Page as Confirm)._confirmHolder.Visible = false;
            (Page as Confirm)._contentWithControl.Visible = true;

            if (SecurityContext.IsAuthenticated == false)
                (Page as Confirm).ErrorMessage +=
                    (message.EndsWith(".") ? "" : ".")
                    + " "
                    + String.Format(Resource.ForSignInFollowMessage,
                                    string.Format("<a href=\"{0}\">",
                                                  VirtualPathUtility.ToAbsolute("~/auth.aspx")),
                                    "</a>");

            _confirmHolder.Visible = false;
        }

        private static string CheckPassword(string pwd)
        {
            try
            {
                UserManagerWrapper.CheckPasswordPolicy(pwd);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return String.Empty;
        }

        private static UserInfo CreateNewUser(string firstName, string lastName, string email, string pwd, EmployeeType employeeType, bool fromInviteLink)
        {
            var isVisitor = employeeType == EmployeeType.Visitor;

            if (SetupInfo.IsSecretEmail(email))
            {
                fromInviteLink = false;
            }

            var userInfo = new UserInfo
            {
                FirstName = string.IsNullOrEmpty(firstName) ? UserControlsCommonResource.UnknownFirstName : firstName,
                LastName = string.IsNullOrEmpty(lastName) ? UserControlsCommonResource.UnknownLastName : lastName,
                Email = email,
            };

            if (CoreContext.Configuration.Personal)
            {
                userInfo.ActivationStatus = EmployeeActivationStatus.Activated;
                userInfo.CultureName = CoreContext.Configuration.CustomMode ? "ru-RU" : Thread.CurrentThread.CurrentUICulture.Name;
            }

            return UserManagerWrapper.AddUser(userInfo, pwd, true, true, isVisitor, fromInviteLink);
        }
    }
}