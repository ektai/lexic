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
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Web.UserControls.Wiki.Data
{
    class BaseDao : IDisposable
    {
        private const string TenantColumn = "tenant";

        protected readonly DbManager db;
        protected readonly int tenant;


        public BaseDao(string dbid, int tenant)
        {
            this.db = new DbManager(dbid);
            this.tenant = tenant;
        }


        public void Dispose()
        {
            db.Dispose();
        }

        
        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), tenant);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumnValue(GetTenantColumnName(table), tenant);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), tenant);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), tenant);
        }

        private string GetTenantColumnName(string table)
        {
            var pos = table.LastIndexOf(' ');
            return (0 < pos ? table.Substring(pos).Trim() + '.' : string.Empty) + TenantColumn;
        }
    }
}