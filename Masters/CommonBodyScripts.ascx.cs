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
using ASC.Web.Core.Client.Bundling;
using ASC.Web.Studio.Masters.MasterResources;

namespace ASC.Web.Studio.Masters
{
    public class CommonBodyScripts : ResourceScriptBundleControl, IStaticBundle
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetData(GetStaticJavaScript());
        }

        public ScriptBundleData GetStaticJavaScript()
        {
            return (ScriptBundleData)
                new ScriptBundleData("studio", "common")
                    .AddSource(ResolveUrl, new MasterTemplateResources())
                    .AddSource(ResolveUrl,
                        "~/js/asc/core/asc.customevents.js",
                        "~/js/asc/api/api.factory.js",
                        "~/js/asc/api/api.helper.js",
                        "~/js/asc/api/asc.teamlab.js",
                        "~/js/asc/plugins/jquery.base64.js",
                        "~/js/asc/plugins/jquery-customcombobox.js",
                        "~/js/asc/plugins/jquery-advansedfilter.js",
                        "~/js/asc/plugins/jquery-advansedselector.js",
                        "~/js/asc/plugins/jquery-useradvansedselector.js",
                        "~/js/asc/plugins/jquery-groupadvansedselector.js",
                        "~/js/asc/plugins/jquery-contactadvansedselector.js",
                        "~/js/asc/plugins/jquery-emailadvansedselector.js",
                        "~/js/asc/plugins/jquery.tlblock.js",
                        "~/js/asc/plugins/popupbox.js",
                        "~/js/asc/core/asc.files.utility.js",
                        "~/js/asc/core/basetemplate.master.init.js",
                        "~/UserControls/Common/HelpCenter/js/help-center.js",
                        "~/js/asc/core/groupselector.js",
                        "~/js/asc/core/asc.mail.utility.js",
                        "~/js/third-party/async.js",
                        "~/js/third-party/clipboard.js",
                        "~/js/asc/core/clipboard.js",
                        "~/js/third-party/email-addresses.js",
                        "~/js/third-party/punycode.js",
                        "~/js/third-party/purify.min.js",
                        "~/js/asc/core/asc.mailreader.js",
                        "~/UserControls/Common/VideoGuides/js/videoguides.js",
                        "~/js/asc/core/asc.feedreader.js",
                        "~/addons/talk/js/talk.navigationitem.js",
                        "~/UserControls/Common/InviteLink/js/invitelink.js",
                        "~/UserControls/Management/InvitePanel/js/invitepanel.js"
                        );
        }

        public StyleBundleData GetStaticStyleSheet()
        {
            return null;
        }
    }
}