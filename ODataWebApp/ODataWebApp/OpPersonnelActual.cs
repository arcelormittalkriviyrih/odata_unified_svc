//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ODataWebApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class OpPersonnelActual
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OpPersonnelActual()
        {
            this.PersonnelActualProperty1 = new HashSet<PersonnelActualProperty>();
        }
    
        public int ID { get; set; }
        public int PersonnelClassID { get; set; }
        public int PersonID { get; set; }
        public string Description { get; set; }
        public string PersonnelUse { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public Nullable<int> PersonnelActualProperty { get; set; }
        public string RequiredByRequestedSegment { get; set; }
        public Nullable<int> SegmentResponseID { get; set; }
        public Nullable<int> JobResponseID { get; set; }
    
        public virtual JobResponse JobResponse { get; set; }
        public virtual Person Person { get; set; }
        public virtual PersonnelClass PersonnelClass { get; set; }
        public virtual OpSegmentResponse OpSegmentResponse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelActualProperty> PersonnelActualProperty1 { get; set; }
    }
}
