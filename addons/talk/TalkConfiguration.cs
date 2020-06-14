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


using ASC.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;
using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ASC.Web.Talk
{
    class TalkConfiguration
    {
        public string ServerAddress
        {
            get;
            private set;
        }

        public string UpdateInterval
        {
            get;
            private set;
        }

        public string OverdueInterval
        {
            get;
            private set;
        }

        public string ServerName
        {
            get;
            private set;
        }

        public string ServerPort
        {
            get;
            private set;
        }

        public string BoshUri
        {
            get;
            private set;
        }

        public string UserName
        {
            get;
            private set;
        }

        public string Jid
        {
            get;
            private set;
        }

        public string FileTransportType
        {
            get;
            private set;
        }

        public string RequestTransportType
        {
            get;
            private set;
        }

        public bool EnabledFirebugLite
        {
            get;
            private set;
        }

        public bool EnabledHistory
        {
            get;
            private set;
        }

        public bool EnabledConferences
        {
            get;
            private set;
        }

        public bool EnabledMassend
        {
            get;
            private set;
        }

        public String ValidSymbols
        {
            get;
            private set;
        }

        public String HistoryLength
        {
            get;
            private set;
        }

        public String ResourcePriority
        {
            get;
            private set;
        }

        public String ClientInactivity
        {
            get;
            private set;
        }

        // for migration from teamlab.com to lexic.xyz
        public bool ReplaceDomain
        {
            get;
            private set;
        }

        public String ReplaceFromDomain
        {
            get;
            private set;
        }

        public String ReplaceToDomain
        {
            get;
            private set;
        }

        public TalkConfiguration()
        {
            RequestTransportType = WebConfigurationManager.AppSettings["RequestTransportType"] ?? "flash";

            // for migration from teamlab.com to lexic.xyz
            var replaceSetting = WebConfigurationManager.AppSettings["jabber.replace-domain"];
            if (!string.IsNullOrEmpty(replaceSetting))
            {
                ReplaceDomain = true;
                var q = replaceSetting.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLowerInvariant());
                ReplaceFromDomain = q.ElementAt(0);
                ReplaceToDomain = q.ElementAt(1);
            }

            ServerAddress = new Uri(CommonLinkUtility.ServerRootPath).Host;
            ServerAddress = ReplaceToOldDomain(ServerAddress);
            ServerName = CoreContext.TenantManager.GetCurrentTenant().TenantDomain;
            ServerName = ReplaceToOldDomain(ServerName);
            ServerPort = WebConfigurationManager.AppSettings["JabberPort"] ?? "5222";
            BoshUri = WebConfigurationManager.AppSettings["BoshPath"] ?? "http://localhost:5280/http-poll/";
            if (RequestTransportType == "handler")
            {
                BoshUri = VirtualPathUtility.ToAbsolute(TalkAddon.BaseVirtualPath + "/http-poll/httppoll.ashx");
            }
            else
            {
                BoshUri = string.Format(BoshUri, ServerAddress);
            }

            try
            {
                UserName = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLowerInvariant();
            }
            catch
            {
                UserName = string.Empty;
            }
            Jid = string.Format("{0}@{1}", UserName, ServerName).ToLowerInvariant();
            FileTransportType = WebConfigurationManager.AppSettings["FileTransportType"] ?? "flash";
            // in seconds
            UpdateInterval = WebConfigurationManager.AppSettings["UpdateInterval"] ?? "3600";
            OverdueInterval = WebConfigurationManager.AppSettings["OverdueInterval"] ?? "60";

            EnabledHistory = (WebConfigurationManager.AppSettings["History"] ?? "on") == "on";
            EnabledMassend = (WebConfigurationManager.AppSettings["Massend"] ?? "on") == "on";
            EnabledConferences = (WebConfigurationManager.AppSettings["Conferences"] ?? "on") == "on";
            EnabledFirebugLite = (WebConfigurationManager.AppSettings["FirebugLite"] ?? "off") == "on";
            ValidSymbols = WebConfigurationManager.AppSettings["ValidSymbols"] ?? "äöüßña-žа-яё";
            HistoryLength = WebConfigurationManager.AppSettings["HistoryLength"] ?? "20";
            ResourcePriority = WebConfigurationManager.AppSettings["ResourcePriority"] ?? "60";
            ClientInactivity = WebConfigurationManager.AppSettings["ClientInactivity"] ?? "30";
        }


        private string ReplaceToOldDomain(string orig)
        {
            if (ReplaceDomain && orig != null && orig.EndsWith(ReplaceToDomain))
            {
                var place = orig.LastIndexOf(ReplaceToDomain);
                if (place >= 0)
                {
                    return orig.Remove(place, ReplaceToDomain.Length).Insert(place, ReplaceFromDomain);
                }
            }
            return orig;
        }
    }
}