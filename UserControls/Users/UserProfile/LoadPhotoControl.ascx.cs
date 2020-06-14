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
using ASC.Core.Users;
using ASC.Web.Core.Users;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    public partial class LoadPhotoControl : UserControl
    {
        public UserInfo User { get; set; }

        protected bool HasAvatar { get; set; }
        protected string MainImgUrl { get; set; }
        protected UserPhotoThumbnailSettings ThumbnailSettings { get; set; }

        protected bool IsLdap { get; set; }

        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/LoadPhotoControl.ascx"; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var defuaultPhoto = UserPhotoManager.GetDefaultPhotoAbsoluteWebPath();

            if (User == null)
            {
                MainImgUrl = defuaultPhoto;
                ThumbnailSettings = UserPhotoThumbnailSettings.LoadForDefaultTenant();
            }
            else
            {
                IsLdap = User.IsLDAP();
                HasAvatar = User.HasAvatar();
                MainImgUrl = UserPhotoManager.GetPhotoAbsoluteWebPath(User.ID);
                ThumbnailSettings = UserPhotoThumbnailSettings.LoadForUser(User.ID);
            }

            Page.RegisterStyle("~/UserControls/Users/UserProfile/css/loadphoto_style.less",
                               "~/UserControls/Users/UserProfile/css/jquery.jcrop.less")
                .RegisterBodyScripts("~/js/uploader/ajaxupload.js",
                                    "~/UserControls/Users/UserProfile/js/loadphoto.js",
                                     "~/UserControls/Users/UserProfile/js/jquery.jcrop.js");

            var script =
                string.Format(
                    "window.ASC.Controls.LoadPhotoImage.init('{0}',[{1},{2}],{{point:{{x:{3},y:{4}}},size:{{width:{5},height:{6}}}}},'{7}', '{8}');",
                    User == null ? "" : User.ID.ToString(),
                    UserPhotoManager.SmallFotoSize.Width,
                    UserPhotoManager.SmallFotoSize.Height,
                    ThumbnailSettings.Point.X,
                    ThumbnailSettings.Point.Y,
                    ThumbnailSettings.Size.Width,
                    ThumbnailSettings.Size.Height,
                    HasAvatar ? MainImgUrl : "",
                    defuaultPhoto);

            Page.RegisterInlineScript(script);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}