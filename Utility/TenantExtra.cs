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
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Web.Studio.Utility
{
    public static class TenantExtra
    {
        public static bool EnableTarrifSettings
        {
            get
            {
                return
                    SetupInfo.IsVisibleSettings<TariffSettings>()
                    && !TenantAccessSettings.Load().Anyone
                    && (!CoreContext.Configuration.Standalone || !string.IsNullOrEmpty(SetupInfo.ControlPanelUrl));
            }
        }

        public static bool Saas
        {
            get { return !CoreContext.Configuration.Standalone; }
        }

        public static bool Enterprise
        {
            get { return CoreContext.Configuration.Standalone && !String.IsNullOrEmpty(SetupInfo.ControlPanelUrl); }
        }

        public static bool Opensource
        {
            get { return CoreContext.Configuration.Standalone && String.IsNullOrEmpty(SetupInfo.ControlPanelUrl); }
        }

        public static bool EnterprisePaid
        {
            get { return Enterprise && GetTenantQuota().Id != Tenant.DEFAULT_TENANT && GetCurrentTariff().State < TariffState.NotPaid; }
        }

        public static bool EnableControlPanel
        {
            get { return Enterprise && GetTenantQuota().ControlPanel && GetCurrentTariff().State < TariffState.NotPaid && CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin(); }
        }

        public static string GetAppsPageLink()
        {
            return VirtualPathUtility.ToAbsolute("~/appinstall.aspx");
        }

        public static string GetTariffPageLink()
        {
            return VirtualPathUtility.ToAbsolute("~/tariffs.aspx");
        }

        public static Tariff GetCurrentTariff()
        {
            return CoreContext.PaymentManager.GetTariff(TenantProvider.CurrentTenantID);
        }

        public static TenantQuota GetTenantQuota()
        {
            return GetTenantQuota(TenantProvider.CurrentTenantID);
        }

        public static TenantQuota GetTenantQuota(int tenant)
        {
            return CoreContext.TenantManager.GetTenantQuota(tenant);
        }

        public static IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return CoreContext.TenantManager.GetTenantQuotas();
        }

        private static TenantQuota GetPrevQuota(TenantQuota curQuota)
        {
            TenantQuota prev = null;
            foreach (var quota in GetTenantQuotas().OrderBy(r => r.ActiveUsers).Where(r => r.Year == curQuota.Year && r.Year3 == curQuota.Year3))
            {
                if (quota.Id == curQuota.Id)
                    return prev;

                prev = quota;
            }
            return null;
        }

        public static int GetPrevUsersCount(TenantQuota quota)
        {
            var prevQuota = GetPrevQuota(quota);
            if (prevQuota == null || prevQuota.Trial)
                return 1;
            return prevQuota.ActiveUsers + 1;
        }

        public static int GetRightQuotaId()
        {
            var q = GetRightQuota();
            return q != null ? q.Id : 0;
        }

        public static TenantQuota GetRightQuota()
        {
            var usedSpace = TenantStatisticsProvider.GetUsedSize();
            var needUsersCount = TenantStatisticsProvider.GetUsersCount();
            var quotas = GetTenantQuotas();

            return quotas.OrderBy(q => q.ActiveUsers)
                         .ThenBy(q => q.Year)
                         .FirstOrDefault(q =>
                                         q.ActiveUsers > needUsersCount
                                         && q.MaxTotalSize > usedSpace
                                         && !q.Free
                                         && !q.Trial);
        }

        public static int GetRemainingCountUsers()
        {
            return GetTenantQuota().ActiveUsers - TenantStatisticsProvider.GetUsersCount();
        }

        public static bool UpdatedWithoutLicense
        {
            get
            {
                DateTime licenseDay;
                return CoreContext.Configuration.Standalone
                       && (licenseDay = GetCurrentTariff().LicenseDate.Date) < DateTime.Today
                       && licenseDay < LicenseReader.VersionReleaseDate;
            }
        }
    }
}