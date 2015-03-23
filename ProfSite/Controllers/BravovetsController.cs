using System;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Web.Mvc;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainObject.Infrastructure;
using BravoVets.DomainService.Service;
using log4net;
using Microsoft.Ajax.Utilities;
using ProfSite.Auth;
using ProfSite.Models;
using ProfSite.Resources;
using ProfSite.Utils;

namespace ProfSite.Controllers
{
    using System.Collections.Generic;

    using BravoVets.DomainObject;
    using BravoVets.DomainService.Contract;

    public class BravovetsController : AbstractBaseController
    {
        protected readonly BravoVetsUserDomainService vetUserDomainService;
        protected readonly SyndicatedContentDomainService syndicatedContentService;

        protected readonly ILog Logger = LogManager.GetLogger(typeof(BravovetsController));

        public BravovetsController()
        {
            vetUserDomainService = new BravoVetsUserDomainService();
            syndicatedContentService = new SyndicatedContentDomainService();
        }



        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var menu = BravectoMenu.CreateBravovetsMenu(filterContext.ActionDescriptor.ActionName);
            ViewBag.Menu = menu;
            ViewBag.Language = this.GetSiteFullLanguage();
        }

        public ActionResult SelectCountry(string subdomain)
        {
            SetUserLanguage();

            var baseUrl = string.IsNullOrEmpty(subdomain) ? string.Empty : string.Format("{0}.", subdomain);

            var selectList = new SelectList(LanguageHelper.SupportedCountries(), "SiteUrl", "DisplayName", "");

            return View(new SelectCountry { ButtonText = Resource.SelectCountryButton, Info = Resource.SelectCountryInfo, BaseUrl = baseUrl, SupportedCountries = selectList });
        }

        public ActionResult Index()
        {
            return View();
        }

        // [ClaimsPrincipalPermission(SecurityAction.Demand, Operation = "AcceptedToc", Resource = "BravoVets")]
        public ActionResult Dashboard()
        {
            ViewBag.Title = Resource.BravoVets_Homepage_Title;
            try
            {
                ClaimsPrincipalPermission.CheckAccess("BravoVets", "AcceptedToc");
            }
            catch (Exception ex)
            {
                return TermsAndConditions();
            }

            return View("Dashboard");
        }

        public ActionResult PracticeHeader()
        {
            //TODO:Push this code to the base class
            var userService = new BravoVetsUserDomainService();
            var currentUser = userService.GetBravoVetsUser(this.GetCurrentUserId());
            var practiceName = currentUser.Veterinarian.BusinessName;
            ViewData["CurrentVetPractice"] = practiceName;

            return PartialView("_Layout_SubHeader");
        }

        public ActionResult TermsAndConditions()
        {
            return View("TermsAndConditions");
        }

        /// <summary>
        /// Save TOC value to db; reset claim and redirect to Dashboard
        /// </summary>
        /// <returns>A redirect</returns>
        public ActionResult AcceptTermsAndConditions()
        {
            var userId = GetCurrentUserId();
            var userDomainService = new BravoVetsUserDomainService();
            var didAccept = userDomainService.AcceptTermsAndConditions(userId);

            if (didAccept)
            {
                this.AuthManager.InvalidateCurrentIdentityInfo();

                return RedirectToAction("EditProfile");
            }
            else
            {
                return this.RedirectToAction("Home", "Bravecto");
            }
        }

        public ActionResult RejectTermsAndConditions()
        {
            var userId = GetCurrentUserId();
            var userDomainService = new BravoVetsUserDomainService();
            var didReject = userDomainService.RejectTermsAndConditions(userId);

            this.AuthManager.InvalidateCurrentIdentityInfo();

            return this.RedirectToAction("Home", "Bravecto");
        }

        public ActionResult TrendingTopics()
        {
            try
            {
                ViewBag.Title = Resource.BravoVets_TrendingTopics_Title;
                ClaimsPrincipalPermission.CheckAccess("BravoVets", "AcceptedToc");
                var model = GetTrendingTopics(ContentSortEnum.ContentDate, ContentFilterEnum.All, 0, 5);

                return View("TrendingTopics", model);
            }
            catch (Exception ex)
            {
                return TermsAndConditions();
            }

        }

