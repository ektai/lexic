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
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using Resources;


namespace ASC.Web.Studio.UserControls.Common.Banner
{
    public partial class Banner : UserControl
    {
        public static string Location { get { return "~/UserControls/Common/Banner/Banner.ascx"; } }
       
        protected static string LanguageName
        {
            get
            {
                return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }
        }
        protected List<BannerType> banners;

        public class BannerType
        {
            public string Id { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Img { get; set; }
            public string ImgType { get; set; }
            public string ImgUrl
            {
                get
                {
                    return WebPath.Exists("/skins/default/images/banner/" + Img + LanguageName + ImgType)
                       ? WebPath.GetPath("/skins/default/images/banner/" + Img + LanguageName + ImgType).ToLowerInvariant()
                       : WebPath.GetPath("/skins/default/images/banner/" + Img + "en" + ImgType).ToLowerInvariant();
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts("~/UserControls/Common/Banner/js/banner.js");

            banners = new List<BannerType>();

            if (CoreContext.Configuration.Personal && SetupInfo.DisplayPersonalBanners)
            {
                banners.Add(new BannerType
                {
                    Id = "chromeStoreBanner",
                    Url = "https://chrome.google.com/webstore/detail/teamlab-personal/iohfebkcjhlelaoibebeohcgkohkcgpn?hl=" + LanguageName,
                    Title = Resource.ChromeStoreBannerTitle,
                    Img = "banner_chrome_store_",
                    ImgType = ".png"

                });
            }
           
        }

    }
}