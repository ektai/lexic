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


using System.Linq;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Web.Mail.Resources;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using ASC.Data.Storage;

namespace ASC.Web.Mail
{
    [WebZoneAttribute(WebZoneType.CustomProductList)]
    public class MailAddon : IAddon, IRenderCustomNavigation
    {
        public static Guid AddonID
        {
            get { return WebItemManager.MailProductID; }
        }

        public static string BaseVirtualPath
        {
            get { return "~/addons/mail/"; }
        }

        private AddonContext _context;

        public bool Visible { get { return true; } }

        public AddonContext Context
        {
            get { return _context; }
        }

        WebItemContext IWebItem.Context
        {
            get { return _context; }
        }

        public string Description
        {
            get { return MailResource.MailDescription; }
        }

        public Guid ID
        {
            get { return AddonID; }
        }

        public void Init()
        {
            _context = new AddonContext
                {
                    DisabledIconFileName = "mail_disabled.png",
                    IconFileName = "mail.png",
                    LargeIconFileName = "product_logolarge.svg",
                    SpaceUsageStatManager = new Configuration.MailSpaceUsageStatManager(),
                    AdminOpportunities = () => MailResource.AddonAdminOpportunities.Split('|').ToList(),
                    UserOpportunities = () => MailResource.AddonUserOpportunities.Split('|').ToList(),
                };
        }

        public string Name
        {
            get { return MailResource.ProductName; }
        }

        public void Shutdown()
        {

        }

        public string StartURL
        {
            get { return BaseVirtualPath; }
        }

        public string HelpURL
        {
            get { return BaseVirtualPath; }
        }

        public string ProductClassName
        {
            get { return "mail"; }
        }

        public string HubUrl
        {
            get { return WebConfigurationManager.AppSettings["web.hub"] ?? string.Empty; }
        }

        #region IRenderCustomNavigation Members
        
        public string RenderCustomNavigation(Page page)
        {
            var updateMailCounters = string.Empty;

            if (!page.AppRelativeTemplateSourceDirectory.Contains(BaseVirtualPath) && HubUrl == string.Empty)
            {
                updateMailCounters = string.Format("\r\nStudioManager.addPendingRequest(Teamlab.getMailFolders);");
            }

           // Migrate to CommonBodyScripts.ascx.cs
           // page.RegisterBodyScripts("~/js/asc/core/asc.mailreader.js");

            if (!string.IsNullOrEmpty(updateMailCounters))
            {
                page.RegisterInlineScript(updateMailCounters);
            }

            return string.Format(@"<li class=""top-item-box mail"">
                                     <a class=""inner-text mailActiveBox"" href=""{0}"" title=""{1}"">
                                       <svg><use base=""{2}"" href=""/skins/default/images/svg/top-studio-menu.svg#svgTopStudioMenumail""></use></svg>
                                       <span id=""TPUnreadMessagesCount"" class=""inner-label""></span>
                                     </a>
                                   </li>",
                                 VirtualPathUtility.ToAbsolute(BaseVirtualPath + "/"),
                                 MailResource.MailTitle,
                                 WebPath.GetPath("/"));
        }

        public Control LoadCustomNavigationControl(Page page)
        {
            return null;
        }

        #endregion
    }
}