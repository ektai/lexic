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

using System;
using System.Collections.Generic;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class CurrencyRateDao : AbstractDao
    {
        public CurrencyRateDao(int tenantID)
            : base(tenantID)
        {
        }

        public virtual List<CurrencyRate> GetAll()
        {
            return Db.ExecuteList(GetSqlQuery(null)).ConvertAll(ToCurrencyRate);
        }

        public virtual CurrencyRate GetByID(int id)
        {
            var rates = Db.ExecuteList(GetSqlQuery(Exp.Eq("id", id))).ConvertAll(ToCurrencyRate);

            return rates.Count > 0 ? rates[0] : null;
        }

        public CurrencyRate GetByCurrencies(string fromCurrency, string toCurrency)
        {
            var rates = Db.ExecuteList(GetSqlQuery(Exp.Eq("from_currency", fromCurrency.ToUpper()) & Exp.Eq("to_currency", toCurrency.ToUpper())))
                .ConvertAll(ToCurrencyRate);
                
            return rates.Count > 0 ? rates[0] : null;
        }

        public int SaveOrUpdate(CurrencyRate currencyRate)
        {
            if (String.IsNullOrEmpty(currencyRate.FromCurrency) || String.IsNullOrEmpty(currencyRate.ToCurrency) || currencyRate.Rate < 0)
                throw new ArgumentException();

            if (currencyRate.ID > 0 && currencyRate.Rate == 0)
                return Delete(currencyRate.ID);

            if (Db.ExecuteScalar<int>(Query("crm_currency_rate").SelectCount().Where(Exp.Eq("id", currencyRate.ID))) == 0)
            {
                var query = Insert("crm_currency_rate")
                    .InColumnValue("id", 0)
                    .InColumnValue("from_currency", currencyRate.FromCurrency.ToUpper())
                    .InColumnValue("to_currency", currencyRate.ToCurrency.ToUpper())
                    .InColumnValue("rate", currencyRate.Rate)
                    .InColumnValue("create_by", SecurityContext.CurrentAccount.ID)
                    .InColumnValue("create_on", DateTime.UtcNow)
                    .InColumnValue("last_modifed_by", SecurityContext.CurrentAccount.ID)
                    .InColumnValue("last_modifed_on", DateTime.UtcNow)
                    .Identity(1, 0, true);

                currencyRate.ID = Db.ExecuteScalar<int>(query);
            }
            else
            {
                Db.ExecuteNonQuery(
                    Update("crm_currency_rate")
                        .Set("from_currency", currencyRate.FromCurrency.ToUpper())
                        .Set("to_currency", currencyRate.ToCurrency.ToUpper())
                        .Set("rate", currencyRate.Rate)
                        .Set("last_modifed_on", DateTime.UtcNow)
                        .Set("last_modifed_by", SecurityContext.CurrentAccount.ID)
                        .Where(Exp.Eq("id", currencyRate.ID)));
            }

            return currencyRate.ID;
        }

        public int Delete(int id)
        {
            if (id <= 0) throw new ArgumentException();

            var sqlQuery = Delete("crm_currency_rate")
                .Where(Exp.Eq("id", id));

            Db.ExecuteNonQuery(sqlQuery);

            return id;
        }

        public List<CurrencyRate> SetCurrencyRates(List<CurrencyRate> rates)
        {
            using (var tx = Db.BeginTransaction())
            {
                Db.ExecuteNonQuery(Delete("crm_currency_rate"));
                
                foreach (var rate in rates)
                {
                    var query = Insert("crm_currency_rate")
                        .InColumnValue("id", 0)
                        .InColumnValue("from_currency", rate.FromCurrency.ToUpper())
                        .InColumnValue("to_currency", rate.ToCurrency.ToUpper())
                        .InColumnValue("rate", rate.Rate)
                        .InColumnValue("create_by", SecurityContext.CurrentAccount.ID)
                        .InColumnValue("create_on", DateTime.UtcNow)
                        .InColumnValue("last_modifed_by", SecurityContext.CurrentAccount.ID)
                        .InColumnValue("last_modifed_on", DateTime.UtcNow)
                        .Identity(1, 0, true);

                    rate.ID = Db.ExecuteScalar<int>(query);
                }

                tx.Commit();

                return rates;
            }
        }

        private SqlQuery GetSqlQuery(Exp where)
        {
            var sqlQuery = Query("crm_currency_rate")
                .Select("id",
                        "from_currency",
                        "to_currency",
                        "rate");

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        private static CurrencyRate ToCurrencyRate(object[] row)
        {
            return new CurrencyRate{
                    ID = Convert.ToInt32(row[0]),
                    FromCurrency = Convert.ToString(row[1]),
                    ToCurrency = Convert.ToString(row[2]),
                    Rate = Convert.ToDecimal(row[3])
                };
        }
    }
}