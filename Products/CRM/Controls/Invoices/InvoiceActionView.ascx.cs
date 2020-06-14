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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using ASC.CRM.Core.Entities;
using ASC.Core;
using ASC.MessagingSystem;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.CRM.Core;
using ASC.Web.Studio.Core;
using System.Web;
using ASC.Common.Logging;
using ASC.CRM.Core.Dao;
using ASC.Web.CRM.Core;
using ASC.Web.Studio.Utility;
using Autofac;
using Newtonsoft.Json;

namespace ASC.Web.CRM.Controls.Invoices
{
    public partial class InvoiceActionView : BaseUserControl
    {
        #region Properies

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Invoices/InvoiceActionView.ascx"); } }

        public InvoiceActionType ActionType { get; set; }

        public Invoice TargetInvoice { get; set; }

        private const string ErrorCookieKey = "save_invoice_error";

        protected string InvoicesNumber
        {
            get
            {
                var number = ActionType == InvoiceActionType.Edit ? TargetInvoice.Number : DaoFactory.InvoiceDao.GetNewInvoicesNumber();
                return number.HtmlEncode();
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            bool havePermission;

            if (ActionType == InvoiceActionType.Duplicate)
            {
                havePermission = TargetInvoice != null && CRMSecurity.CanAccessTo(TargetInvoice);
            }
            else
            {
                havePermission = TargetInvoice == null || CRMSecurity.CanEdit(TargetInvoice);
            }

            if (!havePermission)
            {
                Response.Redirect(PathProvider.StartURL() + "invoices.aspx");
            }

            RegisterClientScriptHelper.DataInvoicesActionView(Page, TargetInvoice);

            InitActionButtons();

            RegisterScript();
        }

        protected void SaveOrUpdateInvoice(Object sender, CommandEventArgs e)
        {
            try
            {
                using (var scope = DIHelper.Resolve())
                {
                    var dao = scope.Resolve<DaoFactory>();

                    var invoice = GetInvoice(dao);

                    var billingAddressID = Convert.ToInt32(Request["billingAddressID"]);
                    var deliveryAddressID = Convert.ToInt32(Request["deliveryAddressID"]);

                    var messageAction = MessageAction.InvoiceCreated;

                    if (invoice.ID > 0)
                    {
                        messageAction = MessageAction.InvoiceUpdated;
                        RemoveInvoiceFile(invoice.FileID, dao);
                    }

                    invoice.ID = dao.InvoiceDao.SaveOrUpdateInvoice(invoice);
                    MessageService.Send(HttpContext.Current.Request, messageAction, MessageTarget.Create(invoice.ID),
                        invoice.Number);

                    var invoiceLines = GetInvoiceLines(dao);

                    foreach (var line in invoiceLines)
                    {
                        line.InvoiceID = invoice.ID;
                        line.ID = dao.InvoiceLineDao.SaveOrUpdateInvoiceLine(line);
                    }

                    RemoveUnusedLines(invoice.ID, invoiceLines, dao);

                    dao.InvoiceDao.UpdateInvoiceJsonData(invoice, billingAddressID, deliveryAddressID);

                    if (Global.CanDownloadInvoices)
                    {
                        PdfQueueWorker.StartTask(HttpContext.Current, TenantProvider.CurrentTenantID, SecurityContext.CurrentAccount.ID, invoice.ID);
                    }

                    string redirectUrl;
                    if (ActionType == InvoiceActionType.Create && UrlParameters.ContactID != 0)
                    {
                        redirectUrl =
                            string.Format(
                                e.CommandArgument.ToString() == "1"
                                    ? "invoices.aspx?action=create&contactID={0}"
                                    : "default.aspx?id={0}#invoices", UrlParameters.ContactID);
                    }
                    else
                    {
                        redirectUrl = e.CommandArgument.ToString() == "1"
                            ? "invoices.aspx?action=create"
                            : string.Format("invoices.aspx?id={0}", invoice.ID);
                    }

                    Response.Redirect(redirectUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                if (!(ex is InvoiceValidationException))
                    LogManager.GetLogger("ASC.CRM").Error(ex);

                var cookie = HttpContext.Current.Request.Cookies.Get(ErrorCookieKey);
                if (cookie == null)
                {
                    cookie = new HttpCookie(ErrorCookieKey)
                        {
                            Value = ex.Message
                        };
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        #endregion

        #region Methods

        private void InitActionButtons()
        {
            saveButton.OnClientClick = String.Format("return ASC.CRM.InvoiceActionView.submitForm('{0}');", saveButton.UniqueID);
            saveAndCreateNewButton.OnClientClick = String.Format("return ASC.CRM.InvoiceActionView.submitForm('{0}');", saveAndCreateNewButton.UniqueID);

            if (ActionType == InvoiceActionType.Create)
            {
                saveButton.Text = CRMInvoiceResource.AddThisInvoiceButton;
                saveAndCreateNewButton.Text = CRMInvoiceResource.AddAndCreateNewInvoiceButton;
                cancelButton.Attributes.Add("href",
                                            Request.UrlReferrer != null && Request.Url != null && String.Compare(Request.UrlReferrer.PathAndQuery, Request.Url.PathAndQuery) != 0
                                                ? Request.UrlReferrer.OriginalString
                                                : "invoices.aspx");
            }

            if (ActionType == InvoiceActionType.Edit)
            {
                saveButton.Text = CRMCommonResource.SaveChanges;
                cancelButton.Attributes.Add("href", String.Format("invoices.aspx?id={0}", TargetInvoice.ID));
            }

            if (ActionType == InvoiceActionType.Duplicate)
            {
                saveButton.Text = CRMInvoiceResource.DuplicateInvoiceButton;
                saveAndCreateNewButton.Text = CRMInvoiceResource.DuplicateAndCreateNewInvoiceButton;
                cancelButton.Attributes.Add("href", String.Format("invoices.aspx?id={0}", TargetInvoice.ID));
            }
        }

        private void RegisterScript()
        {
            Page.RegisterClientScript(new Masters.ClientScripts.ExchangeRateViewData());

            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.InvoiceActionView.init({0}, ""{1}"");",
                (int)ContactSelectorTypeEnum.All,
                ErrorCookieKey
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        private Invoice GetInvoice(DaoFactory dao)
        {
            var invoice = new Invoice();

            if (ActionType == InvoiceActionType.Edit)
            {
                invoice.ID = TargetInvoice.ID;
                invoice.Number = TargetInvoice.Number;
                invoice.FileID = TargetInvoice.FileID;
            }
            else
            {
                invoice.Number = Request["invoiceNumber"];
                if (dao.InvoiceDao.IsExist(invoice.Number))
                    throw new InvoiceValidationException(CRMErrorsResource.InvoiceNumberBusy);
            }

            DateTime issueDate;
            if (!DateTime.TryParse(Request["invoiceIssueDate"], out issueDate))
                throw new InvoiceValidationException("invalid issueDate");
            invoice.IssueDate = issueDate;

            invoice.ContactID = Convert.ToInt32(Request["invoiceContactID"]);
            if (invoice.ContactID <= 0) throw new InvoiceValidationException(CRMErrorsResource.InvoiceContactNotFound);
            var contact = dao.ContactDao.GetByID(invoice.ContactID);
            if (contact == null || !CRMSecurity.CanAccessTo(contact))
            {
                throw new InvoiceValidationException(CRMErrorsResource.InvoiceContactNotFound);
            }

            invoice.ConsigneeID = Convert.ToInt32(Request["invoiceConsigneeID"]);
            if (invoice.ConsigneeID > 0)
            {
                var consignee = dao.ContactDao.GetByID(invoice.ConsigneeID);
                if (consignee == null || !CRMSecurity.CanAccessTo(consignee))
                {
                    throw new InvoiceValidationException(CRMErrorsResource.InvoiceConsigneeNotFound);
                }
            }
            else
            {
                invoice.ConsigneeID = 0;
            }


            invoice.EntityType = EntityType.Opportunity;

            invoice.EntityID = Convert.ToInt32(Request["invoiceOpportunityID"]);
            if (invoice.EntityID > 0)
            {
                var deal = dao.DealDao.GetByID(invoice.EntityID);
                if (deal == null || !CRMSecurity.CanAccessTo(deal))
                    throw new InvoiceValidationException(CRMErrorsResource.DealNotFound);

                var dealMembers = dao.DealDao.GetMembers(invoice.EntityID);
                if (!dealMembers.Contains(invoice.ContactID))
                    throw new InvoiceValidationException("contact doesn't have this opportunity");
            }

            DateTime dueDate;
            if (!DateTime.TryParse(Request["invoiceDueDate"], out dueDate))
                throw new InvoiceValidationException(CRMErrorsResource.InvoiceDueDateInvalid);
            if (issueDate > dueDate)
                throw new InvoiceValidationException(CRMErrorsResource.InvoiceIssueMoreThanDue);
            invoice.DueDate = dueDate;

            invoice.Language = Request["invoiceLanguage"];
            if (string.IsNullOrEmpty(invoice.Language) || SetupInfo.EnabledCultures.All(c => c.Name != invoice.Language))
                throw new InvoiceValidationException(CRMErrorsResource.LanguageNotFound);

            invoice.Currency = Request["invoiceCurrency"];
            if (string.IsNullOrEmpty(invoice.Currency))
            {
                throw new InvoiceValidationException(CRMErrorsResource.CurrencyNotFound);
            }
            else
            {
                invoice.Currency = invoice.Currency.ToUpper();
                if (CurrencyProvider.Get(invoice.Currency) == null)
                {
                    throw new InvoiceValidationException(CRMErrorsResource.CurrencyNotFound);
                }
            }

            invoice.ExchangeRate = Convert.ToDecimal(Request["invoiceExchangeRate"], new CultureInfo("en-US"));
            if (invoice.ExchangeRate <= 0)
                throw new InvoiceValidationException(CRMErrorsResource.ExchangeRateNotSet);

            invoice.PurchaseOrderNumber = Request["invoicePurchaseOrderNumber"];

            invoice.Terms = Request["invoiceTerms"];
            if (string.IsNullOrEmpty(invoice.Terms))
                throw new InvoiceValidationException(CRMErrorsResource.InvoiceTermsNotFound);

            invoice.Description = Request["invoiceDescription"];

            invoice.Status = InvoiceStatus.Draft;

            invoice.TemplateType = InvoiceTemplateType.Eur;

            return invoice;
        }

        private List<InvoiceLine> GetInvoiceLines(DaoFactory dao)
        {
            var invoiceLines = new List<InvoiceLine>();

            if (!Request.Form.AllKeys.Any(x => x.StartsWith("iLineItem_")))
                throw new InvoiceValidationException(CRMErrorsResource.InvoiceItemsListEmpty);

            foreach (var customField in Request.Form.AllKeys)
            {
                if (!customField.StartsWith("iLineItem_")) continue;

                var id = Convert.ToInt32(customField.Split('_')[1]);
                var sortOrder = Convert.ToInt32(customField.Split('_')[2]);

                var invoiceItemID = Convert.ToInt32(Request["iLineItem_" + id + "_" + sortOrder]);
                var invoiceTax1ID = Convert.ToInt32(Request["iLineTax1_" + id + "_" + sortOrder]);
                var invoiceTax2ID = Convert.ToInt32(Request["iLineTax2_" + id + "_" + sortOrder]);

                if (!dao.InvoiceItemDao.IsExist(invoiceItemID))
                    throw new InvoiceValidationException(CRMErrorsResource.InvoiceItemNotFound);

                if (invoiceTax1ID > 0 && !dao.InvoiceTaxDao.IsExist(invoiceTax1ID))
                    throw new InvoiceValidationException(CRMErrorsResource.InvoiceTaxNotFound);

                if (invoiceTax2ID > 0 && !dao.InvoiceTaxDao.IsExist(invoiceTax2ID))
                    throw new InvoiceValidationException(CRMErrorsResource.InvoiceTaxNotFound);

                var line = new InvoiceLine
                {
                    ID = id,
                    InvoiceItemID = invoiceItemID,
                    InvoiceTax1ID = invoiceTax1ID,
                    InvoiceTax2ID = invoiceTax2ID,
                    Description = Request["iLineDescription_" + id + "_" + sortOrder],
                    Quantity = Convert.ToInt32(Request["iLineQuantity_" + id + "_" + sortOrder]),
                    Price = Convert.ToDecimal(Request["iLinePrice_" + id + "_" + sortOrder], new CultureInfo("en-US")),
                    Discount = Convert.ToInt32(Request["iLineDiscount_" + id + "_" + sortOrder]),
                    SortOrder = sortOrder
                };

                invoiceLines.Add(line);
            }

            return invoiceLines;
        }

        private void RemoveUnusedLines(int invoiceID, List<InvoiceLine> lines, DaoFactory dao)
        {
            var oldLines = dao.InvoiceLineDao.GetInvoiceLines(invoiceID);

            foreach (var line in oldLines)
            {
                var contains = lines.Any(x => x.ID == line.ID);
                if (!contains)
                {
                    dao.InvoiceLineDao.DeleteInvoiceLine(line.ID);
                }
            }
        }

        private void RemoveInvoiceFile(int fileID, DaoFactory dao)
        {
            var events = dao.FileDao.GetEventsByFile(fileID);
            foreach (var eventId in events)
            {
                var item = dao.RelationshipEventDao.GetByID(eventId);
                if (item != null && item.CategoryID == (int)HistoryCategorySystem.FilesUpload && dao.RelationshipEventDao.GetFiles(item.ID).Count == 1)
                    dao.RelationshipEventDao.DeleteItem(item);
            }
        }

        #endregion

        #region InvoiceValidationException

        private class InvoiceValidationException : Exception
        {
            public InvoiceValidationException(string message) : base(message)
            {
            }
        }

        #endregion
    }
}