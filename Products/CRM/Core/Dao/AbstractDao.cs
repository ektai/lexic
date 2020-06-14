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
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Caching;
using ASC.Common.Logging;

namespace ASC.CRM.Core.Dao
{
    public class AbstractDao
    {
        protected readonly List<EntityType> _supportedEntityType = new List<EntityType>();
        protected readonly ILog _log = LogManager.GetLogger("ASC.CRM");

        protected readonly ICache _cache = AscCache.Default;
        public IDbManager Db { get; set; }
        /*
        protected readonly String _invoiceItemCacheKey;
        protected readonly String _invoiceTaxCacheKey;
        protected readonly String _invoiceLineCacheKey;
        */
        protected AbstractDao(int tenantID)
        {
            TenantID = tenantID;

            _supportedEntityType.Add(EntityType.Company);
            _supportedEntityType.Add(EntityType.Person);
            _supportedEntityType.Add(EntityType.Contact);
            _supportedEntityType.Add(EntityType.Opportunity);
            _supportedEntityType.Add(EntityType.Case);

            /*
            _invoiceItemCacheKey = String.Concat(TenantID, "/invoiceitem");
            _invoiceTaxCacheKey = String.Concat(TenantID, "/invoicetax");
            _invoiceLineCacheKey = String.Concat(TenantID, "/invoiceline");
            
            if (_cache.Get(_invoiceItemCacheKey) == null)
            {
                _cache.Insert(_invoiceItemCacheKey, String.Empty);
            }
            if (_cache.Get(_invoiceTaxCacheKey) == null)
            {
                _cache.Insert(_invoiceTaxCacheKey, String.Empty);
            }
            if (_cache.Get(_invoiceLineCacheKey) == null)
            {
                _cache.Insert(_invoiceLineCacheKey, String.Empty);
            }
             */
        }

        protected int TenantID
        {
            get;
            private set;
        }

        protected List<int> SearchByTags(EntityType entityType, int[] exceptIDs, IEnumerable<String> tags)
        {
            if (tags == null || !tags.Any())
                throw new ArgumentException();

            var tagIDs = new List<int>();

            foreach (var tag in tags)
                tagIDs.Add(Db.ExecuteScalar<int>(Query("crm_tag")
                        .Select("id")
                        .Where(Exp.Eq("entity_type", (int)entityType) & Exp.Eq("trim(lower(title))", tag.Trim().ToLower()))));

            var sqlQuery = new SqlQuery("crm_entity_tag")
                .Select("entity_id")
                .Select("count(*) as count")
                .GroupBy("entity_id")
                .Having(Exp.Eq("count", tags.Count()));

            if (exceptIDs != null && exceptIDs.Length > 0)
                sqlQuery.Where(Exp.In("entity_id", exceptIDs) & Exp.Eq("entity_type", (int)entityType));
            else
                sqlQuery.Where(Exp.Eq("entity_type", (int)entityType));

            sqlQuery.Where(Exp.In("tag_id", tagIDs));

            return Db.ExecuteList(sqlQuery).ConvertAll(row => Convert.ToInt32(row[0]));
        }

        protected Dictionary<int, int[]> GetRelativeToEntity(int[] contactID, EntityType entityType, int[] entityID)
        {
            var sqlQuery = new SqlQuery("crm_entity_contact");

            if (contactID != null && contactID.Length > 0 && (entityID == null || entityID.Length == 0))
                sqlQuery.Select("entity_id", "contact_id").Where(Exp.Eq("entity_type", entityType) & Exp.In("contact_id", contactID));
            else if (entityID != null && entityID.Length > 0 && (contactID == null || contactID.Length == 0))
                sqlQuery.Select("entity_id", "contact_id").Where(Exp.Eq("entity_type", entityType) & Exp.In("entity_id", entityID));

            var sqlResult = Db.ExecuteList(sqlQuery);

            return sqlResult.GroupBy(item => item[0])
                    .ToDictionary(item => Convert.ToInt32(item.Key),
                                item => item.Select(x => Convert.ToInt32(x[1])).ToArray());
        }

        protected int[] GetRelativeToEntity(int? contactID, EntityType entityType, int? entityID)
        {
            return GetRelativeToEntityInDb(contactID, entityType, entityID);
        }

        protected int[] GetRelativeToEntityInDb(int? contactID, EntityType entityType, int? entityID)
        {
            var sqlQuery = new SqlQuery("crm_entity_contact");

            if (contactID.HasValue && !entityID.HasValue)
                sqlQuery.Select("entity_id").Where(Exp.Eq("entity_type", entityType) & Exp.Eq("contact_id", contactID.Value));
            else if (!contactID.HasValue && entityID.HasValue)
                sqlQuery.Select("contact_id").Where(Exp.Eq("entity_type", entityType) & Exp.Eq("entity_id", entityID.Value));

            return Db.ExecuteList(sqlQuery).Select(row => Convert.ToInt32(row[0])).ToArray();
        }

