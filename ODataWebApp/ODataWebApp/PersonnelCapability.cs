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
    
    public partial class PersonnelCapability
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonnelCapability()
        {
            this.PersonnelCapabilityProperty = new HashSet<PersonnelCapabilityProperty>();
        }
    
        public int ID { get; set; }
        public Nullable<int> PersonnelClassID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public string Description { get; set; }
        public string CapabilityType { get; set; }
        public string Reason { get; set; }
        public string EquipmentElementLevel { get; set; }
        public Nullable<System.DateTimeOffset> StartTime { get; set; }
        public Nullable<System.DateTimeOffset> EndTime { get; set; }
        public string Location { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> ProductionCapabilityID { get; set; }
        public Nullable<int> ProcessSegmentCapabilityID { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual PersonnelClass PersonnelClass { get; set; }
        public virtual ProcessSegmentCapability ProcessSegmentCapability { get; set; }
        public virtual ProductionCapability ProductionCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelCapabilityProperty> PersonnelCapabilityProperty { get; set; }
    }
}
