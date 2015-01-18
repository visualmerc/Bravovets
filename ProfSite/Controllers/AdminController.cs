using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainObject.Infrastructure;
using BravoVets.DomainService.Contract;
using BravoVets.DomainService.Service;
using ProfSite.Models;
using ProfSite.Resources;

namespace ProfSite.Controllers
{
    public class AdminController : AbstractBaseController
    {
        private readonly ISyndicatedContentAdminService adminService;

        public AdminController()
        {
            adminService = new SyndicatedContentAdminService();
        }

        public ActionResult Dashboard()
        {
            return View("Dashboard");
        }

        public ActionResult BravectoResources(string activeTab)
        {
            var model = new AdminManagePostsModel
                        {
                            Header = Resource.Admin_Nav_BravectoResources,
                            GetUrl = "/admin/posts/bravectoresources",
                            ActiveTab = string.IsNullOrEmpty(activeTab) ? "future" : activeTab
                        };


            return View("ManagePosts", model);
        }

        public ActionResult BravectoResourcePosts(string type, ContentSortEnum sortBy, int page, int clientOffset)
        {
            bool isFuture = type != "past";

            var model = new AdminManagePostsModel
                        {
                            Header = Resource.Admin_Nav_BravectoResources,
                            GetUrl = "/admin/posts/bravectoresources",
                            EditUrl = string.Format("/admin/bravectoresources/{0}/edit/", clientOffset),
                            ClientOffset = clientOffset,
                        };

            int countryId = GetCountryId();
            int recCount = 0;

            int start = (page)*10;

            model.Items = adminService.GetBravectoResourcesAdminList(isFuture, (BravoVetsCountryEnum) countryId,
                sortBy, new PagingToken {StartRecord = start, TotalRecords = 10}, out recCount);


            model.PageCount = Math.Ceiling((decimal) recCount/10);

            return PartialView("ManagePostItems", model);
        }

        
        public ActionResult BravectoResourcePreview(AdminSavePostModel data)
        {
            return View("ManagePostPreview",data);
        }

        [HttpPost]
        public ActionResult BravectoResourcePosts(AdminSavePostModel post)
        {
            SyndicatedContent syndicatedContent = null;

            if (post.syndicatedContentId.HasValue)
            {
                syndicatedContent = adminService.GetSyndicatedContent(post.syndicatedContentId.Value);
            }
            else
            {
                syndicatedContent =
                    adminService.GetMinimalEmptySyndicatedContent(SyndicatedContentTypeEnum.BravectoResources,
                        (BravoVetsCountryEnum) GetCountryId(), GetCurrentUserName());
                
            }
            syndicatedContent.BravoVetsStatusId = (int)BravoVetsStatusEnum.Active;

            syndicatedContent.ContentText = post.contentText;
            syndicatedContent.Title = string.IsNullOrEmpty(post.title) ? string.Empty : post.title;
            if (post.postNow)
            {
                syndicatedContent.PublishDateUtc = DateTime.UtcNow;
            }
            else if (post.publishDate.HasValue)
            {
                syndicatedContent.PublishDateUtc = post.publishDate.Value.AddMinutes(post.clientOffset);
            }


            SyndicatedContent savedSyndicatedContent = null;
            if (!post.syndicatedContentId.HasValue)
            {
                savedSyndicatedContent = adminService.QueueNewBravectoResource(syndicatedContent);
            }
            else
            {
                savedSyndicatedContent = adminService.UpdateSyndicatedContent(syndicatedContent);
            }

            UpdateFeaturedContent(post, savedSyndicatedContent, syndicatedContent);

            if (post.attachments != null)
            {
                var contentLinks = new List<SyndicatedContentLink>();
                foreach (AdminPostAttachment attachment in post.attachments)
                {
                    if (attachment.isDeleted)
                    {
                        if (!attachment.id.HasValue)
                        {
                            continue;                            
                        }
                        if (attachment.type == "link")
                        {
                            adminService.DeleteSyndicatedContentLink(attachment.id.Value);
                        }
                        else
                        {
                            adminService.DeleteSyndicatedContentAttachment(attachment.id.Value);
                        }
                        continue;
                    }
                    if (attachment.id.HasValue)
                    {
                        continue;
                    }
                    if (attachment.type == "link")
                    {
                        var contentLink = new SyndicatedContentLink
                                          {
                                              LinkTitle = attachment.name,
                                              LinkUrl = attachment.url,
                                              SyndicatedContentId = savedSyndicatedContent.SyndicatedContentId,
                                              ModifiedDateUtc = DateTime.UtcNow,
                                              CreateDateUtc = DateTime.UtcNow,
                                          };
                        contentLinks.Add(contentLink);
                    }
                }
                adminService.SaveContentLinks(contentLinks);
            }

            var results = new AdminSavePostResultsModel
                          {
                              syndicatedContentId =
                                  savedSyndicatedContent.SyndicatedContentId
                          };
            return Json(results);
        }

