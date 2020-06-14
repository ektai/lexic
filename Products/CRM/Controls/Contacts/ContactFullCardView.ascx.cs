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

using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;
using System;
using System.Linq;
using System.Text;
using System.Web;


#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    public partial class ContactFullCardView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/ContactFullCardView.ascx"); }
        }

        public Contact TargetContact { get; set; }

        public Contact TargetCompanyIfPerson { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            TargetCompanyIfPerson = TargetContact is Person && ((Person)TargetContact).CompanyID != 0 ?
                DaoFactory.ContactDao.GetByID(((Person)TargetContact).CompanyID) :
                null;

            RegisterClientScriptHelper.DataContactFullCardView(Page, TargetContact);

            ExecHistoryView();
            RegisterScript();
        }

        #endregion

        #region Methods

        protected void ExecHistoryView()
        {
            var historyViewControl = (HistoryView)LoadControl(HistoryView.Location);

            historyViewControl.TargetContactID = TargetContact.ID;
            historyViewControl.TargetEntityID = 0;
            historyViewControl.TargetEntityType = EntityType.Contact;

            _phHistoryView.Controls.Add(historyViewControl);
        }

        protected string GetMailingHistoryUrl()
        {
            var primaryEmail = DaoFactory.ContactInfoDao.GetList(TargetContact.ID, ContactInfoType.Email, null, true).FirstOrDefault();
            if (primaryEmail == null || string.IsNullOrEmpty(primaryEmail.Data))
            {
                return string.Empty;
            }

            var virtualPath = string.Format("addons/mail/#inbox/from={0}", HttpUtility.UrlEncode(primaryEmail.Data));
            return Studio.Utility.CommonLinkUtility.GetFullAbsolutePath(virtualPath);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();


            var additionalContactsCount = 0;
            if (TargetContact is Company)
            {
                var members = DaoFactory.ContactDao.GetMembersIDsAndShareType(TargetContact.ID);
                foreach (var m in members) {
                    if (CRMSecurity.CanAccessTo(m.Key, EntityType.Person, m.Value, 0)) {
                        additionalContactsCount++;
                    }
                }
            }
            else if (TargetCompanyIfPerson != null && CRMSecurity.CanAccessTo(TargetCompanyIfPerson))
            {
                additionalContactsCount = 1;
            }

            sb.AppendFormat(@"
                    ASC.CRM.ContactFullCardView.init({0},{1},{2},{3},{4},{5},""{6}"",{7},{8});",
                TargetContact.ID,
                (TargetContact is Company).ToString().ToLower(),
                TargetContact is Person ? ((Person)TargetContact).CompanyID : 0,
                Global.TenantSettings.ChangeContactStatusGroupAuto != null ? Global.TenantSettings.ChangeContactStatusGroupAuto.ToString().ToLower() : "null",
                Global.TenantSettings.AddTagToContactGroupAuto != null ? Global.TenantSettings.AddTagToContactGroupAuto.ToString().ToLower() : "null",
                Global.TenantSettings.WriteMailToHistoryAuto.ToString().ToLower(),
                Studio.Core.FileSizeComment.GetFileImageSizeNote(CRMContactResource.ContactPhotoInfo, true),
                additionalContactsCount,
                CRMSecurity.CanEdit(TargetContact).ToString().ToLower()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}