using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;

namespace ProfSite.Models
{
    public class AdminManagePostsModel
    {
        public AdminManagePostsModel()
        {
            ActiveTab = "future";
            Items = new List<SyndicatedContent>();
           
        }
        public string Header { get; set; }

        public string ActiveTab { get; set; }

        public List<SyndicatedContent> Items { get; set; }

        public int ClientOffset { get; set; }

        public decimal PageCount { get; set; }

        public string GetUrl { get; set; }
        public string EditUrl { get; set; }
       
    }

    public class AdminEditPostModel
    {
        public AdminEditPostModel()
        {
            TwitterValidation = false;
            AllowRichText = true;
            NumberOfAttachments = 5;
            AllowFileAttachments = true;
            PublishDate = DateTime.UtcNow;    
            Attachments = new List<AdminPostAttachment>();
           
        }
        public int? Id
        {
            get; set;
        }
        public string Header { get; set; }

        public string ContentText { get; set; }
        public DateTime PublishDate { get; set; }
        public int ClientOffset { get; set; }
        public string Title { get; set; }
        public bool TwitterValidation { get; set; }
        public bool AllowRichText { get; set; }

        public int NumberOfAttachments { get; set; }
        public bool AllowFileAttachments { get; set; }
        public SyndicatedContentTypeEnum ContentType { get; set; }
        public List<FeaturedContentSlim> FeaturedImages { get; set; }
        public string HeaderText { get; set; }

        public string DeleteUrl { get; set; }
        public string ListUrl { get; set; }
        public string PostUrl { get; set; }
        public string PreviewUrl { get; set; }
        public bool AllowDelete { get; set; }
        public int? FeaturedImageId { get; set; }
        public string FeaturedImageName { get; set; }
        public bool FeaturedImageIsInList { get; set; }
        public string LinkUrl { get; set; }
        public string LinkUrlDisplay { get; set; }

        public string SaveText { get; set; }
        public List<AdminPostAttachment> Attachments { get; set; }
        public string ScheduleTooltip { get; set;}
        public string FeaturedImageTooltip { get; set; }
        public string AttachmentTooltip { get; set; }
    }

    public class AdminSavePostModel
    {
        public int? syndicatedContentId { get; set; }
        public string contentText { get; set; }
        public string title { get; set; }
        public int? featuredContentId { get; set; }
        public string featuredContentUrl { get; set; }
        public List<AdminPostAttachment> attachments { get; set; }
        public bool postNow { get; set; }
        public DateTime? publishDate { get; set; }
        public int clientOffset { get; set; }
    }

    public class AdminPostAttachment
    {
        public string name { get; set; }
        public string url { get; set; }
        public string type { get; set; }

        public int? id { get; set; }

        public bool isDeleted { get; set; }
    }

    public class AdminSavePostResultsModel
    {
        public int syndicatedContentId { get; set; }
        
    }

}