using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainObject
{
    public partial class SyndicatedContent
    {

        private List<SyndicatedContentAttachment> syndicatedContentAttachments = new List<SyndicatedContentAttachment>(); 

        public bool IsFavoritedByMe { get; set; }

        public bool IsSharedByMe { get; set; }

        public bool IsViewedByMe { get; set; }

        public bool IsQueuedByMe { get; set; }

        public List<SyndicatedContentAttachment> SyndicatedContentAttachments
        {
            get
            {
                return this.syndicatedContentAttachments;
            }
            set
            {
                this.syndicatedContentAttachments = value;
            }
        }
    }
}
