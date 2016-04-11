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
    
    public partial class PersonnelRequirement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonnelRequirement()
        {
            this.PersonnelRequirementProperty = new HashSet<PersonnelRequirementProperty>();
        }
    
        public int ID { get; set; }
        public Nullable<int> PersonnelClassID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string RequiredByRequestedSegmentResponce { get; set; }
        public int SegmentRequirementID { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual PersonnelClass PersonnelClass { get; set; }
        public virtual SegmentRequirement SegmentRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelRequirementProperty> PersonnelRequirementProperty { get; set; }
    }
}
