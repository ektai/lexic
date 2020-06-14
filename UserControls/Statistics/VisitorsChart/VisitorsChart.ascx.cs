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
using System.Text;
using System.Web.UI;
using ASC.Web.Studio.Utility;
using System.Web;
using Resources;

namespace ASC.Web.Studio.UserControls.Statistics
{
    [ManagementControl(ManagementType.Statistic, Location)]
    public partial class VisitorsChart : UserControl
    {
        public const string Location = "~/UserControls/Statistics/VisitorsChart/VisitorsChart.ascx";

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Page.RegisterStyle("~/UserControls/Statistics/VisitorsChart/css/visitorschart_style.less")
                .RegisterBodyScripts("~/UserControls/Statistics/VisitorsChart/js/excanvas.min.js",
                "~/UserControls/Statistics/VisitorsChart/js/jquery.flot.js",
                "~/UserControls/Statistics/VisitorsChart/js/tooltip.js",
                "~/UserControls/Statistics/VisitorsChart/js/visitorschart.js");

            var jsResources = new StringBuilder();
            jsResources.Append("jq(document).ready(function(){");
            jsResources.Append("if(typeof window.ASC==='undefined')window.ASC={};");
            jsResources.Append("if(typeof window.ASC.Resources==='undefined')window.ASC.Resources={};");
            jsResources.Append("window.ASC.Resources.chartDateFormat='" + Resource.ChartDateFormat + "';");
            jsResources.Append("window.ASC.Resources.chartMonthNames='" + Resource.ChartMonthNames + "';");
            jsResources.Append("window.ASC.Resources.hitLabel='" + Resource.VisitorsChartHitLabel + "';");
            jsResources.Append("window.ASC.Resources.hostLabel='" + Resource.VisitorsChartHostLabel + "';");
            jsResources.Append("window.ASC.Resources.visitsLabel='" + Resource.VisitorsChartVisitsLabel + "';");
            jsResources.Append("});");

            Page.RegisterInlineScript(jsResources.ToString());
        }
    }
}