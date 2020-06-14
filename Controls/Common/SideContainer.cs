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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Controls.Common
{
    /// <summary>
    /// Base side container
    /// </summary>   
    [ToolboxData("<{0}:SideContainer runat=server></{0}:SideContainer>")]
    public class SideContainer : PlaceHolder
    {
        [Category("Title"), PersistenceMode(PersistenceMode.Attribute)]
        public string Title { get; set; }

        [Category("Title"), PersistenceMode(PersistenceMode.Attribute)]
        public string ImageURL { get; set; }

        [Category("Style"), PersistenceMode(PersistenceMode.Attribute)]
        public string HeaderCSSClass { get; set; }

        [Category("Style"), PersistenceMode(PersistenceMode.Attribute)]
        public string BodyCSSClass { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();
            sb.Append("<div class='studioSideBox'>");

            sb.Append("<div class='studioSideBoxContent'>");

            //header
            sb.Append("<div class='" + (String.IsNullOrEmpty(HeaderCSSClass) ? "studioSideBoxHeader" : HeaderCSSClass) + "'>");
            if (!String.IsNullOrEmpty(ImageURL))
                sb.Append("<img alt='' style='margin-right:8px;' align='absmiddle' src='" + ImageURL + "'/>");

            sb.Append((Title ?? "").HtmlEncode());
            sb.Append("</div>");

            sb.Append("<div class='" + (String.IsNullOrEmpty(BodyCSSClass) ? "studioSideBoxBody" : BodyCSSClass) + "'>");

            writer.Write(sb.ToString());
            base.Render(writer);


            sb = new StringBuilder();
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("</div>");
            writer.Write(sb.ToString());
        }
    }
}