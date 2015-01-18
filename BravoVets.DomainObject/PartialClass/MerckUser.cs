using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainObject
{
    public partial class MerckUser
    {
        /// <summary>
        /// Id passed by the Merck LFW
        /// </summary>
        public int MerckUserId { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CountryOrigin { get; set; }

        public string Occupation { get; set; }

        /*    <add key="USERID" value="1" />
    <add key="EMAILADDRESS" value="nick.vanmatre@merck.com" />
    <add key="FIRSTNAME" value="Nick" />
    <add key="LASTNAME" value="VanMatre" />
    <add key="COUNTRYORIGIN" value="US" />
    <add key="OCCUPATION" value="VET" />*/

    }
}
