using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using log4net;
using ProfSite.Models;
using ProfSite.Resources;
using ProfSite.Utils;

namespace ProfSite.Controllers
{
    using System.Globalization;

    using Newtonsoft.Json;

    public class ShareSocialContentController : AbstractBaseSocialController
    {

        protected readonly ILog Logger = LogManager.GetLogger(typeof(ShareSocialContentController));




        public ActionResult GetQueuedSocialPostDialog(ShowSocialDialogModel data)
        {
            var model = new SocialDialogModel();
            model.ClientOffset = data.clientOffset;
            var queuedContent = publishQueueDomainService.GetQueuedMessage(data.id.Value);

            model.ContentModel = new SocialDialogContentModel(queuedContent)
                                 {
                                     SiteLanguage = GetSiteLanguage(),
                                     Id = queuedContent.QueueContentId                                     
                                 };
            InitSocialDialogModel(queuedContent, model,true);

            if (!queuedContent.IsPublished)
            {
                model.ShowDelete = true;
            }

            return PartialView("ShareSocialContent", model);
        }

        public ActionResult GetSyndicatedSocialPostDialog(ShowSocialDialogModel data)
        {
            int userId = GetCurrentUserId();
            var socialLinks = GetSocialIntegrationModel(userId);

            var model = new SocialDialogModel
                        {
                            Header = Resource.BravoVets_ShareDialog_SyndicatedHeader,
                            HeaderClass = "",
                            PostText = Resource.BravoVets_ShareDialog_SyndicatedHeader,
                            EnableFacebook = socialLinks.FacebookProfile != null,
                            EnableTwitter = socialLinks.TwitterProfile != null,
                            ShowShareOptions = true,
                            ClientOffset = data.clientOffset
                        };

            SyndicatedContent content = syndicatedContentService.GetSyndicatedContent(userId, data.id.Value);

            model.ContentModel = new SocialDialogContentModel(content) { SiteLanguage = GetSiteLanguage() };

            return PartialView("ShareSocialContent", model);
        }

        public ActionResult GetSocialPostDialog(ShowSocialDialogModel data)
        {
            int userId = GetCurrentUserId();

            var model = new SocialDialogModel { ContentModel = new SocialDialogContentModel { SiteLanguage = GetSiteLanguage() } };

            model.ClientOffset = data.clientOffset;

            var queuedContent = publishQueueDomainService.GetMinimalEmptyQueueContent(LanguageHelper.GetCountryId(GetSiteLanguage()), userId);

            queuedContent.PublishDateUtc = DateTime.UtcNow;
            queuedContent.BravoVetsStatusId = (int)BravoVetsStatusEnum.InProcess;
            queuedContent.IsPublished = false;
            queuedContent.PlatformPublishId = data.dialogType == "facebook"
                ? (int)SocialPlatformEnum.Facebook
                : (int)SocialPlatformEnum.Twitter;


            if (queuedContent.PlatformPublishId == (int)SocialPlatformEnum.Facebook)
            {
                var fbSocialIntegration = GetFacebookVetSocialIntegration();
                queuedContent.AccessCode = FacebookHelper.GetAccessToken(fbSocialIntegration);
                queuedContent.AccountName = FacebookHelper.GetId(fbSocialIntegration);
            }
            else
            {
                var twitSocialIntegration = GetTwitterVetSocialIntegration();
                queuedContent.AccessCode = twitSocialIntegration.AccessCode;
                queuedContent.AccountName = twitSocialIntegration.AccessToken;
            }



            publishQueueDomainService.QueueContentForPublish(queuedContent);

            model.ContentModel.Id = queuedContent.QueueContentId;
            model.ContentModel.PublishDate = queuedContent.PublishDateUtc;
            InitSocialDialogModel(queuedContent, model,false);


            return PartialView("ShareSocialContent", model);
        }

