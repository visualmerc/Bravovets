//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BravoVets.DomainObject
{
    using System;
    using System.Collections.Generic;
    
    public partial class BravoVetsCountry
    {
        public BravoVetsCountry()
        {
            this.BravoVetsUsers = new HashSet<BravoVetsUser>();
            this.QueueContents = new HashSet<QueueContent>();
            this.SyndicatedContents = new HashSet<SyndicatedContent>();
            this.Veterinarians = new HashSet<Veterinarian>();
            this.FeaturedContents = new HashSet<FeaturedContent>();
        }
    
        public int BravoVetsCountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryNameResourceKey { get; set; }
        public string CountryIsoCode { get; set; }
        public int BravoVetsLanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string CultureName { get; set; }
        public bool Active { get; set; }
        public System.DateTime CreateDateUtc { get; set; }
        public System.DateTime ModifiedDateUtc { get; set; }
        public bool Deleted { get; set; }
    
        public virtual ICollection<BravoVetsUser> BravoVetsUsers { get; set; }
        public virtual ICollection<QueueContent> QueueContents { get; set; }
        public virtual BravoVetsLanguage BravoVetsLanguage { get; set; }
        public virtual ICollection<SyndicatedContent> SyndicatedContents { get; set; }
        public virtual ICollection<Veterinarian> Veterinarians { get; set; }
        public virtual ICollection<FeaturedContent> FeaturedContents { get; set; }
    }
}