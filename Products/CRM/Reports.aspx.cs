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


#region Import

using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Reports;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.UserControls.Common.LoaderPage;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM
{
    public partial class Reports : BasePage
    {
        protected override void PageLoad()
        {
            if (!Global.CanCreateReports)
                Response.Redirect(PathProvider.StartURL());

            Title = HeaderStringHelper.GetPageTitle(Master.CurrentPageCaption ?? CRMReportResource.Reports);

            loaderHolder.Controls.Add(LoadControl(LoaderPage.Location));

            AdditionalContainerHolder.Controls.Add(LoadControl(ReportsNavigation.Location));

            CommonContainerHolder.Controls.Add(LoadControl(ReportsView.Location));
        }
    }
}