        private ViewSyndicatedContentModel GetTrendingTopics(ContentSortEnum sortBy, ContentFilterEnum filterBy, int? skip, int? take)
        {

            int userId = GetCurrentUserId();
            int countryId = GetCountryId();

            if (!skip.HasValue)
                skip = 0;

            if (!take.HasValue)
                take = 5;

            var pageToken = new PagingToken { StartRecord = skip.Value, TotalRecords = take.Value };

            List<SyndicatedContent> content = null;

            if (filterBy == ContentFilterEnum.All)
            {
                content = syndicatedContentService.GetTrendingTopics(userId, countryId,
                    sortBy, pageToken);
            }
            else
            {             
                content = syndicatedContentService.GetFilteredTrendingTopics(countryId, userId, 
                  filterBy, sortBy, pageToken);
            }

            var socailLink = AbstractBaseSocialController.GetSocialIntegrationModel(userId);


            var model = new ViewSyndicatedContentModel
                        {
                            SocialLinked =
                                socailLink.FacebookProfile != null ||
                                socailLink.TwitterProfile != null,
                            Content = new List<SyndicatedContentModel>(),
                            Skip = skip.Value + take.Value,
                            Take = take.Value,
                            ShowHidden = true
                        };
            model.ShowShared = (filterBy != ContentFilterEnum.Hidden && model.SocialLinked);
            if (skip.Value > 0)
            {
                model.IsFirstPage = false;
            }

            foreach (SyndicatedContent item in content)
            {
                model.Content.Add(new SyndicatedContentModel(item, filterBy, model.ShowShared));
            }

            model.CurrentView = GetCurrentView(filterBy);

            return model;
        }


        private string GetCurrentView(ContentFilterEnum filterBy)
        {
            switch (filterBy)
            {
                case ContentFilterEnum.GenericShare:
                    return Resource.BravoVets_TrendingTopics_Shared;
                case ContentFilterEnum.Favorites:
                    return Resource.BravoVets_ResourceTopics_Favorites;
                case ContentFilterEnum.Hidden:
                    return Resource.BravoVets_TrendingTopics_Hidden;
                default:
                    return Resource.BravoVets_ResourceTopics_All;
            }

        }

        public ActionResult MarkContentAsFavorite(int syndicatedContentId, bool isFavorite)
        {
            var userId = GetCurrentUserId();

            if (isFavorite)
            {
                syndicatedContentService.FavoriteContent(userId, syndicatedContentId);
                return Json(new TrendingTopicsMarkFavoriteModel { IsFavorite = true });
            }


            syndicatedContentService.UnfavoriteContent(userId, syndicatedContentId);
            return Json(new TrendingTopicsMarkFavoriteModel { IsFavorite = false });
        }

        public ActionResult HideContent(int syndicatedContentId, bool hide)
        {
            var userId = GetCurrentUserId();

            if (hide)
            {
                syndicatedContentService.HideContent(userId, syndicatedContentId);
                return Json(new TrendintTopicsHideModel { IsHidden = true });
            }


            syndicatedContentService.UnHideContent(userId, syndicatedContentId);
            return Json(new TrendintTopicsHideModel { IsHidden = false });
        }



        public ActionResult TrendingTopicItems(ContentSortEnum sortBy, ContentFilterEnum filterBy, int? skip, int? take)
        {
            var model = GetTrendingTopics(sortBy, filterBy, skip, take);
            return PartialView("TrendingTopicItems", model);
        }

        public ActionResult ResourceTopics()
        {
            try
            {
                ViewBag.Title = Resource.BravoVets_BravectoResources_Title;
                ClaimsPrincipalPermission.CheckAccess("BravoVets", "AcceptedToc");

                var model = GetResourceTopics(ContentSortEnum.ContentDate, ContentFilterEnum.All, 0, 5);
                return View("ResourceTopics", model);
            }
            catch (Exception ex)
            {
                return TermsAndConditions();
            }
        }


        public ActionResult ResourceTopicsItems(ContentSortEnum sortBy, ContentFilterEnum filterBy, int? skip, int? take)
        {
            var model = GetResourceTopics(sortBy, filterBy, skip, take);
            return PartialView("ResourceTopicsItems", model);
        }

