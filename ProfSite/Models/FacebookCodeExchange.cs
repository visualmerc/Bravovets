using System.Collections.Generic;

namespace ProfSite.Models
{
    public class FacebookCodeExchange
    {        
        public string userid { get; set; }
        //access token
        public string access_token { get; set; }
        /// <summary>
        /// seconds till expiration
        /// </summary>
        public long expires { get; set; }
    }
}