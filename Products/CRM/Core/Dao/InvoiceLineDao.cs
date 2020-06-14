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
using ASC.Collections;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.CRM.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ASC.CRM.Core.Dao
{
    public class CachedInvoiceLineDao : InvoiceLineDao
    {
        private readonly HttpRequestDictionary<InvoiceLine> _invoiceLineCache = new HttpRequestDictionary<InvoiceLine>("crm_invoice_line");

        public CachedInvoiceLineDao(int tenantID)
            : base(tenantID)
        {
        }

        public override InvoiceLine GetByID(int invoiceLineID)
        {
            return _invoiceLineCache.Get(invoiceLineID.ToString(CultureInfo.InvariantCulture), () => GetByIDBase(invoiceLineID));
        }

        private InvoiceLine GetByIDBase(int invoiceLineID)
        {
            return base.GetByID(invoiceLineID);
        }

        public override int SaveOrUpdateInvoiceLine(InvoiceLine invoiceLine)
        {
            if (invoiceLine != null && invoiceLine.ID > 0)
                ResetCache(invoiceLine.ID);

            return base.SaveOrUpdateInvoiceLine(invoiceLine);
        }

        public override void DeleteInvoiceLine(int invoiceLineID)
        {
            ResetCache(invoiceLineID);

            base.DeleteInvoiceLine(invoiceLineID);
        }

        private void ResetCache(int invoiceLineID)
        {
            _invoiceLineCache.Reset(invoiceLineID.ToString(CultureInfo.InvariantCulture));
        }
    }
    
    public class InvoiceLineDao : AbstractDao
    {
        public InvoiceLineDao(int tenantID)
            : base(tenantID)
        {
        }


        public static string GetJson(InvoiceItem invoiceItem) {
            return invoiceItem == null ?
                    string.Empty :
                    JsonConvert.SerializeObject(new
                    {
                        id = invoiceItem.ID,
                        title = invoiceItem.Title,
                        description = invoiceItem.Description
                    });
        }
        public static string GetJson(InvoiceTax invoiceTax) {
            return invoiceTax == null ?
                    string.Empty :
                    JsonConvert.SerializeObject(new
                    {
                        id = invoiceTax.ID,
                        name = invoiceTax.Name,
                        rate = invoiceTax.Rate,
                        description = invoiceTax.Description
                    });
        }

        #region Get

        public virtual List<InvoiceLine> GetAll()
        {
            return Db.ExecuteList(GetInvoiceLineSqlQuery(null)).ConvertAll(ToInvoiceLine);
        }

        public virtual List<InvoiceLine> GetByID(int[] ids)
        {
            return Db.ExecuteList(GetInvoiceLineSqlQuery(Exp.In("id", ids))).ConvertAll(ToInvoiceLine);
        }

        public virtual InvoiceLine GetByID(int id)
        {
            var invoiceLines = Db.ExecuteList(GetInvoiceLineSqlQuery(Exp.Eq("id", id))).ConvertAll(ToInvoiceLine);

            return invoiceLines.Count > 0 ? invoiceLines[0] : null;
        }
        
        public List<InvoiceLine> GetInvoiceLines(int invoiceID)
        {
            return GetInvoiceLinesInDb(invoiceID);
        }

        public List<InvoiceLine> GetInvoiceLinesInDb(int invoiceID)
        {
            return Db.ExecuteList(GetInvoiceLineSqlQuery(Exp.Eq("invoice_id", invoiceID)).OrderBy("sort_order", true)).ConvertAll(ToInvoiceLine);
        }

        #endregion


        #region SaveOrUpdate

        public virtual int SaveOrUpdateInvoiceLine(InvoiceLine invoiceLine)
        {
            _cache.Remove(new Regex(TenantID.ToString(CultureInfo.InvariantCulture) + "invoice.*"));

            return SaveOrUpdateInvoiceLineInDb(invoiceLine);
        }

        private int SaveOrUpdateInvoiceLineInDb(InvoiceLine invoiceLine)
        {
            if (invoiceLine.InvoiceID <= 0 || invoiceLine.InvoiceItemID <= 0)
                throw new ArgumentException();

            if (String.IsNullOrEmpty(invoiceLine.Description))
            {
                invoiceLine.Description = String.Empty;
            }

            if (Db.ExecuteScalar<int>(Query("crm_invoice_line").SelectCount().Where(Exp.Eq("id", invoiceLine.ID))) == 0)
            {
                invoiceLine.ID = Db.ExecuteScalar<int>(
                               Insert("crm_invoice_line")
                              .InColumnValue("id", 0)
                              .InColumnValue("invoice_id", invoiceLine.InvoiceID)
                              .InColumnValue("invoice_item_id", invoiceLine.InvoiceItemID)
                              .InColumnValue("invoice_tax1_id", invoiceLine.InvoiceTax1ID)
                              .InColumnValue("invoice_tax2_id", invoiceLine.InvoiceTax2ID)
                              .InColumnValue("sort_order", invoiceLine.SortOrder)
                              .InColumnValue("description", invoiceLine.Description)
                              .InColumnValue("quantity", invoiceLine.Quantity)
                              .InColumnValue("price", invoiceLine.Price)
                              .InColumnValue("discount", invoiceLine.Discount)
                              .Identity(1, 0, true));
            }
            else
            {

                Db.ExecuteNonQuery(
                    Update("crm_invoice_line")
                        .Set("invoice_id", invoiceLine.InvoiceID)
                        .Set("invoice_item_id", invoiceLine.InvoiceItemID)
                        .Set("invoice_tax1_id", invoiceLine.InvoiceTax1ID)
                        .Set("invoice_tax2_id", invoiceLine.InvoiceTax2ID)
                        .Set("sort_order", invoiceLine.SortOrder)
                        .Set("description", invoiceLine.Description)
                        .Set("quantity", invoiceLine.Quantity)
                        .Set("price", invoiceLine.Price)
                        .Set("discount", invoiceLine.Discount)
                        .Where(Exp.Eq("id", invoiceLine.ID)));
            }
            return invoiceLine.ID;
        }

        #endregion


        #region Delete

        public virtual void DeleteInvoiceLine(int invoiceLineID)
        {
            var invoiceLine = GetByID(invoiceLineID);

            if (invoiceLine == null) return;

            Db.ExecuteNonQuery(Delete("crm_invoice_line").Where("id", invoiceLineID));

            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceLineCacheKey, String.Empty);*/
        }

        public void DeleteInvoiceLines(int invoiceID)
        {
            Db.ExecuteNonQuery(Delete("crm_invoice_line").Where(Exp.Eq("invoice_id", invoiceID)));

            /*_cache.Remove(_invoiceItemCacheKey);
            _cache.Insert(_invoiceLineCacheKey, String.Empty);*/
        }

        public Boolean CanDelete(int invoiceLineID)
        {
            return CanDeleteInDb(invoiceLineID);
        }

        public Boolean CanDeleteInDb(int invoiceLineID)
        {

                var invoiceID = Db.ExecuteScalar<int>(Query("crm_invoice_line").Select("invoice_id")
                                     .Where(Exp.Eq("id", invoiceLineID)));

                if (invoiceID == 0) return false;

                var count = Db.ExecuteScalar<int>(Query("crm_invoice_line").SelectCount()
                                        .Where(Exp.Eq("invoice_id", invoiceID)));

                return count > 1;
        }

        #endregion


        #region Private Methods

        private static InvoiceLine ToInvoiceLine(object[] row)
        {
            return new InvoiceLine
                {
                    ID = Convert.ToInt32(row[0]),
                    InvoiceID = Convert.ToInt32(row[1]),
                    InvoiceItemID = Convert.ToInt32(row[2]),
                    InvoiceTax1ID = Convert.ToInt32(row[3]),
                    InvoiceTax2ID = Convert.ToInt32(row[4]),
                    SortOrder = Convert.ToInt32(row[5]),
                    Description = Convert.ToString(row[6]),
                    Quantity = Convert.ToInt32(row[7]),
                    Price = Convert.ToDecimal(row[8]),
                    Discount = Convert.ToInt32(row[9])
                };
        }

        private SqlQuery GetInvoiceLineSqlQuery(Exp where)
        {
            var sqlQuery = Query("crm_invoice_line")
                .Select(
                    "id",
                    "invoice_id",
                    "invoice_item_id",
                    "invoice_tax1_id",
                    "invoice_tax2_id",
                    "sort_order",
                    "description",
                    "quantity",
                    "price",
                    "discount");

            if (where != null)
            {
                sqlQuery.Where(where);
            }

            return sqlQuery;
        }

        #endregion
    }
}