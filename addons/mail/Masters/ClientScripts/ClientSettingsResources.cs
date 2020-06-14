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
using System.Web;
using System.Web.Configuration;
using ASC.Mail;
using ASC.Core;
using ASC.Core.Users;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Mail.Masters.ClientScripts
{
    public class MasterSettingsResources : ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.Mail.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            return new List<KeyValuePair<string, object>>(1)
            {
                RegisterObject(new
                {
                    file_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_21.png"),
                    file_archive_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_archive_21.png"),
                    file_avi_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_avi_21.png"),
                    file_cal_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_cal_21.png"),
                    file_csv_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_csv_21.png"),
                    file_djvu_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_djvu_21.png"),
                    file_doc_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_doc_21.png"),
                    file_doct_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_doct_21.png"),
                    file_docx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_docx_21.png"),
                    file_dvd_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_dvd_21.png"),
                    file_ebook_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_ebook_21.png"),
                    file_flv_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_flv_21.png"),
                    file_gdoc_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_gdoc_21.png"),
                    file_gsheet_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_gsheet_21.png"),
                    file_gslides_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_gslides_21.png"),
                    file_html_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_html_21.png"),
                    file_iaf_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_iaf_21.png"),
                    file_image_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_image_21.png"),
                    file_m2ts_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_m2ts_21.png"),
                    file_mkv_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_mkv_21.png"),
                    file_mov_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_mov_21.png"),
                    file_mp4_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_mp4_21.png"),
                    file_mpg_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_mpg_21.png"),
                    file_odp_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_odp_21.png"),
                    file_ods_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_ods_21.png"),
                    file_odt_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_odt_21.png"),
                    file_pdf_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_pdf_21.png"),
                    file_pps_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_pps_21.png"),
                    file_ppsx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_ppsx_21.png"),
                    file_ppt_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_ppt_21.png"),
                    file_pptt_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_pptt_21.png"),
                    file_pptx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_pptx_21.png"),
                    file_rtf_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_rtf_21.png"),
                    file_sound_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_sound_21.png"),
                    file_svg_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_svg_21.png"),
                    file_txt_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_txt_21.png"),
                    file_xls_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xls_21.png"),
                    file_xlst_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xlst_21.png"),
                    file_xlsx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xlsx_21.png"),
                    file_xml_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xml_21.png"),
                    file_xps_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xps_21.png"),
                    file_docm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_docm_21.png"),
                    file_dot_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_dot_21.png"),
                    file_dotm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_dotm_21.png"),
                    file_dotx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_dotx_21.png"),
                    file_pot_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_pot_21.png"),
                    file_potm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_potm_21.png"),
                    file_potx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_potx_21.png"),
                    file_ppsm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_ppsm_21.png"),
                    file_pptm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_pptm_21.png"),
                    file_soundunk_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_soundunk_21.png"),
                    file_video_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_video_21.png"),
                    file_videounk_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_videounk_21.png"),
                    file_xlsm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xlsm_21.png"),
                    file_xlt_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xlt_21.png"),
                    file_xltm_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xltm_21.png"),
                    file_xltx_21 = CommonLinkUtility.GetFullAbsolutePath("~/UserControls/Common/ckeditor/plugins/filetype/images/file_xltx_21.png"),
                    IsTurnOnAttachmentsGroupOperations = Defines.IsAttachmentsGroupOperationsAvailable,
                    IsTurnOnOAuth = GoogleLoginProvider.Instance.IsEnabled && Convert.ToBoolean(WebConfigurationManager.AppSettings["mail.google-oauth"] ?? "true"),
                    OAuthLocation = VirtualPathUtility.ToAbsolute(OAuth.Location),
                    userDisplayFormat = (int) UserFormatter.GetUserDisplayDefaultOrder()
        })
            };
        }

        protected override string GetCacheHash()
        {
            /* return user last mod time + culture */
            return SecurityContext.CurrentAccount.ID.ToString()
                       + CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).LastModified.ToString(CultureInfo.InvariantCulture)
                       + CoreContext.TenantManager.GetCurrentTenant().LastModified.ToString(CultureInfo.InvariantCulture);
        }
    }
}