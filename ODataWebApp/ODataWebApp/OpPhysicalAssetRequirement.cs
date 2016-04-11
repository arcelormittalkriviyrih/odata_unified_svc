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
    
    public partial class OpPhysicalAssetRequirement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OpPhysicalAssetRequirement()
        {
            this.PhysicalAssetRequirementProperty = new HashSet<PhysicalAssetRequirementProperty>();
        }
    
        public int ID { get; set; }
        public int PhysicalAssetClassID { get; set; }
        public int PhysicalAssetID { get; set; }
        public string Description { get; set; }
        public string PhysicalAssetUse { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public string EquipmentLevel { get; set; }
        public string RequiredByRequestedSegment { get; set; }
        public Nullable<int> SegmenRequirementID { get; set; }
        public Nullable<int> JobOrderID { get; set; }
    
        public virtual JobOrder JobOrder { get; set; }
        public virtual PhysicalAsset PhysicalAsset { get; set; }
        public virtual PhysicalAssetClass PhysicalAssetClass { get; set; }
        public virtual OpSegmentRequirement OpSegmentRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhysicalAssetRequirementProperty> PhysicalAssetRequirementProperty { get; set; }
    }
}
