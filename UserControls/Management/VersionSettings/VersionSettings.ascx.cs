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
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio.UserControls.Management.VersionSettings
{
    [AjaxNamespace("VersionSettingsController")]
    public partial class VersionSettings : UserControl
    {
        public const string Location = "~/UserControls/Management/VersionSettings/VersionSettings.ascx";

        protected string HelpLink { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            HelpLink = CommonLinkUtility.GetHelpLink();

            Page.RegisterStyle("~/UserControls/Management/VersionSettings/css/versionsettings.less")
                .RegisterBodyScripts("~/UserControls/Management/VersionSettings/js/script.js");
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SwitchVersion(string version)
        {
            try
            {
                var tenantVersion = int.Parse(version);

                if (CoreContext.TenantManager.GetTenantVersions().All(x => x.Id != tenantVersion))
                    throw new ArgumentException(Resource.SettingsBadPortalVersion);

                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
                var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                try
                {
                    CoreContext.TenantManager.SetTenantVersion(tenant, tenantVersion);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException(Resource.SettingsAlreadyCurrentPortalVersion, e);
                }
                return new { Status = 1 };
            }
            catch (Exception e)
            {
                return new { Status = 0, e.Message };
            }
        }

        protected string GetLocalizedName(string name)
        {
            try
            {
                var localizedName = Resource.ResourceManager.GetString(("version_" + name.Replace(".", "")).ToLowerInvariant());
                if (string.IsNullOrEmpty(localizedName))
                {
                    localizedName = name;
                }
                return localizedName;
            }
            catch (Exception)
            {
                return name;
            }
        }
    }
}