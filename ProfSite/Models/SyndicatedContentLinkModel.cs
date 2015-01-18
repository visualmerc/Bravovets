using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BravoVets.DomainObject;

namespace ProfSite.Models
{
    public class SyndicatedContentLinkModel
    {
        public SyndicatedContentLinkModel(SyndicatedContentLink scl)
        {
            if (scl.SyndicatedContentAttachmentId.HasValue)
            {
                var sca =
                    scl.SyndicatedContent.SyndicatedContentAttachments.FirstOrDefault(
                        a => a.SyndicatedContentAttachmentId == scl.SyndicatedContentAttachmentId.Value);
                if (sca != null && sca.AttachmentExtension == "pdf")
                {
                    this.ActiveCss = "doc";
                }
                else
                {
                    this.ActiveCss = "picture";
                }
            }
            else
            {
                this.ActiveCss = "link";
            }

            this.LinkUrl = scl.LinkUrl;
            this.LinkTitle = scl.LinkTitle;
        }

        public string ActiveCss { get; set; }

        public string LinkTitle { get; set; }

        public string LinkUrl { get; set; }

    }
}