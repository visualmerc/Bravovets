using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainObject
{
    public partial class QueueContent
    {
        private List<QueueContentAttachment> queueContentAttachments = new List<QueueContentAttachment>();

        public List<QueueContentAttachment> QueueContentAttachments
        {
            get
            {
                return this.queueContentAttachments;
            }
            set
            {
                this.queueContentAttachments = value;
            }
        }
    }
}
