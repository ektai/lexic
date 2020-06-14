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
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;
using MimeKit.Utils;
using ASC.Web.Core.WhiteLabel;

namespace ASC.Web.Studio.Core.Notify
{
    public static class NotifyConfiguration
    {
        private static bool configured;
        private static readonly object locker = new object();
        private static readonly Regex urlReplacer = new Regex(@"(<a [^>]*href=(('(?<url>[^>']*)')|(""(?<url>[^>""]*)""))[^>]*>)|(<img [^>]*src=(('(?<url>(?![data:|cid:])[^>']*)')|(""(?<url>(?![data:|cid:])[^>""]*)""))[^/>]*/?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex textileLinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);

        public static void Configure()
        {
            lock (locker)
            {
                if (!configured)
                {
                    configured = true;

                    WorkContext.NotifyContext.NotifyClientRegistration += NotifyClientRegisterCallback;
                    WorkContext.NotifyContext.NotifyEngine.BeforeTransferRequest += BeforeTransferRequest;
                }
            }
        }

        public static void RegisterSendMethods()
        {
            StudioNotifyService.Instance.RegisterSendMethod();
        }


        private static void NotifyClientRegisterCallback(Context context, INotifyClient client)
        {
            #region url correction

            var absoluteUrl = new SendInterceptorSkeleton(
                "Web.UrlAbsoluter",
                InterceptorPlace.MessageSend,
                InterceptorLifetime.Global,
                (r, p) =>
                {
                    if (r != null && r.CurrentMessage != null && r.CurrentMessage.ContentType == Pattern.HTMLContentType)
                    {
                        var body = r.CurrentMessage.Body;

                        body = urlReplacer.Replace(body, m =>
                        {
                            var url = m.Groups["url"].Value;
                            var ind = m.Groups["url"].Index - m.Index;
                            return string.IsNullOrEmpty(url) && ind > 0 ?
                                m.Value.Insert(ind, CommonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                                m.Value.Replace(url, CommonLinkUtility.GetFullAbsolutePath(url));
                        });

                        body = textileLinkReplacer.Replace(body, m =>
                        {
                            var url = m.Groups["link"].Value;
                            var ind = m.Groups["link"].Index - m.Index;
                            return string.IsNullOrEmpty(url) && ind > 0 ?
                                m.Value.Insert(ind, CommonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                                m.Value.Replace(url, CommonLinkUtility.GetFullAbsolutePath(url));
                        });

                        r.CurrentMessage.Body = body;
                    }
                    return false;
                });
            client.AddInterceptor(absoluteUrl);

            #endregion

            #region security and culture

            var securityAndCulture = new SendInterceptorSkeleton(
                "ProductSecurityInterceptor",
                 InterceptorPlace.DirectSend,
                 InterceptorLifetime.Global,
                 (r, p) =>
                 {
                     try
                     {
                         // culture
                         var u = Constants.LostUser;

                         if (32 <= r.Recipient.ID.Length)
                         {
                             var guid = default(Guid);
                             try
                             {
                                 guid = new Guid(r.Recipient.ID);
                             }
                             catch (FormatException) { }
                             catch (OverflowException) { }

                             if (guid != default(Guid))
                             {
                                 u = CoreContext.UserManager.GetUsers(guid);
                             }
                         }

                         if (Constants.LostUser.Equals(u))
                         {
                             u = CoreContext.UserManager.GetUserByEmail(r.Recipient.ID);
                         }

                         if (Constants.LostUser.Equals(u))
                         {
                             u = CoreContext.UserManager.GetUserByUserName(r.Recipient.ID);
                         }

                         if (!Constants.LostUser.Equals(u))
                         {
                             var culture = !string.IsNullOrEmpty(u.CultureName) ? u.GetCulture() : CoreContext.TenantManager.GetCurrentTenant().GetCulture();
                             Thread.CurrentThread.CurrentCulture = culture;
                             Thread.CurrentThread.CurrentUICulture = culture;

                             // security
                             var tag = r.Arguments.Find(a => a.Tag == CommonTags.ModuleID);
                             var productId = tag != null ? (Guid)tag.Value : Guid.Empty;
                             if (productId == Guid.Empty)
                             {
                                 tag = r.Arguments.Find(a => a.Tag == CommonTags.ProductID);
                                 productId = tag != null ? (Guid)tag.Value : Guid.Empty;
                             }
                             if (productId == Guid.Empty)
                             {
                                 productId = (Guid)(CallContext.GetData("asc.web.product_id") ?? Guid.Empty);
                             }
                             if (productId != Guid.Empty && productId != new Guid("f4d98afdd336433287783c6945c81ea0") /* ignore people product */)
                             {
                                 return !WebItemSecurity.IsAvailableForUser(productId, u.ID);
                             }
                         }

                         var tagCulture = r.Arguments.FirstOrDefault(a => a.Tag == CommonTags.Culture);
                         if (tagCulture != null)
                         {
                             var culture = CultureInfo.GetCultureInfo((string)tagCulture.Value);
                             Thread.CurrentThread.CurrentCulture = culture;
                             Thread.CurrentThread.CurrentUICulture = culture;
                         }
                     }
                     catch (Exception error)
                     {
                         LogManager.GetLogger("ASC").Error(error);
                     }
                     return false;
                 });
            client.AddInterceptor(securityAndCulture);

            #endregion

            #region white label correction

            var whiteLabel = new SendInterceptorSkeleton(
                "WhiteLabelInterceptor",
                 InterceptorPlace.MessageSend,
                 InterceptorLifetime.Global,
                 (r, p) =>
                 {
                     try
                     {
                         var tags = r.Arguments;

                         var logoTextTag = tags.FirstOrDefault(a => a.Tag == CommonTags.LetterLogoText);
                         var logoText = logoTextTag != null ? (String)logoTextTag.Value : string.Empty;

                         if (!string.IsNullOrEmpty(logoText))
                         {
                             r.CurrentMessage.Body = r.CurrentMessage.Body
                                 .Replace(string.Format("${{{0}}}", CommonTags.LetterLogoText), logoText);
                         }
                     }
                     catch (Exception error)
                     {
                         LogManager.GetLogger("ASC").Error(error);
                     }
                     return false;
                 });
            client.AddInterceptor(whiteLabel);

            #endregion
        }


        private static void BeforeTransferRequest(NotifyEngine sender, NotifyRequest request)
        {
            var aid = Guid.Empty;
            var aname = string.Empty;
            if (SecurityContext.IsAuthenticated)
            {
                aid = SecurityContext.CurrentAccount.ID;
                if (CoreContext.UserManager.UserExists(aid))
                {
                    aname = CoreContext.UserManager.GetUsers(aid).DisplayUserName(false)
                        .Replace(">", "&#62")
                        .Replace("<", "&#60");
                }
            }

            IProduct product;
            IModule module;
            CommonLinkUtility.GetLocationByRequest(out product, out module);
            if (product == null && CallContext.GetData("asc.web.product_id") != null)
            {
                product = WebItemManager.Instance[(Guid)CallContext.GetData("asc.web.product_id")] as IProduct;
            }

            var logoText = TenantWhiteLabelSettings.DefaultLogoText;
            if ((TenantExtra.Enterprise || CoreContext.Configuration.CustomMode) && !MailWhiteLabelSettings.Instance.IsDefault)
            {
                logoText = TenantLogoManager.GetLogoText();
            }

            request.Arguments.Add(new TagValue(CommonTags.AuthorID, aid));
            request.Arguments.Add(new TagValue(CommonTags.AuthorName, aname));
            request.Arguments.Add(new TagValue(CommonTags.AuthorUrl, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(aid))));
            request.Arguments.Add(new TagValue(CommonTags.VirtualRootPath, CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')));
            request.Arguments.Add(new TagValue(CommonTags.ProductID, product != null ? product.ID : Guid.Empty));
            request.Arguments.Add(new TagValue(CommonTags.ModuleID, module != null ? module.ID : Guid.Empty));
            request.Arguments.Add(new TagValue(CommonTags.ProductUrl, CommonLinkUtility.GetFullAbsolutePath(product != null ? product.StartURL : "~")));
            request.Arguments.Add(new TagValue(CommonTags.DateTime, TenantUtil.DateTimeNow()));
            request.Arguments.Add(new TagValue(CommonTags.RecipientID, Context.SYS_RECIPIENT_ID));
            request.Arguments.Add(new TagValue(CommonTags.ProfileUrl, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetMyStaff())));
            request.Arguments.Add(new TagValue(CommonTags.RecipientSubscriptionConfigURL, CommonLinkUtility.GetMyStaff()));
            request.Arguments.Add(new TagValue(CommonTags.HelpLink, CommonLinkUtility.GetHelpLink(false)));
            request.Arguments.Add(new TagValue(CommonTags.LetterLogoText, logoText));
            request.Arguments.Add(new TagValue(CommonTags.MailWhiteLabelSettings, MailWhiteLabelSettings.Instance));
            request.Arguments.Add(new TagValue(CommonTags.SendFrom, CoreContext.TenantManager.GetCurrentTenant().Name));
            request.Arguments.Add(new TagValue(CommonTags.ImagePath, StudioNotifyHelper.GetNotificationImageUrl("").TrimEnd('/')));

            AddLetterLogo(request);
        }

        private static void AddLetterLogo(NotifyRequest request)
        {
            if (TenantExtra.Enterprise || CoreContext.Configuration.CustomMode)
            {
                try
                {
                    var logoData = TenantLogoManager.GetMailLogoDataFromCache();

                    if (logoData == null)
                    {
                        var logoStream = TenantLogoManager.GetWhitelabelMailLogo();
                        logoData = ReadStreamToByteArray(logoStream) ?? GetDefaultMailLogo();

                        if (logoData != null)
                            TenantLogoManager.InsertMailLogoDataToCache(logoData);
                    }

                    if (logoData != null)
                    {
                        var attachment = new NotifyMessageAttachment
                        {
                            FileName = "logo.png",
                            Content = logoData,
                            ContentId = MimeUtils.GenerateMessageId()
                        };

                        request.Arguments.Add(new TagValue(CommonTags.LetterLogo, "cid:" + attachment.ContentId));
                        request.Arguments.Add(new TagValue(CommonTags.EmbeddedAttachments, new[] { attachment }));
                        return;
                    }
                }
                catch (Exception error)
                {
                    LogManager.GetLogger("ASC").Error(error);
                }
            }

            var logoUrl = CommonLinkUtility.GetFullAbsolutePath(TenantLogoManager.GetLogoDark(true));

            request.Arguments.Add(new TagValue(CommonTags.LetterLogo, logoUrl));
        }

        private static byte[] ReadStreamToByteArray(Stream inputStream)
        {
            if (inputStream == null) return null;

            using (inputStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    inputStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] GetDefaultMailLogo()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "skins", "default", "images", "lexic_logo", "dark_general.png");

            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
        }
    }
}
