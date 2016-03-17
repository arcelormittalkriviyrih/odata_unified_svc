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
    
    public partial class OpPersonnelRequirement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OpPersonnelRequirement()
        {
            this.PersonnelRequirementProperty = new HashSet<PersonnelRequirementProperty>();
        }
    
        public int ID { get; set; }
        public int PersonID { get; set; }
        public string Description { get; set; }
        public string PersonnelUse { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string RequiredByRequestedSegment { get; set; }
        public Nullable<int> JobOrderID { get; set; }
        public Nullable<int> SegmenRequirementID { get; set; }
    
        public virtual JobOrder JobOrder { get; set; }
        public virtual Person Person { get; set; }
        public virtual OpSegmentRequirement OpSegmentRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelRequirementProperty> PersonnelRequirementProperty { get; set; }
    }
}
