using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;

namespace ProfSite.Models
{
    public class SyndicatedContentModel
    {

        public SyndicatedContentModel()
        {
            
        }

        public SyndicatedContentModel(SyndicatedContent syndicatedContent, ContentFilterEnum filterBy, bool showShare)
        {
            Title = syndicatedContent.Title;
            ContentText = syndicatedContent.ContentText;
            SetImage(syndicatedContent);
            Id = syndicatedContent.SyndicatedContentId;
            LinkUrl = syndicatedContent.LinkUrl;
            LinkUrlName = syndicatedContent.LinkUrlName;
            IsFavorite = syndicatedContent.IsFavoritedByMe;
            ShowShare = showShare;

            ShowHidden = filterBy != ContentFilterEnum.GenericShare;

            Hidden = filterBy == ContentFilterEnum.Hidden;

            this.BuildAttachmentLinks(syndicatedContent);
        }


        public SyndicatedContentModel(SyndicatedContent syndicatedContent, bool showShare)
        {
            Title = syndicatedContent.Title;
            ContentText = syndicatedContent.ContentText;
            SetImage(syndicatedContent);
            Id = syndicatedContent.SyndicatedContentId;
            LinkUrl = syndicatedContent.LinkUrl;
            LinkUrlName = syndicatedContent.LinkUrlName;
            IsFavorite = syndicatedContent.IsFavoritedByMe;
            ShowShare = showShare;
            this.BuildAttachmentLinks(syndicatedContent);
        }

        public SyndicatedContentModel(QueueContent content, bool showShare)
        {
            ContentText = content.ContentText;            
            Id = content.QueueContentId;
            LinkUrl = content.LinkUrl;
            LinkUrlName = content.LinkUrl;
            ShowShare = showShare;
            this.SyndicatedLinks = new List<SyndicatedContentLinkModel>();
        }

        private void SetImage(SyndicatedContent syndicatedContent)
        {

            if (syndicatedContent.SyndicatedContentAttachments == null)
                return;

            var attachment =
                syndicatedContent.SyndicatedContentAttachments.FirstOrDefault(sca => sca.DisplayInUi);

            if (attachment == null)
                return;

            ImageUrl = string.Format("/syndicatedcontent/image/{0}/{1}", attachment.SyndicatedContentId, attachment.SyndicatedContentAttachmentId);
            ImageName = attachment.AttachmentFileName;

        }

        private void BuildAttachmentLinks(SyndicatedContent syndicatedContent)
        {
            this.SyndicatedLinks = new List<SyndicatedContentLinkModel>();
            foreach (var scl in syndicatedContent.SyndicatedContentLinks)
            {
                this.SyndicatedLinks.Add(new SyndicatedContentLinkModel(scl));
            }

            if (!string.IsNullOrEmpty(syndicatedContent.LinkUrl))
            {
                SyndicatedLinks.Add(new SyndicatedContentLinkModel(new SyndicatedContentLink{LinkUrl = syndicatedContent.LinkUrl,LinkTitle = syndicatedContent.LinkUrlName}));
            }
        }

        public string ActiveCSS { get; set; }
        public string Title { get; set; }
        public string ContentText { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }

        public List<SyndicatedContentLinkModel> SyndicatedLinks { get; set; }

        public int Id { get; set; }

        public string LinkUrl { get; set; }
        public string LinkUrlName { get; set; }
        public bool IsFavorite { get; set; }
        public bool Hidden { get; set; }
        public bool ShowShare { get; set; }
        public string SiteLanguage { get; set; }
        public int queuedContentId { get; set; }
        public bool ShowHidden { get; set; }
    }

    public class ViewSyndicatedContentModel
    {
        public ViewSyndicatedContentModel()
        {
            ShowHidden = false;
            ShowShared = false;
            IsFirstPage = true;
        }

        public List<SyndicatedContentModel> Content { get; set; }
        public bool ShowShared { get; set; }
        public bool ShowHidden { get; set; }
        public bool SocialLinked { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }

        public string CurrentView { get; set; }
        public bool IsFirstPage { get; set; }
    }
}