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


using ASC.Web.CRM.Core.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ASC.Web.CRM.Classes
{
    public class ImportCSVSettings
    {
        public ImportCSVSettings(String jsonStr)
        {
            var json = JObject.Parse(jsonStr);

            if (json == null) return;

            HasHeader = json["has_header"].Value<bool>();
            DelimiterCharacter = Convert.ToChar(json["delimiter_character"].Value<int>());
            Encoding = Encoding.GetEncoding(json["encoding"].Value<int>());
            QuoteType = Convert.ToChar(json["quote_character"].Value<int>());

            JToken columnMappingToken;

            if (json.TryGetValue("column_mapping", out columnMappingToken))
                ColumnMapping = (JObject)columnMappingToken;

            JToken duplicateRecordRuleToken;

            if (json.TryGetValue("removing_duplicates_behavior", out duplicateRecordRuleToken))
                DuplicateRecordRule = duplicateRecordRuleToken.Value<int>();

            JToken isPrivateToken;
            if (json.TryGetValue("is_private", out isPrivateToken))
            {
                IsPrivate = isPrivateToken.Value<bool>();
                AccessList = json["access_list"].Values<String>().Select(item => new Guid(item)).ToList();
            }

            JToken shareTypeToken;
            if (json.TryGetValue("share_type", out shareTypeToken))
            {
                ShareType = (ShareType)(shareTypeToken.Value<int>());
                ContactManagers = json["contact_managers"].Values<String>().Select(item => new Guid(item)).ToList();
            }

            if (json["tags"] != null)
            {
                Tags = json["tags"].Values<String>().ToList();
            }
        }

        public bool IsPrivate { get; set; }

        public ShareType ShareType { get; set; }

        public int DuplicateRecordRule { get; set; }

        public bool HasHeader { get; set; }

        public char DelimiterCharacter { get; set; }

        public char QuoteType { get; set; }

        public Encoding Encoding { get; set; }

        public List<Guid> AccessList { get; set; }

        public List<Guid> ContactManagers { get; set; }

        public JObject ColumnMapping { get; set; }

        public List<String> Tags { get; set; }
    }

}