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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Studio;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.People.Resources;

namespace ASC.Web.People
{
    internal class UserPhotoUploader : IFileUploadHandler
    {
        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var result = new FileUploadResult();
            try
            {
                if (context.Request.Files.Count != 0)
                {
                    Guid userId;
                    try
                    {
                        userId = new Guid(context.Request["userId"]);
                    }
                    catch 
                    {
                        userId = SecurityContext.CurrentAccount.ID;
                    }
                    SecurityContext.DemandPermissions(new UserSecurityProvider(userId), Constants.Action_EditUser);

                    var userPhoto = context.Request.Files[0];

                    if (userPhoto.InputStream.Length > SetupInfo.MaxImageUploadSize)
                    {
                        result.Success = false;
                        result.Message = FileSizeComment.FileImageSizeExceptionString;
                        return result;
                    }
                    
                    var data = new byte[userPhoto.InputStream.Length];

                    var br = new BinaryReader(userPhoto.InputStream);
                    br.Read(data, 0, (int)userPhoto.InputStream.Length);
                    br.Close();

                    CheckImgFormat(data);

                    if (context.Request["autosave"] == "true")
                    {
                        if (data.Length > SetupInfo.MaxImageUploadSize)
                            throw new ImageSizeLimitException();

                        var mainPhoto = UserPhotoManager.SaveOrUpdatePhoto(userId, data);

                        result.Data =
                            new
                                {
                                    main = mainPhoto,
                                    retina = UserPhotoManager.GetRetinaPhotoURL(userId),
                                    max = UserPhotoManager.GetMaxPhotoURL(userId),
                                    big = UserPhotoManager.GetBigPhotoURL(userId),
                                    medium = UserPhotoManager.GetMediumPhotoURL(userId),
                                    small = UserPhotoManager.GetSmallPhotoURL(userId),
                                };
                    }
                    else
                    {
                        result.Data = UserPhotoManager.SaveTempPhoto(data, SetupInfo.MaxImageUploadSize, UserPhotoManager.OriginalFotoSize.Width, UserPhotoManager.OriginalFotoSize.Height);
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = PeopleResource.ErrorEmptyUploadFileSelected;
                }

            }
            catch (UnknownImageFormatException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorUnknownFileImageType;
            }
            catch (ImageWeightLimitException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorImageWeightLimit;
            }
            catch (ImageSizeLimitException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorImageSizetLimit;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message.HtmlEncode();
            }

            return result;
        }

        private static void CheckImgFormat(byte[] data)
        {
            ImageFormat imgFormat;

            try
            {
                using (var stream = new MemoryStream(data))
                using (var img = new Bitmap(stream))
                {
                    imgFormat = img.RawFormat;
                }
            }
            catch (OutOfMemoryException)
            {
                throw new ImageSizeLimitException();
            }
            catch (ArgumentException error)
            {
                throw new UnknownImageFormatException(error);
            }

            if (!imgFormat.Equals(ImageFormat.Png) && !imgFormat.Equals(ImageFormat.Jpeg))
            {
                throw new UnknownImageFormatException();
            }
        }
    }

    public partial class ProfileAction : MainPage
    {
        public ProfileHelper ProfileHelper;

        protected bool IsPageEditProfile()
        {
            return (Request["action"] == "edit");
        }

        protected bool IsAdmin()
        {
            return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin() ||
                WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ProfileHelper = new ProfileHelper(Request["user"]);
            var userInfo = ProfileHelper.UserInfo;

            if (userInfo.IsMe() && userInfo.IsVisitor() && IsPageEditProfile())
            {
                Response.Redirect("/my.aspx?action=edit");
            }

            if (IsPageEditProfile() ? !userInfo.IsMe() && (!IsAdmin() || userInfo.IsOwner()) : !IsAdmin())
            {
                Response.Redirect("~/products/people/", true);
            }

            var userProfileEditControl = LoadControl(UserProfileEditControl.Location) as UserProfileEditControl;


            _contentHolderForEditForm.Controls.Add(userProfileEditControl);
        }
    }
}