        private static void InitSocialDialogModel(QueueContent queuedContent, SocialDialogModel model,bool isEdit)
        {
            switch (queuedContent.PlatformPublishId)
            {
                case (int)SocialPlatformEnum.Facebook:
                    model.Header = isEdit ? Resource.BravoVets_ShareDialog_Facebook_EditHeader : Resource.BravoVets_ShareDialog_Facebook_Header;
                    model.HeaderClass = "facebook";

                    model.PostText = queuedContent.IsPublished ? Resource.BravoVets_ShareDialog_Facebook_Repost : Resource.BravoVets_ShareDialog_Facebook_Post;

                    break;
                case (int)SocialPlatformEnum.Twitter:
                    model.Header = isEdit ? Resource.BravoVets_ShareDialog_Twitter_EditHeader : Resource.BravoVets_ShareDialog_Twitter_Header;
                    model.HeaderClass = "twitter";
                    model.PostText = queuedContent.IsPublished ? Resource.BravoVets_ShareDialog_Twitter_Retweet : Resource.BravoVets_ShareDialog_Twitter_Header;

                    //model.EnableAttachUrl = false;
                    break;
            }


            if (queuedContent.QueueContentAttachments == null) return;

            foreach (var queueContentAttachment in queuedContent.QueueContentAttachments)
            {
                model.ContentModel.Attachments.Add(new SocialDialogAttachmentModel
                                                   {
                                                       Url = "/syndicatedcontent/queuedcontent/" + queueContentAttachment.QueueContentAttachmentId,
                                                       DeleteUrl = "/sharesocial/DeleteFile?queuedContentId=" + queueContentAttachment.QueueContentAttachmentId,
                                                       AttachmentId = queueContentAttachment.QueueContentAttachmentId,
                                                       FileName = queueContentAttachment.AttachmentFileName,
                                                      
                                                   });
            }
        }


