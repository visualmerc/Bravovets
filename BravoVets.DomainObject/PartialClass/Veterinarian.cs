using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainObject
{
    public partial class Veterinarian
    {
        public bool CanEditFacilities { get; set; }

        public bool IsFacebookLinked { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }

        public bool IsTwitterLinked { get; set; }

        public VeterinarianFacility EditableFacility { get; set; }
    }
}
