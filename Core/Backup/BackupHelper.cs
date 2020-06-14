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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Web.Studio.Core.Backup
{
    public class BackupHelper
    {
        public const long AvailableZipSize = 10 * 1024 * 1024 * 1024L;
        private static readonly Guid mailStorageTag = new Guid("666ceac1-4532-4f8c-9cba-8f510eca2fd1");

        public static BackupAvailableSize GetAvailableSize(int tenantId)
        {
            if (CoreContext.Configuration.Standalone)
                return BackupAvailableSize.Available;

            var size = CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(tenantId))
                      .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty && !new Guid(r.Tag).Equals(mailStorageTag))
                      .Sum(r => r.Counter);
            if (size > AvailableZipSize)
            {
                return BackupAvailableSize.NotAvailable;
            }

            size = TenantStatisticsProvider.GetUsedSize(tenantId);
            if (size > AvailableZipSize)
            {
                return BackupAvailableSize.WithoutMail;
            }

            return BackupAvailableSize.Available;
        }

        public static bool ExceedsMaxAvailableSize(int tenantId)
        {
            return GetAvailableSize(tenantId) != BackupAvailableSize.Available;
        }
    }

    public enum BackupAvailableSize
    {
        Available,
        WithoutMail,
        NotAvailable,
    }
}