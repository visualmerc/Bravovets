using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainObject
{
    public class FeaturedContentSlim
    {
        public int FeaturedContentId { get; set; }
        public int SyndicatedContentTypeId { get; set; }
        public string ContentFileName { get; set; }
        public string ContentExtension { get; set; }
        public byte[] ContentThumbnail { get; set; }

    }
}
