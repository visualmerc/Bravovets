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
    
    public partial class BravoVetsUserActivity
    {
        public long BravoVetsUserActivityId { get; set; }
        public int BravoVetsUserId { get; set; }
        public int ActivityTypeId { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public System.DateTime CreateDateUtc { get; set; }
        public System.DateTime ModifiedDateUtc { get; set; }
        public bool Deleted { get; set; }
    
        public virtual ActivityType ActivityType { get; set; }
        public virtual BravoVetsUser BravoVetsUser { get; set; }
    }
}
