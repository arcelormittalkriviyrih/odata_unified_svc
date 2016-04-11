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
    
    public partial class OpEquipmentCapability
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OpEquipmentCapability()
        {
            this.EquipmentCapabilityProperty = new HashSet<EquipmentCapabilityProperty>();
        }
    
        public int ID { get; set; }
        public Nullable<int> EquipmentClassID { get; set; }
        public Nullable<int> EquipmentID { get; set; }
        public string Description { get; set; }
        public string CapabilityType { get; set; }
        public string Reason { get; set; }
        public string ConfidenceFactor { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public string EquipmentUse { get; set; }
        public Nullable<System.DateTimeOffset> StartTime { get; set; }
        public Nullable<System.DateTimeOffset> EndTime { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> OperationCapabilityID { get; set; }
        public Nullable<int> WorkCapabilityID { get; set; }
        public Nullable<int> WorkMasterCapabilityID { get; set; }
        public Nullable<int> ProcessSegmentCapabilityID { get; set; }
    
        public virtual Equipment Equipment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentCapabilityProperty> EquipmentCapabilityProperty { get; set; }
        public virtual EquipmentClass EquipmentClass { get; set; }
        public virtual OperationsCapability OperationsCapability { get; set; }
        public virtual OpProcessSegmentCapability OpProcessSegmentCapability { get; set; }
        public virtual WorkCapability WorkCapability { get; set; }
        public virtual WorkMasterCapability WorkMasterCapability { get; set; }
    }
}
