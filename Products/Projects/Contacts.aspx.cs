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


using System.Web;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Projects
{
    public partial class Contacts : BasePage
    {
        public bool CanLinkContact { get; set; }

        protected override bool CanRead
        {
            get
            {
                var crmEnabled = WebItemManager.Instance[WebItemManager.CRMProductID];

                return crmEnabled != null && !crmEnabled.IsDisabled() && ProjectSecurity.CanReadContacts(Project);
            }
        }

        protected override void PageLoad()
        {
            CanLinkContact = ProjectSecurity.CanLinkContact(Project);

            var button = "";

            if(CanLinkContact)
            {
                button = "<a class='link-with-entity link dotline'>" + ProjectsCommonResource.EmptyScreenContactsButton + "</a>";
            }

            var escNoContacts = new Studio.Controls.Common.EmptyScreenControl
            {
                Header = ProjectsCommonResource.EmptyScreenContasctsHeader,
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_persons.png", ProductEntryPoint.ID),
                Describe = ProjectsCommonResource.EmptyScreenContactsDescribe,
                ID = "escNoContacts",
                ButtonHTML = button,
                CssClass = "display-none"
            };
            emptyScreen.Controls.Add(escNoContacts);

            Page.Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.ModuleContacts);

            Master.RegisterCRMResources();
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath, "contacts.js");
        }
    }
}
