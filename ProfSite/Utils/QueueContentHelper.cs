using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Repository.Hierarchy;

namespace ProfSite.Utils
{
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainService.Contract;
    using BravoVets.DomainService.Service;

    using ProfSite.Models;

    public static class QueueContentHelper
    {
        private const int calendarTextLength = 25;

        public static List<SocialCalendarPostModel> GetCalendarPostsByUserByDate(int bravoVetsUserId, DateTime startDate, DateTime endDate)
        {
            var calendarPosts = new List<SocialCalendarPostModel>();

            IPublishQueueDomainService queueService = new PublishQueueDomainService();
            var queueContent = queueService.GetQueuedContentForUser(bravoVetsUserId, startDate, endDate);
            foreach (var queue in queueContent)
            {
                var postModel = ConvertQueueItemToPostModel(queue);

                calendarPosts.Add(postModel);
            }

            return calendarPosts;
        }

        public static void DeliverEligibleQueuedMessages()
        {
            IPublishQueueDomainService queueService = new PublishQueueDomainService();
            ILookupDomainService lookupService = new LookupDomainService();

            var activeCountries = lookupService.GetBravoVetsCountries();

            // Iterate through the active countries
            foreach (var bravoVetsCountry in activeCountries)
            {
                Guid deliverySessionId;
                var queuedContent = queueService.GetAllQueuedContentForPublish(DateTime.UtcNow, bravoVetsCountry.BravoVetsCountryId, out deliverySessionId);
                foreach (QueueContent queueContent in queuedContent)
                {

                    bool wasSuccess = DeliverQueuedContent(queueContent,bravoVetsCountry);

                    queueService.QueueContentForPublish(queueContent);

                    if (wasSuccess)
                    {
                        queueService.LogQueuedContentDelivery(
                            deliverySessionId,
                            queueContent.QueueContentId,
                            wasSuccess,
                            String.Empty);
                    }
                    else
                    {
                        queueService.LogQueuedContentDelivery(
                            deliverySessionId,
                            queueContent.QueueContentId,
                            wasSuccess,
                            queueContent.PublishError);
                    }

                }
            }

        }


        public static bool DeliverQueuedContent(QueueContent queuedContent)
        {
            ILookupDomainService lookupService = new LookupDomainService();
            var countries = lookupService.GetBravoVetsCountries();

            var country = countries.Find(x => x.BravoVetsCountryId == queuedContent.BravoVetsCountryId);

            return DeliverQueuedContent(queuedContent,country);
        }

        private static bool DeliverQueuedContent(QueueContent queuedContent,BravoVetsCountry country)
        {
            bool wasSuccess = false;

            if (queuedContent.PlatformPublishId == (int) SocialPlatformEnum.Facebook)
            {
                wasSuccess = PostFacebookContent(queuedContent,country.LanguageCode);
                if (wasSuccess)
                {
                    queuedContent.IsPublished = true;
                }
            }
            else
            {
                wasSuccess = PostTwitterContent(queuedContent,country.LanguageCode);

                if (wasSuccess)
                {
                    queuedContent.IsPublished = true;
                }
            }

            return wasSuccess;
        }


        private static bool PostTwitterContent(QueueContent queuedContent,string languageCode)
        {
            try
            {
                string accessCode = queuedContent.AccessCode;
                string accessToken = queuedContent.AccountName;

                if (queuedContent.QueueContentAttachments.Count > 0 ||
                    queuedContent.SyndicatedContentPostTypeId == (int)SyndicatedContentPostTypeEnum.ImagePost && queuedContent.QueueContentAttachments.Count > 0)
                {
                    TwitterHelper.PostImage(accessCode, accessToken, queuedContent.ContentText, queuedContent.QueueContentAttachments, languageCode);
                }
                else
                {
                    var contentText = queuedContent.ContentText;
                    if (!string.IsNullOrEmpty(queuedContent.LinkUrl))
                    {
                        contentText += queuedContent.LinkUrl;
                    }

                    TwitterHelper.PostTweet(accessCode, accessToken, contentText, languageCode);
                }

                return true;
            }
            catch (Exception ex)
            {
                queuedContent.PublishError = ex.Message;
                return false;
            }
        }

        private static bool PostFacebookContent(QueueContent queuedContent,string langeuageCode)
        {
            var accessCode = queuedContent.AccessCode;
            var accountId = queuedContent.AccountName;
            try
            {
                switch (queuedContent.SyndicatedContentPostTypeId)
                {
                    case (int) SyndicatedContentPostTypeEnum.LinkPostPage:
                        FacebookHelper.PostLink(accountId, accessCode, queuedContent.ContentText,queuedContent.LinkUrl);
                        break;
                    case (int)SyndicatedContentPostTypeEnum.ImagePost:
                        FacebookHelper.PostImages(accountId,accessCode,queuedContent.ContentText,queuedContent.QueueContentAttachments);
                        break;
                    default:
                        FacebookHelper.PostStatus(accountId, accessCode, queuedContent.ContentText);
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                queuedContent.PublishError = ex.Message;
                return false;
            }
        }


        private static SocialCalendarPostModel ConvertQueueItemToPostModel(QueueContent queue)
        {
            var postModel = new SocialCalendarPostModel();

            var publishDateUtc = queue.PublishDateUtc; 

            switch (queue.PlatformPublishId)
            {
                case (int)SocialPlatformEnum.Facebook:
                    postModel.cssclass = "event-facebook";
                    postModel.network = "facebook";
                    break;
                case (int)SocialPlatformEnum.Twitter:
                    postModel.cssclass = "event-twitter";
                    postModel.network = "twitter";
                    break;
            }
            // postModel.display_start = publishDateUtc.ToLocalTime().ToLongTimeString();

            postModel.display_start = publishDateUtc.Hour.ToString();
            postModel.seedDate = publishDateUtc;

            var unixTime = (long)publishDateUtc.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            postModel.end = unixTime.ToString();
            postModel.id = queue.QueueContentId;
            postModel.start = unixTime.ToString();
            if (queue.ContentText.Length > calendarTextLength)
            {
                postModel.title = queue.ContentText.Substring(0, calendarTextLength) + "...";
            }
            else
            {
                postModel.title = queue.ContentText;
            }
            // postModel.url = "javascript:$.socialPostDialog({ 'dialogType': 'socialcal' }); ";
            postModel.url = "javascript:void(0);";
            return postModel;
        }

        /*
         * {
			"id": "293",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores, deserunt!",
			"url": "http://www.example.com/",
			"class": "event-twitter",
			"network": "twitter",
			"display_start": "10:00",
			"start": "1397063217000",
			"end":   "1397063217000"
		},
         * 		{
			"id": "294",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Saepe, enim.",
			"url": "http://www.example.com/",
			"class": "event-facebook",
			"network": "facebook",
			"display_start": "11:00",
			"start": "1397063217000",
			"end":   "1397063217000"
		},
         */
    }
}