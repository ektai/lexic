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
using System.Configuration;
using System.Globalization;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio.Masters.MasterResources
{
    public class MasterSettingsResources : ClientScript
    {
        protected override bool CheckAuth { get { return false; } }

        protected override string BaseNamespace
        {
            get { return "ASC.Resources.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var curQuota = TenantExtra.GetTenantQuota();
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            var helpLink = CommonLinkUtility.GetHelpLink();

            var result = new List<KeyValuePair<string, object>>(4)
            {
                RegisterObject(
                new { 
                        ApiPath = SetupInfo.WebApiBaseUrl,
                        IsAuthenticated = SecurityContext.IsAuthenticated,
                        IsAdmin = CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID),
                        IsVisitor = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor(),
                        //CurrentTenantId = tenant.TenantId,
                        CurrentTenantCreatedDate = tenant.CreatedDateTime,
                        CurrentTenantVersion = tenant.Version,
                        CurrentTenantTimeZone = new
                            {
                                Id = tenant.TimeZone.Id,
                                DisplayName = Common.Utils.TimeZoneConverter.GetTimeZoneName(tenant.TimeZone),
                                BaseUtcOffset = tenant.TimeZone.GetOffset(true).TotalMinutes,
                                UtcOffset = tenant.TimeZone.GetOffset().TotalMinutes
                            },
                        TenantIsPremium = curQuota.Trial ? "No" : "Yes",
                        TenantTariff = curQuota.Id,
                        EmailRegExpr = @"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$",
                        UserNameRegExpr = UserFormatter.UserNameRegex,
                        GroupSelector_MobileVersionGroup = new { Id = -1, Name = UserControlsCommonResource.LblSelect.HtmlEncode().ReplaceSingleQuote() },
                        GroupSelector_WithGroupEveryone = new { Id = Constants.GroupEveryone.ID, Name = UserControlsCommonResource.Everyone.HtmlEncode().ReplaceSingleQuote() },
                        GroupSelector_WithGroupAdmin = new { Id = Constants.GroupAdmin.ID, Name = UserControlsCommonResource.Admin.HtmlEncode().ReplaceSingleQuote() },
                        SetupInfoNotifyAddress = SetupInfo.NotifyAddress,
                        SetupInfoTipsAddress = SetupInfo.TipsAddress,
                        CKEDITOR_BASEPATH = WebPath.GetPath("/UserControls/Common/ckeditor/"),
                        MaxImageFCKWidth = ConfigurationManager.AppSettings["MaxImageFCKWidth"] ?? "620",
                        UserPhotoHandlerUrl = VirtualPathUtility.ToAbsolute("~/UserPhoto.ashx"),
                        UserDefaultBigPhotoURL = UserPhotoManager.GetDefaultBigPhotoURL(),
                        ImageWebPath = WebImageSupplier.GetImageFolderAbsoluteWebPath(),
                        UrlShareTwitter = SetupInfo.ShareTwitterUrl,
                        UrlShareFacebook = SetupInfo.ShareFacebookUrl,
                        LogoDarkUrl = CommonLinkUtility.GetFullAbsolutePath(TenantLogoManager.GetLogoDark(true)),
                        HelpLink = helpLink ?? "",
                        MailMaximumMessageBodySize = ConfigurationManager.AppSettings["mail.maximum-message-body-size"] ?? "524288"
        })
            };

            if (CoreContext.Configuration.Personal)
            {
                result.Add(RegisterObject(new { CoreContext.Configuration.Personal }));
            }

            if (CoreContext.Configuration.CustomMode)
            {
                result.Add(RegisterObject(new { CoreContext.Configuration.CustomMode }));
            }

            if (CoreContext.Configuration.Standalone)
            {
                result.Add(RegisterObject(new { CoreContext.Configuration.Standalone }));
            }

            if (!string.IsNullOrEmpty(helpLink))
            {
                result.Add(RegisterObject(new { FilterHelpCenterLink = helpLink.TrimEnd('/') + "/tipstricks/using-search.aspx" }));
            }

            return result;
        }

        protected override string GetCacheHash()
        {
            /* return user last mod time + culture */
            var hash = SecurityContext.CurrentAccount.ID.ToString()
                       + CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).LastModified.ToString(CultureInfo.InvariantCulture)
                       + CoreContext.TenantManager.GetCurrentTenant().LastModified.ToString(CultureInfo.InvariantCulture);
            if (CoreContext.Configuration.Standalone)
            {
                // flush javascript for due tariff
                hash += DateTime.UtcNow.Date.ToString(CultureInfo.InvariantCulture);
            }
            return hash;
        }
    }
}