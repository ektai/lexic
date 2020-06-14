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
using System.Web.UI;
using ASC.Web.Core.Client.Bundling;
using ASC.Web.People.Masters.ClientScripts;
using ASC.Web.People.UserControls;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.People.Masters
{
    public partial class PeopleBaseTemplate : MasterPage, IStaticBundle
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitScripts();

            _sidepanelHolder.Controls.Add(LoadControl(SideNavigationPanel.Location));

            //UserMaker.AddOnlyOne(Page, ControlHolder);
            //ControlHolder.Controls.Add(new ImportUsersWebControl());
            ControlHolder.Controls.Add(LoadControl(ResendInvitesControl.Location));

            Master
                .AddClientScript(
                    new ClientSettingsResources(),
                    new ClientCustomResources(),
                    new ClientLocalizationResources());
        }

        private void InitScripts()
        {
            Master
                .AddStaticStyles(GetStaticStyleSheet())
                .AddStaticBodyScripts(GetStaticJavaScript())
                .RegisterInlineScript(
                    "jQuery(document.body).children('form').bind('submit', function() { return false; });");
        }

        public ScriptBundleData GetStaticJavaScript()
        {
            return (ScriptBundleData)
                new ScriptBundleData("people", "people")
                    .AddSource(ResolveUrl, new ClientTemplateResources())
                    .AddSource(ResolveUrl,
                        "~/Products/People/js/peoplemanager.js",
                        "~/Products/People/js/filterhandler.js",
                        "~/Products/People/js/navigatorhandler.js",
                        "~/Products/People/js/peoplecontroller.js",
                        "~/Products/People/js/peoplecore.js",
                        "~/Products/People/js/departmentmanagement.js",
                        "~/Products/People/js/peopleactions.js",
                        "~/Products/People/js/reassigns.js",
                        "~/Products/People/js/sidenavigationpanel.js");
        }

        public StyleBundleData GetStaticStyleSheet()
        {
            return (StyleBundleData)
                new StyleBundleData("people", "people")
                    .AddSource(ResolveUrl,
                        "~/Products/People/App_Themes/default/css/people.master.less");
        }
    }
}