        private ViewSyndicatedContentModel GetResourceTopics(ContentSortEnum sortBy, ContentFilterEnum filterBy, int? skip, int? take)
        {

            int userId = GetCurrentUserId();
            int countryId = GetCountryId();

            if (!skip.HasValue)
                skip = 0;

            if (!take.HasValue)
                take = 5;

            var pageToken = new PagingToken { StartRecord = skip.Value, TotalRecords = take.Value };

            List<SyndicatedContent> content = null;
            if (filterBy == ContentFilterEnum.All)
            {

                content = syndicatedContentService.GetBravectoResources(userId, countryId, sortBy, pageToken);
            }
            else
            {
                //GetFilteredBravectoResources(int bravoVetsCountryId, int bravoVetsUserId, ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken)
                content = syndicatedContentService.GetFilteredBravectoResources(countryId, userId, filterBy, sortBy,
                    pageToken);
            }

            var model = new ViewSyndicatedContentModel { ShowHidden = true, SocialLinked = false, Content = new List<SyndicatedContentModel>(), Skip = skip.Value + take.Value, Take = take.Value };
            if (skip.Value > 0)
            {
                model.IsFirstPage = false;
            }

            foreach (SyndicatedContent item in content)
            {
                model.Content.Add(new SyndicatedContentModel(item, filterBy, model.SocialLinked));
            }

            model.CurrentView = GetCurrentView(filterBy);



            return model;
        }


        public ActionResult SocialTips()
        {
            try
            {
                ViewBag.Title = Resource.BravoVets_SocialTips_Title;
                ClaimsPrincipalPermission.CheckAccess("BravoVets", "AcceptedToc");

                var model = GetSocialTips(ContentSortEnum.ContentDate, ContentFilterEnum.All, 0, 5);
                return View("SocialTips", model);
            }
            catch (Exception ex)
            {
                return TermsAndConditions();
            }
        }

        public ActionResult SocialTipItems(ContentSortEnum sortBy, ContentFilterEnum filterBy, int? skip, int? take)
        {
            var model = GetSocialTips(sortBy, filterBy, skip, take);
            return PartialView("SocialTipItems", model);
        }

        private ViewSyndicatedContentModel GetSocialTips(ContentSortEnum sortBy, ContentFilterEnum filterBy, int? skip, int? take)
        {

            int userId = GetCurrentUserId();
            int countryId = GetCountryId();

            if (!skip.HasValue)
                skip = 0;

            if (!take.HasValue)
                take = 5;

            var pageToken = new PagingToken { StartRecord = skip.Value, TotalRecords = take.Value };

            List<SyndicatedContent> content = null;
            if (filterBy == ContentFilterEnum.All)
            {

                content = syndicatedContentService.GetSocialTips(userId, countryId, sortBy, pageToken);
            }
            else
            {
                // GetFilteredSocialTips(int bravoVetsCountryId, int bravoVetsUserId, ContentFilterEnum filter, ContentSortEnum sort, PagingToken pagingToken)
                content = syndicatedContentService.GetFilteredSocialTips(countryId, userId, filterBy, sortBy, pageToken);
            }

            var model = new ViewSyndicatedContentModel { SocialLinked = false, Content = new List<SyndicatedContentModel>(), Skip = skip.Value + take.Value, Take = take.Value };
            if (skip.Value > 0)
            {
                model.IsFirstPage = false;
            }

            foreach (SyndicatedContent item in content)
            {
                model.Content.Add(new SyndicatedContentModel(item, model.SocialLinked));
            }

            model.CurrentView = GetCurrentView(filterBy);

            return model;
        }

        public ActionResult EditProfile()
        {
            ViewBag.Title = Resource.BravoVets_Profile_Title;
            var user = this.GetCurrentUserForEdit();
            ILookupDomainService lookupManager = new LookupDomainService();
            ViewBag.Countries = lookupManager.GetBravoVetsCountries();
            return View("Profile", user);
        }

        [HttpPost]
        public ActionResult EditProfile(BravoVetsUser bvUser)
        {
            return this.SerializeProfileEdits(bvUser, false);
        }


