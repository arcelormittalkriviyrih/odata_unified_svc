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
    
    public partial class ProcessSegmentCapability
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProcessSegmentCapability()
        {
            this.EquipmentCapability = new HashSet<EquipmentCapability>();
            this.MaterialCapability = new HashSet<MaterialCapability>();
            this.PersonnelCapability = new HashSet<PersonnelCapability>();
            this.ProcessSegmentCapability11 = new HashSet<ProcessSegmentCapability>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public Nullable<int> ProcessSegmentID { get; set; }
        public string CapabilityType { get; set; }
        public string Reason { get; set; }
        public string Location { get; set; }
        public string EquipmentElementLevel { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> ProcessSegmentCapability1 { get; set; }
        public Nullable<int> ProductionCapabilityID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentCapability> EquipmentCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialCapability> MaterialCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelCapability> PersonnelCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessSegmentCapability> ProcessSegmentCapability11 { get; set; }
        public virtual ProcessSegmentCapability ProcessSegmentCapability2 { get; set; }
        public virtual ProductionCapability ProductionCapability { get; set; }
    }
}
