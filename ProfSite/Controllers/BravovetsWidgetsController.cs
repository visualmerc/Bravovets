using System.Collections.Generic;
using System.Web.Mvc;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainObject.Infrastructure;
using BravoVets.DomainService.Service;
using ProfSite.Models;
using ProfSite.Resources;

namespace ProfSite.Controllers
{
    public class BravovetsWidgetsController : AbstractBaseSocialController
    {
        protected readonly SyndicatedContentDomainService syndicatedContentService;
        protected readonly BravoVetsUserDomainService vetUserDomainService;

        public BravovetsWidgetsController()
        {
            vetUserDomainService = new BravoVetsUserDomainService();
            syndicatedContentService = new SyndicatedContentDomainService();
        }

        public ActionResult SocialTips()
        {
            int userId = GetCurrentUserId();
            int langId = GetCountryId();

            List<SyndicatedContent> contents = syndicatedContentService.GetSocialTips(userId, langId,
                ContentSortEnum.ContentDate,
                new PagingToken {StartRecord = 0, TotalRecords = 5});

            SyndicatedContentWidgetModel model = GetSyndicatedContentWidgetModel("social-tips", "Social Tips", "/socialtips", contents, false);
            return PartialView("SyndicatedContentWidget", model);
        }

        private SyndicatedContentWidgetModel GetSyndicatedContentWidgetModel(string id,string title, string url,
            List<SyndicatedContent> contents,bool showShare)
        {
            var model = new SyndicatedContentWidgetModel
                        {
                            Id = id,
                            Title = title,
                            Contents = new List<SyndicatedContentModel>(),
                            ViewAllUrl = url,
                            ShowShare = showShare,
                        };

            foreach (SyndicatedContent syndicatedContent in contents)
            {
                var item = new SyndicatedContentModel(syndicatedContent, ContentFilterEnum.All,false);
                model.Contents.Add(item);
            }
            if (model.Contents.Count > 0)
            {
                model.Contents[0].ActiveCSS = "active";
            }
            return model;
        }

        public ActionResult TrendingTopics()
        {
            int userId = GetCurrentUserId();
            int langId = GetCountryId();

            List<SyndicatedContent> contents = syndicatedContentService.GetTrendingTopics(userId, langId,
                    ContentSortEnum.ContentDate, new PagingToken{StartRecord = 0,TotalRecords = 5});

            SyndicatedContentWidgetModel model = GetSyndicatedContentWidgetModel("trending-topics", "Trending Topics", "/trendingtopics", contents,true);
            return PartialView("SyndicatedContentWidget", model);
        }


        public ActionResult BravectoContent()
        {
            int userId = GetCurrentUserId();
            int langId = GetCountryId();

            var contents =new List<SyndicatedContent>();
            if (contents.Count.Equals(0))
                return new EmptyResult();


            SyndicatedContentWidgetModel model = GetSyndicatedContentWidgetModel("bravecto-content", "Bravecto Content", "/resourcetopics", contents,false);
            return PartialView("SyndicatedContentWidget", model);
        }

        public ActionResult SocialActivity()
        {
            SocialIntegrationModel model = GetSocialFollowingModel();

            return PartialView("SocialActivityWidget", model);
        }
    }
}