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
using System.Web;
using System.Web.UI;
using ASC.Web.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Linq;

namespace ASC.Web.Studio.UserControls.Management
{
    [ManagementControl(ManagementType.ProductsAndInstruments, Location, SortOrder = 100)]
    public partial class DefaultPageSettings : UserControl
    {
        public const string Location = "~/UserControls/Management/DefaultPageSettings/DefaultPageSettings.ascx";

        protected List<DefaultStartPageWrapper> DefaultPages { get; set; }
        protected Guid DefaultProductID { get; set; }
        protected string HelpLink { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts("~/UserControls/Management/DefaultPageSettings/js/defaultpage.js");

            DefaultPages = new List<DefaultStartPageWrapper>();

            var defaultPageSettings = StudioDefaultPageSettings.Load();
            DefaultProductID = defaultPageSettings.DefaultProductID;

            var products = WebItemManager.Instance.GetItemsAll<IProduct>().Where(p => p.Visible);
            foreach (var p in products)
            {
                var productInfo = WebItemSecurity.GetSecurityInfo(p.ID.ToString());
                if (productInfo.Enabled)
                    DefaultPages.Add(new DefaultStartPageWrapper
                        {
                            ProductID = p.ID,
                            DisplayName = p.Name,
                            ProductName = p.GetSysName(),
                            IsSelected = DefaultProductID.Equals(p.ID)
                        });
            }

            DefaultPages.Add(new DefaultStartPageWrapper
                {
                    ProductID = defaultPageSettings.FeedModuleID,
                    DisplayName = Resources.UserControlsCommonResource.FeedTitle,
                    ProductName = "feed",
                    IsSelected = DefaultProductID.Equals(defaultPageSettings.FeedModuleID)
                });

            DefaultPages.Add(new DefaultStartPageWrapper
                {
                    ProductID = Guid.Empty,
                    DisplayName = Resources.Resource.DefaultPageSettingsChoiseOfProducts,
                    ProductName = string.Empty,
                    IsSelected = DefaultProductID.Equals(Guid.Empty)
                });

            HelpLink = CommonLinkUtility.GetHelpLink();
        }
    }

    public class DefaultStartPageWrapper
    {
        public Guid ProductID { get; set; }
        public string DisplayName { get; set; }
        public string ProductName { get; set; }
        public bool IsSelected { get; set; }
    }
}