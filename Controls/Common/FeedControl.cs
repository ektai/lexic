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


using System.ComponentModel;
using System.Web.UI;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Controls.Common
{
    [DefaultProperty("Title")]
    [ToolboxData("<{0}:FeedControl runat=server></{0}:FeedControl>")]
    public class FeedControl : Control
    {
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Title { get; set; }

        [DefaultValue("")]
        [Localizable(false)]
        public string ProductId { get; set; }

        [DefaultValue("")]
        [Localizable(false)]
        public string ContainerId { get; set; }

        [DefaultValue("")]
        [Localizable(true)]
        public string ModuleId { get; set; }

        [DefaultValue(false)]
        [Localizable(false)]
        public bool ContentOnly { get; set; }

        [DefaultValue(true)]
        [Localizable(false)]
        public bool AutoFill { get; set; }

        public FeedControl()
        {
            ContentOnly = false;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (AutoFill)
            {
                if (string.IsNullOrEmpty(ProductId))
                {
                    ProductId = CommonLinkUtility.GetProductID().ToString("D");
                }
                if (string.IsNullOrEmpty(ModuleId))
                {
                    IProduct product;
                    IModule module;
                    CommonLinkUtility.GetLocationByRequest(out product, out module);
                    if (module != null)
                    {
                        ModuleId = module.ID.ToString("D");
                    }
                }
            }

            writer.Write(Services.WhatsNew.feed.RenderRssMeta(Title, ProductId));
        }
    }
}