//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SanayeeIdentity_5.Controllers.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdressDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdressDetail()
        {
            this.Requests = new HashSet<Request>();
        }
    
        public int AdressId { get; set; }
        public Nullable<int> AreaId { get; set; }
        public string Text { get; set; }
        public Nullable<int> Mapid { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual Map Map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Requests { get; set; }
    }
}