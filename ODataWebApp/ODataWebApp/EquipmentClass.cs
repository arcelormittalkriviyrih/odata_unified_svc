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
    
    public partial class EquipmentClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EquipmentClass()
        {
            this.BatchListEntry = new HashSet<BatchListEntry>();
            this.Equipment = new HashSet<Equipment>();
            this.EquipmentActual = new HashSet<EquipmentActual>();
            this.EquipmentCapability = new HashSet<EquipmentCapability>();
            this.EquipmentClass1 = new HashSet<EquipmentClass>();
            this.EquipmentClassProperty = new HashSet<EquipmentClassProperty>();
            this.EquipmentInformation = new HashSet<EquipmentInformation>();
            this.EquipmentRequirement = new HashSet<EquipmentRequirement>();
            this.EquipmentSegmentSpecification = new HashSet<EquipmentSegmentSpecification>();
            this.EquipmentSpecification = new HashSet<EquipmentSpecification>();
            this.OpEquipmentActual = new HashSet<OpEquipmentActual>();
            this.OpEquipmentCapability = new HashSet<OpEquipmentCapability>();
            this.OpEquipmentRequirement = new HashSet<OpEquipmentRequirement>();
            this.OpEquipmentSpecification = new HashSet<OpEquipmentSpecification>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public string EquipmentLevel { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchListEntry> BatchListEntry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Equipment> Equipment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentActual> EquipmentActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentCapability> EquipmentCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentClass> EquipmentClass1 { get; set; }
        public virtual EquipmentClass EquipmentClass2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentClassProperty> EquipmentClassProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentInformation> EquipmentInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentRequirement> EquipmentRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentSegmentSpecification> EquipmentSegmentSpecification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentSpecification> EquipmentSpecification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpEquipmentActual> OpEquipmentActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpEquipmentCapability> OpEquipmentCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpEquipmentRequirement> OpEquipmentRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpEquipmentSpecification> OpEquipmentSpecification { get; set; }
    }
}
