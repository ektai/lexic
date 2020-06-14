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
using System.Web.Configuration;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Files.Classes;

namespace ASC.Web.Files.Helpers
{
    public static class ThirdpartyConfiguration
    {
        public static IEnumerable<string> ThirdPartyProviders
        {
            get { return (WebConfigurationManager.AppSettings["files.thirdparty.enable"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries); }
        }

        public static bool SupportInclusion
        {
            get
            {
                using (var providerDao = Global.DaoFactory.GetProviderDao())
                {
                    if (providerDao == null) return false;
                }

                return SupportBoxInclusion || SupportDropboxInclusion || SupportDocuSignInclusion || SupportGoogleDriveInclusion || SupportOneDriveInclusion || SupportSharePointInclusion || SupportWebDavInclusion || SupportNextcloudInclusion || SupportOwncloudInclusion || SupportYandexInclusion;
            }
        }

        public static bool SupportBoxInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("box") && BoxLoginProvider.Instance.IsEnabled;
            }
        }

        public static bool SupportDropboxInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("dropboxv2") && DropboxLoginProvider.Instance.IsEnabled;
            }
        }

        public static bool SupportOneDriveInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("onedrive") && OneDriveLoginProvider.Instance.IsEnabled;
            }
        }

        public static bool SupportSharePointInclusion
        {
            get { return ThirdPartyProviders.Contains("sharepoint"); }
        }

        public static bool SupportWebDavInclusion
        {
            get { return ThirdPartyProviders.Contains("webdav"); }
        }

        public static bool SupportNextcloudInclusion
        {
            get { return ThirdPartyProviders.Contains("nextcloud"); }
        }

        public static bool SupportOwncloudInclusion
        {
            get { return ThirdPartyProviders.Contains("owncloud"); }
        }

        public static bool SupportYandexInclusion
        {
            get { return ThirdPartyProviders.Contains("yandex"); }
        }

        public static string DropboxAppKey
        {
            get { return DropboxLoginProvider.Instance["dropboxappkey"]; }
        }

        public static string DropboxAppSecret
        {
            get { return DropboxLoginProvider.Instance["dropboxappsecret"]; }
        }

        public static bool SupportDocuSignInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("docusign") && DocuSignLoginProvider.Instance.IsEnabled;
            }
        }

        public static bool SupportGoogleDriveInclusion
        {
            get
            {
                return ThirdPartyProviders.Contains("google") && GoogleLoginProvider.Instance.IsEnabled;
            }
        }
    }
}