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
using System.Web;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using Report = ASC.Web.Projects.Classes.Report;

namespace ASC.Web.Projects.Controls.Reports
{
    public partial class ReportTemplateView : BaseUserControl
    {
        public ReportTemplate Template { get; set; }

        public string TmplParamHour { get; set; }
        public int TmplParamMonth { get; set; }
        public int TmplParamWeek { get; set; }
        public string TmplParamPeriod { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Template = Page.EngineFactory.ReportEngine.GetTemplate(int.Parse(Request["tmplId"]));
            if (Template == null)
            {
                Page.RedirectNotFound("reports.aspx");
            }
            else
            {
                var filters = (ReportFilters)LoadControl(PathProvider.GetFileStaticRelativePath("Reports/ReportFilters.ascx"));
                filters.Report = Report.CreateNewReport(Template.ReportType, Template.Filter);
                _filter.Controls.Add(filters);
                InitTmplParam();

                Page.Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, HttpUtility.HtmlDecode(Template.Name)));
            }
        }

        private void InitTmplParam()
        {
            var cron = Template.Cron.Split(' ');

            try
            {
                TmplParamWeek = Int32.Parse(cron[5]);
                TmplParamPeriod = "week";
            }
            catch (FormatException)
            {
                try
                {
                    TmplParamMonth = Int32.Parse(cron[3]);
                    TmplParamPeriod = "month";
                }
                catch (FormatException)
                {
                    TmplParamPeriod = "day";
                }
            }

            TmplParamHour = cron[2];

        }
    }
}