        protected void SetRelative(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID == 0)
                throw new ArgumentException();

            using (var tx = Db.BeginTransaction())
            {
                var sqlQuery = new SqlQuery("crm_entity_contact")
                    .Select("contact_id")
                    .Where(Exp.Eq("entity_type", entityType) & Exp.Eq("entity_id", entityID));

                var exists = Db.ExecuteList(sqlQuery).Select(row => Convert.ToInt32(row[0])).ToList();
                foreach (var existContact in exists)
                {
                    var sqlDelete = new SqlDelete("crm_entity_contact").Where(Exp.Eq("entity_type", entityType) & Exp.Eq("entity_id", entityID) & Exp.Eq("contact_id", existContact));
                    Db.ExecuteNonQuery(sqlDelete);
                }

                if (!(contactID == null || contactID.Length == 0))
                    foreach (var id in contactID)
                        SetRelative(id, entityType, entityID);

                tx.Commit();
            }
        }

        protected void SetRelative(int contactID, EntityType entityType, int entityID)
        {
            Db.ExecuteNonQuery(new SqlInsert("crm_entity_contact", true)
                                       .InColumnValue("entity_id", entityID)
                                       .InColumnValue("entity_type", (int)entityType)
                                       .InColumnValue("contact_id", contactID)
                                    );
        }

        protected void RemoveRelativeInDb(int[] contactID, EntityType entityType, int[] entityID)
        {

            if ((contactID == null || contactID.Length == 0) && (entityID == null || entityID.Length == 0))
                throw new ArgumentException();

            var sqlQuery = new SqlDelete("crm_entity_contact");

            if (contactID != null && contactID.Length > 0)
                sqlQuery.Where(Exp.In("contact_id", contactID));

            if (entityID != null && entityID.Length > 0)
                sqlQuery.Where(Exp.In("entity_id", entityID) & Exp.Eq("entity_type", (int)entityType));

            Db.ExecuteNonQuery(sqlQuery);
        }

        protected void RemoveRelative(int contactID, EntityType entityType, int entityID)
        {
            int[] contactIDs = null;
            int[] entityIDs = null;


            if (contactID > 0)
                contactIDs = new[] { contactID };

            if (entityID > 0)
                entityIDs = new[] { entityID };


            RemoveRelativeInDb(contactIDs, entityType, entityIDs);
        }


        public int SaveOrganisationLogo(byte[] bytes)
        {
            var logo_id = 0;
            logo_id = Db.ExecuteScalar<int>(
                                        Insert("crm_organisation_logo")
                                        .InColumnValue("id", 0)
                                        .InColumnValue("content", Convert.ToBase64String(bytes))
                                        .InColumnValue("create_on", DateTime.UtcNow)
                                        .InColumnValue("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                                        .Identity(1, 0, true));
            return logo_id;
        }

        public string GetOrganisationLogoBase64(int logo_id)
        {
            var content = "";
            if (logo_id <= 0) throw new ArgumentException();

            content = Db.ExecuteList(
                                    new SqlQuery("crm_organisation_logo")
                                    .Select("content")
                                    .Where("id", logo_id)).Select(row => Convert.ToString(row[0])).FirstOrDefault();
            return content;
        }

        #region HasCRMActivity

        public bool HasActivity()
        {
            return Db.ExecuteScalar<bool>(@"select exists(select 1 from crm_case where tenant_id = @tid) or " +
                "exists(select 1 from crm_deal where tenant_id = @tid) or exists(select 1 from crm_task where tenant_id = @tid) or " +
                "exists(select 1 from crm_contact where tenant_id = @tid)", new { tid = TenantID });
        }

        #endregion

        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(GetTenantColumnName(table)).Values(TenantID);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";
            if (!table.Contains(" ")) return tenant;
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;
        }


        protected static Guid ToGuid(object guid)
        {
            var str = guid as string;
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }

        protected Exp BuildLike(string[] columns, string[] keywords)
        {
            return BuildLike(columns, keywords, true);
        }

        protected Exp BuildLike(string[] columns, string[] keywords, bool startWith)
        {
            if (columns == null) throw new ArgumentNullException("columns");
            if (keywords == null) throw new ArgumentNullException("keywords");

            var like = Exp.Empty;
            foreach (var keyword in keywords)
            {
                if (keyword == null) throw new ArgumentNullException("keyword");
                var keywordLike = Exp.Empty;
                foreach (string column in columns)
                {
                    if (column == null) throw new ArgumentNullException("column");
                    keywordLike |= Exp.Like(column, keyword, startWith ? SqlLike.StartWith : SqlLike.EndWith) |
                                  Exp.Like(column, ' ' + keyword);
                }
                like &= keywordLike;
            }
            return like;
        }
    }
}