        private void UpdateFeaturedContent(AdminSavePostModel post, SyndicatedContent savedSyndicatedContent,
            SyndicatedContent syndicatedContent)
        {
            if (post.featuredContentId.HasValue)
            {
                if (post.featuredContentId > -1)
                {
                    if (savedSyndicatedContent.SyndicatedContentAttachments == null)
                    {
                        adminService.AssociateFeaturedContent(post.featuredContentId.Value,
                            savedSyndicatedContent.SyndicatedContentId);
                    }
                    else
                    {
                        SyndicatedContentAttachment featuredContent =
                            syndicatedContent.SyndicatedContentAttachments.FirstOrDefault(x => x.DisplayInUi);
                        if (featuredContent != null)
                        {
                            adminService.DeleteSyndicatedContentAttachment(featuredContent.SyndicatedContentAttachmentId);
                        }
                        adminService.AssociateFeaturedContent(post.featuredContentId.Value,
                            savedSyndicatedContent.SyndicatedContentId);
                    }
                }
            }
            else
            {
                if (syndicatedContent.SyndicatedContentAttachments != null)
                {
                    SyndicatedContentAttachment featuredContent =
                        syndicatedContent.SyndicatedContentAttachments.FirstOrDefault(x => x.DisplayInUi);
                    if (featuredContent != null)
                    {
                        adminService.DeleteSyndicatedContentAttachment(featuredContent.SyndicatedContentAttachmentId);
                    }
                }
            }
        }


        [HttpDelete]
        public ActionResult BravectoResourcePosts(int id)
        {
            adminService.DeleteSyndicatedContent(id);
            return new EmptyResult();
        }

        public ActionResult BravectoResourcesEdit(int? id, int clientOffset)
        {
            var model = new AdminEditPostModel
                        {
                            Header = Resource.Admin_EditPost_BravectoResources_HeaderText,
                            Id = id,
                            TwitterValidation = false,
                            AllowRichText = true,
                            AllowFileAttachments = true,
                            NumberOfAttachments = 5,
                            ContentType = SyndicatedContentTypeEnum.BravectoResources,
                            HeaderText = Resource.Admin_EditPost_BravectoHeaderCopy,
                            ClientOffset = clientOffset,
                            ListUrl = "/admin/bravectoresources",
                            PreviewUrl = "/adminpost/bravectoresources/preview",
                            ScheduleTooltip = Resource.Admin_EditPost_Schedule_Tooltip,
                            FeaturedImageTooltip = Resource.Admin_EditPost_FeaturedImage_Tooltip,
                            AttachmentTooltip = Resource.Admin_EditPost_Bravecto_Attachment_Tooltip
                        };
            if (id.HasValue)
            {
                model.DeleteUrl = "/admin/posts/bravectoresources?id=" + id.Value;
                model.PostUrl = "/adminpost/posts/bravectoresources?id=" + id.Value;
                model.AllowDelete = true;
            }
            else
            {
                model.PostUrl = "/adminpost/posts/bravectoresources";
                model.AllowDelete = false;
            }
            PopulateModelWithContent(id, model, SyndicatedContentTypeEnum.BravectoResources);

            return View("ManagePostsEdit", model);
        }

