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
    
    public partial class SyndicatedContentLink
    {
        public int SyndicatedContentLinkId { get; set; }
        public int SyndicatedContentId { get; set; }
        public string LinkTitle { get; set; }
        public string LinkUrl { get; set; }
        public Nullable<int> SyndicatedContentAttachmentId { get; set; }
        public System.DateTime CreateDateUtc { get; set; }
        public System.DateTime ModifiedDateUtc { get; set; }
    
        public virtual SyndicatedContent SyndicatedContent { get; set; }
    }
}