        public ActionResult EditProfileAndFacility(int? id)
        {
            ViewBag.Title = Resource.BravoVets_EditProfile_Title;
            BravoVetsUser user;
            user = id.HasValue ? this.GetCurrentUserEditableFacility(id.Value) : this.GetCurrentUserForEdit();
            user.Veterinarian.CanEditFacilities = true;
            ILookupDomainService lookupManager = new LookupDomainService();
            ViewBag.Countries = lookupManager.GetBravoVetsCountries();
            return View("Profile", user);
        }

        [HttpPost]
        public ActionResult EditProfileAndFacility(BravoVetsUser bvUser)
        {
            return this.SerializeProfileEdits(bvUser, true);
        }

        public RedirectResult DeleteFacility(int id)
        {
            var vetService = new VeterinarianDomainService();
            vetService.DeleteVeterinarianFacility(id);
            return this.Redirect(string.Format("/EditProfileAndFacility/"));
        }



        #region private methods

        private ActionResult SerializeProfileEdits(BravoVetsUser bvUser, bool showFacilities)
        {
            var passedVetValidation = this.ValidateVetValues(bvUser);
            if (passedVetValidation && this.UpdateBravoVetsUserFromProfile(bvUser))
            {
                return RedirectToAction("Dashboard");
            }
            else if (showFacilities)
            {
                int id = bvUser.Veterinarian.EditableFacility.VeterinarianFacilityId;
                BravoVetsUser user;
                user = id > 0 ? this.GetCurrentUserEditableFacility(id) : this.GetCurrentUserForEdit();
                user.Veterinarian.CanEditFacilities = true;
                user.Veterinarian.EditableFacility.IsEditable = true;
                ILookupDomainService lookupManager = new LookupDomainService();
                ViewBag.Countries = lookupManager.GetBravoVetsCountries();
                return View("Profile", user);
            }
            else
            {
                var user = this.GetCurrentUserForEdit();
                return View("Profile", user);
            }
        }

        private bool UpdateBravoVetsUserFromProfile(BravoVetsUser bvUser)
        {
            bool passedValidation = true;

            var veterinarianFacility = bvUser.Veterinarian.EditableFacility;

            // skip this, if there is no text in the facility name
            if (veterinarianFacility != null)
            {
                if (string.IsNullOrEmpty(veterinarianFacility.FacilityName))
                {
                    bvUser.Veterinarian.EditableFacility.IsEditable = false;
                }
                else
                {
                    passedValidation = this.ValidateFacilityValues(bvUser);
                }
            }

            if (!passedValidation)
            {
                return false;
            }

            var userDomainService = new BravoVetsUserDomainService();
            var newUser = userDomainService.UpdateUserFromProfile(bvUser);
            return true;

            //BravoVetsUser user = userDomainService.GetBravoVetsUserForProfileEdit(bvUser.BravoVetsUserId);
            //return user;
        }

        private BravoVetsUser GetCurrentUserForEdit()
        {
            var userId = this.GetCurrentUserId();
            var userDomainService = new BravoVetsUserDomainService();
            var user = userDomainService.GetBravoVetsUserForProfileEdit(userId);

            return user;
        }

        private BravoVetsUser GetCurrentUserEditableFacility(int facilityId)
        {
            var userId = this.GetCurrentUserId();
            var userDomainService = new BravoVetsUserDomainService();
            var user = userDomainService.GetBravoVetsUserForProfileEdit(userId, facilityId);
            return user;
        }

        private bool ValidateVetValues(BravoVetsUser bvUser)
        {
            if (string.IsNullOrEmpty(bvUser.Veterinarian.BusinessName))
            {
                ModelState.AddModelError("Veterinarian.BusinessName", Resource.ValueIsRequired);
            }
            else if (bvUser.Veterinarian.BusinessName.Length > 255)
            {
                ModelState.AddModelError("Veterinarian.BusinessName", Resource.ValueIsTooLong);
            }

            return this.ModelState.IsValid;
        }

        private bool ValidateFacilityValues(BravoVetsUser bvUser)
        {
            var vf = bvUser.Veterinarian.EditableFacility;

            if (vf.FacilityName.Length > 255)
            {
                ModelState.AddModelError("Veterinarian.EditableFacility.FacilityName", Resource.ValueIsTooLong);
            }


            return this.ModelState.IsValid;
        }

        #endregion

    }
}