        public ActionResult SocialTips(string activeTab)
        {
            var model = new AdminManagePostsModel
                        {
                            Header = Resource.Admin_Nav_SocialTips,
                            GetUrl = "/admin/posts/socialtips",
                            ActiveTab = string.IsNullOrEmpty(activeTab) ? "future" : activeTab
                        };

            return View("ManagePosts", model);
        }

        public ActionResult SocialTipPosts(string type, ContentSortEnum sortBy, int page, int clientOffset)
        {
            bool isFuture = type != "past";

            var model = new AdminManagePostsModel
                        {
                            Header = Resource.Admin_Nav_SocialTips,
                            GetUrl = "/admin/posts/socialtips",
                            EditUrl = string.Format("/admin/socialtips/{0}/edit/", clientOffset),
                            ClientOffset = clientOffset
                        };

            int countryId = GetCountryId();
            int recCount = 0;

            int start = (page)*10;

            model.Items = adminService.GetSocialTipsAdminList(isFuture, (BravoVetsCountryEnum) countryId,
                sortBy, new PagingToken {StartRecord = start, TotalRecords = 10}, out recCount);


            model.PageCount = Math.Ceiling((decimal) recCount/10);

            return PartialView("ManagePostItems", model);
        }

        [HttpDelete]
        public ActionResult SocialTipPosts(int id)
        {
            adminService.DeleteSyndicatedContent(id);
            return new EmptyResult();
        }

        public ActionResult SocialTipPreview(AdminSavePostModel data)
        {
            return View("ManagePostPreview", data);
        }

        [HttpPost]
        public ActionResult SocialTipPosts(AdminSavePostModel post)
        {
            SyndicatedContent syndicatedContent = null;

            if (post.syndicatedContentId.HasValue)
            {
                syndicatedContent = adminService.GetSyndicatedContent(post.syndicatedContentId.Value);
            }
            else
            {
                syndicatedContent =
                    adminService.GetMinimalEmptySyndicatedContent(SyndicatedContentTypeEnum.SocialTips,
                        (BravoVetsCountryEnum)GetCountryId(), GetCurrentUserName());
            }
            syndicatedContent.BravoVetsStatusId = (int)BravoVetsStatusEnum.Active;

            syndicatedContent.ContentText = post.contentText;
            syndicatedContent.Title = post.title;
            if (post.postNow)
            {
                syndicatedContent.PublishDateUtc = DateTime.UtcNow;
            }
            else if (post.publishDate.HasValue)
            {
                syndicatedContent.PublishDateUtc = post.publishDate.Value.AddMinutes(post.clientOffset);
            }


            SyndicatedContent savedSyndicatedContent = null;
            if (!post.syndicatedContentId.HasValue)
            {
                savedSyndicatedContent = adminService.QueueNewSocialTip(syndicatedContent);
            }
            else
            {
                savedSyndicatedContent = adminService.UpdateSyndicatedContent(syndicatedContent);
            }

            UpdateFeaturedContent(post, savedSyndicatedContent, syndicatedContent);

            if (post.attachments != null)
            {
                var contentLinks = new List<SyndicatedContentLink>();
                foreach (AdminPostAttachment attachment in post.attachments)
                {
                    if (attachment.isDeleted)
                    {
                        if (!attachment.id.HasValue)
                        {
                            continue;
                        }
                        if (attachment.type == "link")
                        {
                            adminService.DeleteSyndicatedContentLink(attachment.id.Value);
                        }
                        else
                        {
                            adminService.DeleteSyndicatedContentAttachment(attachment.id.Value);
                        }
                        continue;
                    }
                    if (attachment.id.HasValue)
                    {
                        continue;
                    }
                    if (attachment.type == "link")
                    {
                        var contentLink = new SyndicatedContentLink
                        {
                            LinkTitle = attachment.name,
                            LinkUrl = attachment.url,
                            SyndicatedContentId = savedSyndicatedContent.SyndicatedContentId,
                            ModifiedDateUtc = DateTime.UtcNow,
                            CreateDateUtc = DateTime.UtcNow,
                        };
                        contentLinks.Add(contentLink);
                    }
                }
                adminService.SaveContentLinks(contentLinks);
            }

            var results = new AdminSavePostResultsModel
            {
                syndicatedContentId =
                    savedSyndicatedContent.SyndicatedContentId
            };
            return Json(results);
        }
        public ActionResult SocialTipEdit(int? id, int clientOffset)
        {
           
            var model = new AdminEditPostModel
            {
                Header = Resource.Admin_EditPost_SocialTips_HeaderText,
                Id = id,
                TwitterValidation = false,
                AllowRichText = true,
                AllowFileAttachments = true,
                NumberOfAttachments = 5,
                ContentType = SyndicatedContentTypeEnum.SocialTips,
                HeaderText = Resource.Admin_EditPost_SocialTipsHeaderCopy,
                ClientOffset = clientOffset,
                ListUrl = "/admin/socialtips",
                PreviewUrl = "/adminpost/socialtips/preview",
                ScheduleTooltip = Resource.Admin_EditPost_Schedule_Tooltip,
                FeaturedImageTooltip = Resource.Admin_EditPost_FeaturedImage_Tooltip,
                AttachmentTooltip = Resource.Admin_EditPost_SocialTips_Attachment_Tooltip
            };
            if (id.HasValue)
            {
                model.DeleteUrl = "/admin/posts/socialtips?id=" + id.Value;
                model.PostUrl = "/adminpost/posts/socialtips?id=" + id.Value;
                model.AllowDelete = true;
            }
            else
            {
                model.PostUrl = "/adminpost/posts/socialtips";
                model.AllowDelete = false;
            }
            PopulateModelWithContent(id, model, SyndicatedContentTypeEnum.SocialTips);

            return View("ManagePostsEdit", model);

        }

