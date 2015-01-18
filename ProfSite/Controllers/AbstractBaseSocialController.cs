using System.Web.Mvc;
using BravoVets.DomainObject;
using BravoVets.DomainService.Service;
using log4net;
using ProfSite.Models;
using ProfSite.Utils;

namespace ProfSite.Controllers
{
    public abstract class AbstractBaseSocialController : AbstractBaseController
    {
        protected readonly BravoVetsUserDomainService vetUserDomainService;
        protected readonly SyndicatedContentDomainService syndicatedContentService;
        protected readonly PublishQueueDomainService publishQueueDomainService;

        protected readonly ILog Logger = LogManager.GetLogger(typeof(AbstractBaseSocialController));

        public AbstractBaseSocialController()
        {
            vetUserDomainService = new BravoVetsUserDomainService();
            syndicatedContentService = new SyndicatedContentDomainService();
            publishQueueDomainService = new PublishQueueDomainService();
        }

        protected VeterinarianSocialIntegration GetTwitterVetSocialIntegration()
        {
            var userId = GetCurrentUserId();
            BravoVetsUser currentUser = vetUserDomainService.GetBravoVetsUser(userId);

            var twitterInfo = vetUserDomainService.GetTwitterSocialIntegration(currentUser);

            return twitterInfo;
        }

        protected VeterinarianSocialIntegration GetFacebookVetSocialIntegration()
        {
            var userId = GetCurrentUserId();
            BravoVetsUser currentUser = vetUserDomainService.GetBravoVetsUser(userId);

            var twitterInfo = vetUserDomainService.GetFacebookSocialIntegration(currentUser);

            return twitterInfo;
        }

        public SocialIntegrationModel GetSocialFollowingModel()
        {
            
            var model = GetSocialIntegrationModel(GetCurrentUserId());
            if (model.FacebookProfile == null)
                model.FacebookProfile = new FacebookProfile {likes = 0};

            if (model.TwitterProfile == null)
                model.TwitterProfile = new TwitterProfile {followers_count = 0};

            return model;
        }

        public static SocialIntegrationModel GetSocialIntegrationModel(int userId)
        {
            var userDomainService = new BravoVetsUserDomainService();
            BravoVetsUser currentUser = userDomainService.GetBravoVetsUser(userId);

            var twitterInfo = userDomainService.GetTwitterSocialIntegration(currentUser);

            var model = new SocialIntegrationModel();


            if (twitterInfo != null && !string.IsNullOrEmpty(twitterInfo.AccessToken))
            {
                model.TwitterProfile = new TwitterProfile {followers_count = twitterInfo.NumberOfFollowers,difference = twitterInfo.FollowerDiff};
            }
          
            var facebookInfo = userDomainService.GetFacebookSocialIntegration(currentUser);

            if (facebookInfo != null && !string.IsNullOrEmpty(facebookInfo.AccessToken))
            {
                model.FacebookProfile = new FacebookProfile {likes = facebookInfo.NumberOfFollowers,difference = facebookInfo.FollowerDiff};
            }
           
            return model;
        }

        public static void UpdateSocialIntegrationInfo(int userId, string siteLangauage)
        {
            var userDomainService = new BravoVetsUserDomainService();
            BravoVetsUser currentUser = userDomainService.GetBravoVetsUser(userId);

            var twitterInfo = userDomainService.GetTwitterSocialIntegration(currentUser);
            if (twitterInfo != null && !string.IsNullOrEmpty(twitterInfo.AccessToken))
            {
                int previousFollowers = twitterInfo.NumberOfFollowers;
                var twitterProfile = TwitterHelper.GetProfile(twitterInfo.AccessCode, twitterInfo.AccessToken,
                    twitterInfo.AccountName, siteLangauage);
                int currentFollowers = twitterProfile.followers_count;
                int diff = currentFollowers - previousFollowers;
                twitterInfo.FollowerDiff = diff;
                twitterInfo.NumberOfFollowers = currentFollowers;

                userDomainService.UpdateVeterinarianSocialIntegration(twitterInfo);
            }

            var faceBookInfo = userDomainService.GetFacebookSocialIntegration(currentUser);
            if (faceBookInfo != null && !string.IsNullOrEmpty(faceBookInfo.AccessToken))
            {
                int previousLikes = faceBookInfo.NumberOfFollowers;
                var faceBookProfile = FacebookHelper.GetProfile(faceBookInfo);
                int likes = faceBookProfile.likes;
                int diff = likes - previousLikes;
                faceBookInfo.FollowerDiff = diff;
                faceBookInfo.NumberOfFollowers = likes;

                userDomainService.UpdateVeterinarianSocialIntegration(faceBookInfo); 
            }
        }


        public ActionResult GetSocialFollowing()
        {
            var model = GetSocialFollowingModel();

            return PartialView("SocialFollowing", model);
        }

    }
}