        public ActionResult DeleteQueuedContent(int queuedContentId)
        {
            try
            {
                publishQueueDomainService.DequeueContent(queuedContentId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new EmptyResult();
        }

        public ActionResult DeleteFile(int queuedContentId)
        {
            try
            {
                publishQueueDomainService.DeleteAttachment(queuedContentId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new EmptyResult();
        }

        public ActionResult AttachUrl(int queuedContentId, string url)
        {
            var queuedContent = publishQueueDomainService.GetQueuedMessage(queuedContentId);
            queuedContent.LinkUrl = url;
            queuedContent.SyndicatedContentPostTypeId =(int) SyndicatedContentPostTypeEnum.LinkPostPage;
            publishQueueDomainService.QueueContentForPublish(queuedContent);
            return new EmptyResult();
        }

        public ActionResult DeleteUrl(int queuedContentId)
        {
            var queuedContent = publishQueueDomainService.GetQueuedMessage(queuedContentId);
            queuedContent.LinkUrl = string.Empty;
            publishQueueDomainService.QueueContentForPublish(queuedContent);
            return new EmptyResult();
        }


        public ActionResult UploadFile(int? queuedContentId)
        {

            var queuedContent = publishQueueDomainService.GetQueuedMessage(queuedContentId.Value);
            queuedContent.SyndicatedContentPostTypeId = (int)SyndicatedContentPostTypeEnum.ImagePost;
            publishQueueDomainService.QueueContentForPublish(queuedContent);

            var results = new UploadFilesResults
                          {
                              files = new List<ViewDataUploadFilesResult>(),
                              queuedcontentid = queuedContentId.Value
                          };
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);
                var fileNameEncoded = HttpUtility.HtmlEncode(fileName);
                var attachment = new QueueContentAttachment
                                   {
                                       AttachmentExtension = file.ContentType,
                                       AttachmentFileName = fileNameEncoded,
                                       QueueContentId = queuedContentId.Value,
                                       AttachmentFile = GetFile(file),
                                   };
                publishQueueDomainService.AddNewAttachments(new List<QueueContentAttachment> { attachment });
                results.files.Add(new ViewDataUploadFilesResult()
                {
                    url = "/syndicatedcontent/queuedcontent/" + attachment.QueueContentAttachmentId,
                    thumbnail_url = "/syndicatedcontent/queuedcontent/" + +attachment.QueueContentAttachmentId,
                    name = fileNameEncoded,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "/sharesocial/DeleteFile?queuedContentId=" + attachment.QueueContentAttachmentId,
                    delete_type = "DELETE",
                    attachmentId = attachment.QueueContentAttachmentId
                });
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var response = new ContentResult
            {
                Content = serializer.Serialize(results)
            };
            return response;

        }

        private byte[] GetFile(HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        public ActionResult CloneQueuedContent(int contentId, string message, bool postNow, DateTime? postDate,int clientOffset)
        {
            int userId = GetCurrentUserId();

            var clonedQueuedContent = publishQueueDomainService.CloneQueuedContent(contentId);
            
            UpdateQueuedContent(clonedQueuedContent, message, postNow, postDate, userId,clientOffset);
            
            return new EmptyResult();
        }


        public ActionResult UpdateQueuedContent(int contentId, string message, bool postNow, DateTime? postDate, int clientOffset)
        {
            int userId = GetCurrentUserId();
            var queuedContent = publishQueueDomainService.GetQueuedMessage(contentId);

            UpdateQueuedContent(queuedContent, message, postNow, postDate, userId,clientOffset);

            return new EmptyResult();
        }

        private void UpdateQueuedContent(QueueContent queuedContent, string message, bool postNow, DateTime? postDate, 
            int userId,int clientOffSet)
        {
            queuedContent.ContentText = message;
            queuedContent.BravoVetsStatusId = (int) BravoVetsStatusEnum.Active;

            if (postNow)
            {
                QueueContentHelper.DeliverQueuedContent(queuedContent);
            }
            else
            {
                if (!postDate.HasValue)
                {
                    postDate = DateTime.Now.AddDays(1);
                }
                queuedContent.PublishDateUtc = postDate.Value.AddMinutes(clientOffSet);
                if (queuedContent.IsPublished)
                {
                    queuedContent.IsPublished = false;
                }
            }

            publishQueueDomainService.QueueContentForPublish(queuedContent);

            if (queuedContent.SyndicatedContentId.HasValue)
            {
                syndicatedContentService.ShareContent(userId, queuedContent.SyndicatedContentId.Value,
                    (SocialPlatformEnum) queuedContent.PlatformPublishId);
            }
        }


        public ActionResult QueueSyndicatedContent(int contentId, string message, bool postToTwitter, bool postToFacebook, bool postNow, DateTime? postDate,int clientOffSet)
        {
            try
            {
                int userId = GetCurrentUserId();

                if (postToFacebook)
                {
                    var fbQueuedContent = CreateQueueContent(userId, contentId, message, SocialPlatformEnum.Facebook, postDate,clientOffSet);
                    if (postNow)
                    {
                        QueueContentHelper.DeliverQueuedContent(fbQueuedContent);
                    }
                    publishQueueDomainService.QueueContentForPublish(fbQueuedContent);
                    syndicatedContentService.ShareContent(userId, fbQueuedContent.SyndicatedContentId.Value, SocialPlatformEnum.Facebook);
                }
                if (postToTwitter)
                {
                    var twQueuedContent = CreateQueueContent(userId, contentId, message, SocialPlatformEnum.Twitter, postDate, clientOffSet);
                    if (postNow)
                    {
                        QueueContentHelper.DeliverQueuedContent(twQueuedContent);
                    }

                    publishQueueDomainService.QueueContentForPublish(twQueuedContent);
                    syndicatedContentService.ShareContent(userId, twQueuedContent.SyndicatedContentId.Value, SocialPlatformEnum.Twitter);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new EmptyResult();
        }

        private QueueContent CreateQueueContent(int userId, int syndicatedContentId, string message, SocialPlatformEnum platform, DateTime? publishDate,int clientOffSet)
        {
            if (!publishDate.HasValue)
            {
                publishDate = DateTime.UtcNow;
            }

            var queuedContent = publishQueueDomainService.GetMinimalEmptyQueueContent(LanguageHelper.GetCountryId(GetSiteLanguage()), userId);
            queuedContent.PublishDateUtc = publishDate.Value.AddMinutes(clientOffSet);
            queuedContent.ContentText = message;
            queuedContent.IsPublished = false;
            queuedContent.PlatformPublishId = (int)platform;
            queuedContent.BravoVetsStatusId = (int)BravoVetsStatusEnum.Active;
            queuedContent.SyndicatedContentId = syndicatedContentId;
            publishQueueDomainService.QueueContentForPublish(queuedContent);

            var syndicatedContent = syndicatedContentService.GetSyndicatedContent(userId, syndicatedContentId);
            if (syndicatedContent != null)
            {
                queuedContent.LinkUrl = syndicatedContent.LinkUrl;
            }

            //no longer publishing images from syndicated content
            //  publishQueueDomainService.AssociateSyndicatedContentAttachments(queuedContent.QueueContentId, syndicatedContentId);

            var fullQueuedContent = publishQueueDomainService.GetQueuedMessage(queuedContent.QueueContentId);

            if (fullQueuedContent.PlatformPublishId == (int)SocialPlatformEnum.Facebook)
            {
                var fbSocialIntegration = GetFacebookVetSocialIntegration();
                fullQueuedContent.AccessCode = FacebookHelper.GetAccessToken(fbSocialIntegration);
                fullQueuedContent.AccountName = FacebookHelper.GetId(fbSocialIntegration);
            }
            else
            {
                var twitSocialIntegration = GetTwitterVetSocialIntegration();
                fullQueuedContent.AccessCode = twitSocialIntegration.AccessCode;
                fullQueuedContent.AccountName = twitSocialIntegration.AccessToken;
            }

           

            return fullQueuedContent;
        }




        public ActionResult SocialCalendar()
        {
            var menu = BravectoMenu.CreateBravovetsMenu("socialcalendar");
            ViewBag.Menu = menu;
            ViewBag.Title = Resource.BravoVets_SocialCalendar_Title;

            ViewBag.Language = this.GetSiteFullLanguage();

            return View();
        }


        //[JsonRequestBehavior(allowget)]
        public ActionResult SocialCalendarPosts(string @from, string to)
        {
            DateTime startDate;
            DateTime endDate;

            double startDateMilliseconds;
            double endDateMilliseconds;

            if (string.IsNullOrEmpty(@from)) 
            {
                startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            }
            else if (Double.TryParse(@from, out startDateMilliseconds))
            {
                startDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(startDateMilliseconds);
            }
            else
            {
                startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            }

            if (string.IsNullOrEmpty(to))
            {
                endDate = startDate.AddMonths(1).AddSeconds(-1);
            }
            else if (Double.TryParse(to, out endDateMilliseconds))
            {
                endDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(endDateMilliseconds);
            }
            else
            {
                endDate = startDate.AddMonths(1).AddSeconds(-1);
            }

            int userId = GetCurrentUserId();
            var resultList = QueueContentHelper.GetCalendarPostsByUserByDate(userId, startDate, endDate);

            var model = new SocialCalendarPostsModel()
                        {
                            success = 1,
                            result = resultList
                        };


            return Content(JsonConvert.SerializeObject(model), "application/json");
            //return Json(model, JsonRequestBehavior.AllowGet);
        }

        public void DeliverQueuedMessages()
        {
            // Todo: Decide whether any JSON should be returned.  This method would mostly be called by the windows service
            QueueContentHelper.DeliverEligibleQueuedMessages();
        }

    }
}