        public ActionResult TrendingTopicsPosts(string type, ContentSortEnum sortBy, int page, int clientOffset)
        {
            bool isFuture = type != "past";

            var model = new AdminManagePostsModel
                        {
                            Header = Resource.Admin_Nav_TrendingTopics,
                            GetUrl = "/admin/posts/trendingtopics",
                            EditUrl = string.Format("/admin/trendingtopics/{0}/edit/", clientOffset),
                            ClientOffset = clientOffset
                        };

            int countryId = GetCountryId();
            int recCount = 0;

            int start = (page)*10;

            model.Items = adminService.GetTrendingTopicsAdminList(isFuture, (BravoVetsCountryEnum) countryId,
                sortBy, new PagingToken {StartRecord = start, TotalRecords = 10}, out recCount);


            model.PageCount = Math.Ceiling((decimal) recCount/10);

            return PartialView("ManagePostItems", model);
        }

        [HttpDelete]
        public ActionResult TrendingTopicsPosts(int id)
        {
            adminService.DeleteSyndicatedContent(id);
            return new EmptyResult();
        }


        public ActionResult TrendingTopicsPreview(AdminSavePostModel data)
        {
            return View("ManagePostPreview", data);
        }
        [HttpPost]
        public ActionResult TrendingTopicsPosts(AdminSavePostModel post)
        {
            SyndicatedContent syndicatedContent = null;

            if (post.syndicatedContentId.HasValue)
            {
                syndicatedContent = adminService.GetSyndicatedContent(post.syndicatedContentId.Value);
            }
            else
            {
                syndicatedContent =
                    adminService.GetMinimalEmptySyndicatedContent(SyndicatedContentTypeEnum.TrendingTopics,
                        (BravoVetsCountryEnum) GetCountryId(), GetCurrentUserName());
                syndicatedContent.BravoVetsStatusId = (int) BravoVetsStatusEnum.Active;
            }

            syndicatedContent.ContentText = post.contentText;
            syndicatedContent.Title = post.title;
            if (post.postNow)
            {
                syndicatedContent.PublishDateUtc = DateTime.UtcNow;
            }
            else if (post.publishDate.HasValue)
            {
                syndicatedContent.PublishDateUtc = post.publishDate.Value.AddMinutes(post.clientOffset);
            }

            if (post.attachments != null && post.attachments.Count > 0)
            {
                syndicatedContent.LinkUrl = post.attachments[0].url;
                syndicatedContent.LinkUrlName = post.attachments[0].name;
                syndicatedContent.SyndicatedContentPostTypeId = (int) SyndicatedContentPostTypeEnum.LinkPostPage;
            }
            else
            {
                syndicatedContent.LinkUrl = string.Empty;
                syndicatedContent.SyndicatedContentPostTypeId = (int) SyndicatedContentPostTypeEnum.TextOnly;
            }

            SyndicatedContent savedSyndicatedContent = null;
            if (!post.syndicatedContentId.HasValue)
            {
                savedSyndicatedContent = adminService.QueueNewTrendingTopic(syndicatedContent);
            }
            else
            {
                savedSyndicatedContent = adminService.UpdateSyndicatedContent(syndicatedContent);
            }


            UpdateFeaturedContent(post, savedSyndicatedContent, syndicatedContent);

            var results = new AdminSavePostResultsModel
                          {
                              syndicatedContentId =
                                  savedSyndicatedContent.SyndicatedContentId
                          };
            return Json(results);
        }


