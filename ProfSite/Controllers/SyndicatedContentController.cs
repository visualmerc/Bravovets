using System.Linq;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainService.Service;
using log4net;

namespace ProfSite.Controllers
{
    public class SyndicatedContentController : AbstractBaseController
    {
        protected readonly SyndicatedContentDomainService syndicatedContentService;
        protected readonly PublishQueueDomainService publishQueueDomainService;
        protected readonly SyndicatedContentAdminService adminService;

        protected readonly ILog Logger = LogManager.GetLogger(typeof(SyndicatedContentController));

        public SyndicatedContentController()
        {
            syndicatedContentService = new SyndicatedContentDomainService();
            publishQueueDomainService = new PublishQueueDomainService();
            adminService = new SyndicatedContentAdminService();
        }

        public ActionResult QueuedContent(int contentId)
        {

           var attachment =  publishQueueDomainService.GetQueueContentAttachment(contentId);
           if (attachment == null)
               return new EmptyResult();

           return File(attachment.AttachmentFile, GetImageMIMEType(attachment.AttachmentExtension));

        }

        public ActionResult FeaturedContent(SyndicatedContentTypeEnum type,int contentId, bool useFullImage)
        {
            if (useFullImage)
            {
                var featuredContent = adminService.GetFeaturedContent(contentId);
                if (featuredContent == null)
                {
                    return new EmptyResult();
                }
                return File(featuredContent.ContentFile, GetImageMIMEType(featuredContent.ContentExtension));
            }

            var items = adminService.GetFeaturedContent(type, (BravoVetsCountryEnum) GetCountryId());
            
            var item = items.Find(x => x.FeaturedContentId == contentId);
           
            if (item == null)
                return new EmptyResult();

            return File(item.ContentThumbnail, GetImageMIMEType(item.ContentExtension));
        }

        public ActionResult Image(int contentId, int attachmentId)
        {
           
            var attachments = syndicatedContentService.GetAttachmentsById(contentId);
           
            var attachment = attachments.FirstOrDefault(x => x.SyndicatedContentAttachmentId == attachmentId);

            if (attachment == null)
                return new EmptyResult();

            return File(attachment.AttachmentFile, GetImageMIMEType(attachment.AttachmentExtension));
        }

        public ActionResult Attachment(int contentId)
        {
            var attachment = adminService.GetSyndicatedContentAttachment(contentId);

            if (attachment == null)
                return new EmptyResult();

            return File(attachment.AttachmentFile, GetImageMIMEType(attachment.AttachmentExtension));
        }

        public ActionResult ViewContent(int contentId)
        {
            int userId = GetCurrentUserId();
            var content = syndicatedContentService.GetSyndicatedContent(userId, contentId);

            syndicatedContentService.ViewContent(userId, contentId);

            return Redirect(content.LinkUrl);
        }

        private string GetImageMIMEType(string ext)
        {
            if (ext == "jpg")
                return "image/jpeg";

            if (ext == "gif")
                return "image/gif";

            if (ext == "png")
                return "image/png";

            return ext;
        }

    }
}