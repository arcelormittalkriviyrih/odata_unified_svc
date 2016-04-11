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
    
    public partial class MaterialConsumedRequirement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaterialConsumedRequirement()
        {
            this.MaterialConsumedRequirementProperty = new HashSet<MaterialConsumedRequirementProperty>();
        }
    
        public int ID { get; set; }
        public Nullable<int> MaterialClassID { get; set; }
        public Nullable<int> MaterialDefinitionID { get; set; }
        public Nullable<int> MaterialLotID { get; set; }
        public Nullable<int> MaterialSubLotID { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string RequiredByRequestedSegmentResponce { get; set; }
        public int SegmentRequirementID { get; set; }
    
        public virtual MaterialClass MaterialClass { get; set; }
        public virtual MaterialDefinition MaterialDefinition { get; set; }
        public virtual MaterialLot MaterialLot { get; set; }
        public virtual MaterialSubLot MaterialSubLot { get; set; }
        public virtual SegmentRequirement SegmentRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialConsumedRequirementProperty> MaterialConsumedRequirementProperty { get; set; }
    }
}