        public ActionResult TrendingTopicsEdit(int? id, int clientOffset)
        {
            var model = new AdminEditPostModel
                        {
                            Header = Resource.Admin_EditPost_TrendingTopicsHeaderText,
                            Id = id,
                            TwitterValidation = true,
                            AllowRichText = false,
                            AllowFileAttachments = false,
                            NumberOfAttachments = 1,
                            ContentType = SyndicatedContentTypeEnum.TrendingTopics,
                            HeaderText = Resource.Admin_EditPost_TrendingTopicHeaderCopy,
                            ClientOffset = clientOffset,
                            ListUrl = "/admin/trendingtopics",
                            PreviewUrl = "/adminpost/trendingtopics/preview",
                            ScheduleTooltip = Resource.Admin_EditPost_Schedule_Tooltip,
                            FeaturedImageTooltip = Resource.Admin_EditPost_FeaturedImage_Tooltip,
                            AttachmentTooltip = Resource.Admin_EditPost_Trending_Attachment_Tooltip
                        };
            if (id.HasValue)
            {
                model.DeleteUrl = "/admin/posts/trendingtopics?id=" + id.Value;
                model.PostUrl = "/adminpost/posts/trendingtopics?id=" + id.Value;
                model.AllowDelete = true;
            }
            else
            {
                model.PostUrl = "/adminpost/posts/trendingtopics";
                model.AllowDelete = false;
            }
            PopulateModelWithContent(id, model, SyndicatedContentTypeEnum.TrendingTopics);

            return View("ManagePostsEdit", model);
        }

        private void PopulateModelWithContent(int? id, AdminEditPostModel model, SyndicatedContentTypeEnum type)
        {
            model.FeaturedImages = adminService.GetFeaturedContent(type, (BravoVetsCountryEnum) GetCountryId());

            if (id.HasValue)
            {
                SyndicatedContent syndicatedContent = adminService.GetSyndicatedContent(id.Value);
                model.ContentText = syndicatedContent.ContentText;
                model.Title = syndicatedContent.Title;
                model.LinkUrl = syndicatedContent.LinkUrl;
               
                model.LinkUrlDisplay = syndicatedContent.LinkUrlName;
                model.PublishDate = syndicatedContent.PublishDateUtc;


                if (syndicatedContent.SyndicatedContentAttachments != null)
                {
                    SyndicatedContentAttachment featuredImage =
                        syndicatedContent.SyndicatedContentAttachments.FirstOrDefault(x => x.DisplayInUi);
                    if (featuredImage != null)
                    {
                        model.FeaturedImageId = featuredImage.SyndicatedContentAttachmentId;
                        model.FeaturedImageName = featuredImage.AttachmentFileName;
                    }
                }

                if (syndicatedContent.SyndicatedContentLinks != null)
                {
                    model.Attachments = new List<AdminPostAttachment>();
                    foreach (SyndicatedContentLink syndicatedContentLink in syndicatedContent.SyndicatedContentLinks)
                    {
                        model.Attachments.Add(new AdminPostAttachment
                                              {
                                                  name = syndicatedContentLink.LinkTitle,
                                                  url = syndicatedContentLink.LinkUrl,
                                                  type =
                                                      syndicatedContentLink.SyndicatedContentAttachmentId.HasValue
                                                          ? "file"
                                                          : "link",
                                                  id = syndicatedContentLink.SyndicatedContentLinkId
                                              });
                    }
                }


                if (model.FeaturedImageId.HasValue)
                {
                    FeaturedContentSlim currentFeaturedImage =
                        model.FeaturedImages.Find(x => x.ContentFileName == model.FeaturedImageName);
                    model.FeaturedImageIsInList = currentFeaturedImage != null;
                }
                model.SaveText = Resource.Admin_EditPost_UpdateText;
            }
            else
            {
                model.SaveText = Resource.Admin_EditPost_ScheduleText;
            }
        }

