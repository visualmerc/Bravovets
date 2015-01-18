using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using LFW20.FrontEnd.PBAC;

namespace ProfSite.Models
{
    public class ShowSocialDialogModel
    {
        public string dialogType { get; set; }
        public int? id { get; set; }
        public int clientOffset { get; set; }
    }

    public class SocialDialogModel
    {
        public SocialDialogModel()
        {
            EnableFacebook = false;
            EnableTwitter = false;
            ShowShareOptions = false;
            EnableAttachUrl = true;
            ShowDelete = false;
        }
        public string HeaderClass { get; set; }
        public string Header { get; set; }

        public string PostText { get; set; }
        public bool ShowShareOptions { get; set; }
        public bool EnableFacebook { get; set; }
        public bool EnableTwitter { get; set; }
        public bool EnableAttachUrl { get; set; }
        public bool ShowDelete { get; set; }

        public int ClientOffset { get; set; }
        public SocialDialogContentModel ContentModel { get; set; }
    }

    public class SocialDialogContentModel
    {

        public SocialDialogContentModel()
        {
            Attachments = new List<SocialDialogAttachmentModel>();
            PostNow = true;
            PublishDate = DateTime.UtcNow;
            IsPublished = false;
        }

        public SocialDialogContentModel(SyndicatedContent syndicatedContent)
        {
            ContentText = syndicatedContent.ContentText;
            Id = syndicatedContent.SyndicatedContentId;
            LinkUrl = syndicatedContent.LinkUrl;
            Attachments = new List<SocialDialogAttachmentModel>();
            PostNow = true;
            PublishDate = DateTime.UtcNow;
            IsPublished = false;
        }


        public SocialDialogContentModel(QueueContent content)
        {
            ContentText = content.ContentText;
            Id = content.QueueContentId;
            LinkUrl = content.LinkUrl;
            PublishDate = content.PublishDateUtc;
            Attachments = new List<SocialDialogAttachmentModel>();

            PostNow = content.BravoVetsStatusId == (int) BravoVetsStatusEnum.InProcess;

            IsPublished = content.IsPublished;
        }

        public string ContentText { get; set; }

        public string LinkUrl { get; set; }

        public string SiteLanguage { get; set; }

        public int Id { get; set; }

        public DateTime PublishDate { get; set; }

        public bool PostNow { get; set; }

        /// <summary>
        /// Adding this as a property, since published content can be opened from the calendar
        /// </summary>
        public bool IsPublished { get; set; }

        public List<SocialDialogAttachmentModel> Attachments { get; set; }
    }

    public class SocialDialogAttachmentModel
    {
        public int AttachmentId { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string FileName { get; set; }
       
    }


}