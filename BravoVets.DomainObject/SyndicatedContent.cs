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
    
    public partial class SyndicatedContent
    {
        public SyndicatedContent()
        {
            this.SyndicatedContentTags = new HashSet<SyndicatedContentTag>();
            this.SyndicatedContentUsers = new HashSet<SyndicatedContentUser>();
            this.SyndicatedContentLinks = new HashSet<SyndicatedContentLink>();
        }
    
        public int SyndicatedContentId { get; set; }
        public int BravoVetsCountryId { get; set; }
        public int SyndicatedContentTypeId { get; set; }
        public int SyndicatedContentPostTypeId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Subject { get; set; }
        public string Summary { get; set; }
        public string ContentText { get; set; }
        public string LinkUrl { get; set; }
        public int BravoVetsStatusId { get; set; }
        public int NumberOfFavorites { get; set; }
        public int NumberOfShares { get; set; }
        public int NumberOfViews { get; set; }
        public System.DateTime PublishDateUtc { get; set; }
        public System.DateTime CreateDateUtc { get; set; }
        public System.DateTime ModifiedDateUtc { get; set; }
        public bool Deleted { get; set; }
        public string LinkUrlName { get; set; }
    
        public virtual BravoVetsCountry BravoVetsCountry { get; set; }
        public virtual BravoVetsStatu BravoVetsStatu { get; set; }
        public virtual SyndicatedContentType SyndicatedContentType { get; set; }
        public virtual SyndicatedContentPostType SyndicatedContentPostType { get; set; }
        public virtual ICollection<SyndicatedContentTag> SyndicatedContentTags { get; set; }
        public virtual ICollection<SyndicatedContentUser> SyndicatedContentUsers { get; set; }
        public virtual ICollection<SyndicatedContentLink> SyndicatedContentLinks { get; set; }
    }
}