        public ActionResult TrendingTopics(string activeTab)
        {
            var model = new AdminManagePostsModel
                        {
                            Header = Resource.Admin_Nav_TrendingTopics,
                            GetUrl = "/admin/posts/trendingtopics",
                            ActiveTab = string.IsNullOrEmpty(activeTab) ? "future" : activeTab
                        };

            return View("ManagePosts", model);
        }

        public ActionResult UploadFile(int? syndicatedContentId, string attachmentName, SyndicatedContentTypeEnum contentType)
        {
            SyndicatedContent syndicatedContent = null;

            if (syndicatedContentId.HasValue)
            {
                syndicatedContent = adminService.GetSyndicatedContent(syndicatedContentId.Value);
            }
            else
            {
                syndicatedContent =
                    adminService.GetMinimalEmptySyndicatedContent(contentType,
                        (BravoVetsCountryEnum)GetCountryId(), GetCurrentUserName());
                syndicatedContent.BravoVetsStatusId = (int)BravoVetsStatusEnum.InProcess;

                if (contentType == SyndicatedContentTypeEnum.SocialTips)
                {
                    adminService.QueueNewSocialTip(syndicatedContent);
                }
                else if (contentType == SyndicatedContentTypeEnum.BravectoResources)
                {
                    adminService.QueueNewBravectoResource(syndicatedContent);
                }
            }

            

            var results = new UploadFilesResults
            {
                files = new List<ViewDataUploadFilesResult>(),
                queuedcontentid = syndicatedContent.SyndicatedContentId
            };

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);
                var fileNameEncoded = HttpUtility.HtmlEncode(fileName);
                var attachment = new SyndicatedContentAttachment
                {
                    AttachmentExtension = file.ContentType,
                    AttachmentFileName = fileNameEncoded,
                    SyndicatedContentId = syndicatedContent.SyndicatedContentId,
                    AttachmentFile = GetFile(file),
                    DisplayInUi = false,
                    CreateDateUtc = DateTime.UtcNow,
                    ModifiedDateUtc = DateTime.UtcNow,
                };
                var savedAttachment = adminService.SaveSyndicatedContentAttachment(attachment);
                var contentLink = new SyndicatedContentLink
                                  {
                                      LinkTitle = string.IsNullOrEmpty(attachmentName)?fileNameEncoded:attachmentName,
                                      CreateDateUtc = DateTime.UtcNow,
                                      ModifiedDateUtc = DateTime.UtcNow,
                                      SyndicatedContentId = syndicatedContent.SyndicatedContentId,
                                      SyndicatedContentAttachmentId = savedAttachment.SyndicatedContentAttachmentId,
                                      LinkUrl = string.Format("/syndicatedcontent/attachment/{0}", savedAttachment.SyndicatedContentAttachmentId)
                                  };
                adminService.SaveContentLink(contentLink);
                results.files.Add(new ViewDataUploadFilesResult()
                {
                    url = contentLink.LinkUrl,                    
                    name = attachmentName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    attachmentId = savedAttachment.SyndicatedContentAttachmentId
                });
            }

            var serializer = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue};

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


    }
}