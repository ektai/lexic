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
using System.Linq;
using System.Threading;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Thrdparty;
using ASC.Thrdparty.Twitter;
using ASC.Web.CRM.Classes.SocialMedia;
using ASC.Web.CRM.Resources;


namespace ASC.Web.CRM.SocialMedia
{
    public class SocialMediaUI
    {
        private ILog _logger = LogManager.GetLogger("ASC");
        private DaoFactory DaoFactory { get; set; }

        public SocialMediaUI(DaoFactory factory)
        {
            DaoFactory = factory;
        }

        public List<SocialMediaImageDescription> GetContactSMImages(int contactID)
        {
            var contact = DaoFactory.ContactDao.GetByID(contactID);

            var images = new List<SocialMediaImageDescription>();


            var socialNetworks = DaoFactory.ContactInfoDao.GetList(contact.ID, null, null, null);

            var twitterAccounts = socialNetworks.Where(sn => sn.InfoType == ContactInfoType.Twitter).Select(sn => sn.Data.Trim()).ToList();

            Func<List<String>, Tenant, List<SocialMediaImageDescription>> dlgGetTwitterImageDescriptionList = GetTwitterImageDescriptionList;

            // Parallelizing

            var waitHandles = new List<WaitHandle>();

            var currentTenant = CoreContext.TenantManager.GetCurrentTenant();

            var arGetAvatarsFromTwitter = dlgGetTwitterImageDescriptionList.BeginInvoke(twitterAccounts, currentTenant, null, null);
            waitHandles.Add(arGetAvatarsFromTwitter.AsyncWaitHandle);

            WaitHandle.WaitAll(waitHandles.ToArray());

            images.AddRange(dlgGetTwitterImageDescriptionList.EndInvoke(arGetAvatarsFromTwitter));

            return images;
        }

        public List<SocialMediaImageDescription> GetContactSMImages(List<String> twitter)
        {
            var images = new List<SocialMediaImageDescription>();

            Func<List<String>, Tenant, List<SocialMediaImageDescription>> dlgGetTwitterImageDescriptionList = GetTwitterImageDescriptionList;

            // Parallelizing

            var waitHandles = new List<WaitHandle>();

            var currentTenant = CoreContext.TenantManager.GetCurrentTenant();

            var arGetAvatarsFromTwitter = dlgGetTwitterImageDescriptionList.BeginInvoke(twitter, currentTenant, null, null);
            waitHandles.Add(arGetAvatarsFromTwitter.AsyncWaitHandle);

            WaitHandle.WaitAll(waitHandles.ToArray());

            images.AddRange(dlgGetTwitterImageDescriptionList.EndInvoke(arGetAvatarsFromTwitter));

            return images;
        }

        private List<SocialMediaImageDescription> GetTwitterImageDescriptionList(List<String> twitterAccounts, Tenant tenant)
        {
            var images = new List<SocialMediaImageDescription>();

            if (twitterAccounts.Count == 0)
                return images;

            try
            {
                CoreContext.TenantManager.SetCurrentTenant(tenant);

                var provider = new TwitterDataProvider(TwitterApiHelper.GetTwitterApiInfoForCurrentUser());

                twitterAccounts = twitterAccounts.Distinct().ToList();
                images.AddRange(from twitterAccount in twitterAccounts
                                let imageUrl = provider.GetUrlOfUserImage(twitterAccount, TwitterDataProvider.ImageSize.Small)
                                where imageUrl != null
                                select new SocialMediaImageDescription
                                    {
                                        Identity = twitterAccount,
                                        ImageUrl = imageUrl,
                                        SocialNetwork = SocialNetworks.Twitter
                                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return images;
        }

        public Exception ProcessError(Exception exception, string methodName)
        {
            if (exception is ConnectionFailureException)
                return new Exception(CRMSocialMediaResource.ErrorTwitterConnectionFailure);

            if (exception is RateLimitException)
                return new Exception(CRMSocialMediaResource.ErrorTwitterRateLimit);

            if (exception is ResourceNotFoundException)
                return new Exception(CRMSocialMediaResource.ErrorTwitterAccountNotFound);

            if (exception is UnauthorizedException)
                return new Exception(CRMSocialMediaResource.ErrorTwitterUnauthorized);

            if (exception is SocialMediaException)
                return new Exception(CRMSocialMediaResource.ErrorInternalServer);

            if (exception is SocialMediaAccountNotFound)
                return exception;

            var unknownErrorText = String.Format("{0} error: Unknown exception:", methodName);
            _logger.Error(unknownErrorText, exception);
            return new Exception(CRMSocialMediaResource.ErrorInternalServer);
        }
    }
}