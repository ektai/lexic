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
using ASC.CRM.Core;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Classes;
using System.Text;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;

namespace ASC.Web.CRM.Controls.Cases
{
    public partial class CasesDetailsView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Cases/CasesDetailsView.ascx"); }
        }

        public ASC.CRM.Core.Entities.Cases TargetCase { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            ExecFullCardView();
            ExecPeopleInCaseView();
            ExecFilesView();
            RegisterScript();
        }

        #endregion

        #region Methods

        public void ExecPeopleInCaseView()
        {
            RegisterClientScriptHelper.DataListContactTab(Page, TargetCase.ID, EntityType.Case);
        }

        protected void ExecFullCardView()
        {
            var dealFullCardControl = (CasesFullCardView)LoadControl(CasesFullCardView.Location);
            dealFullCardControl.TargetCase = TargetCase;
            _phProfileView.Controls.Add(dealFullCardControl);
        }

        public void ExecFilesView()
        {
            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "case";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);
        }

        private void RegisterScript()
        {
            Page.RegisterInlineScript(@"ASC.CRM.CasesDetailsView.init();");
        }

        #endregion
    }
}