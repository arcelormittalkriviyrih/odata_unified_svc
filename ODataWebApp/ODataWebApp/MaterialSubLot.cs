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
    
    public partial class MaterialSubLot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaterialSubLot()
        {
            this.MaterialActual = new HashSet<MaterialActual>();
            this.MaterialCapability = new HashSet<MaterialCapability>();
            this.MaterialConsumedActual = new HashSet<MaterialConsumedActual>();
            this.MaterialConsumedRequirement = new HashSet<MaterialConsumedRequirement>();
            this.MaterialInformation = new HashSet<MaterialInformation>();
            this.MaterialLotProperty = new HashSet<MaterialLotProperty>();
            this.MaterialProducedActual = new HashSet<MaterialProducedActual>();
            this.MaterialProducedRequirement = new HashSet<MaterialProducedRequirement>();
            this.MaterialRequirement = new HashSet<MaterialRequirement>();
            this.MaterialSubLot1 = new HashSet<MaterialSubLot>();
            this.MaterialSubLotLinks = new HashSet<MaterialSubLotLinks>();
            this.MaterialSubLotLinks1 = new HashSet<MaterialSubLotLinks>();
            this.OpMaterialActual = new HashSet<OpMaterialActual>();
            this.OpMaterialCapability = new HashSet<OpMaterialCapability>();
            this.OpMaterialRequirement = new HashSet<OpMaterialRequirement>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Nullable<int> HierarchyScope { get; set; }
        public string Status { get; set; }
        public string StorageLocation { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> MaterialLotID { get; set; }
        public Nullable<int> AssemblyLotID { get; set; }
        public Nullable<int> AssemblySubLotID { get; set; }
        public string AssemblyType { get; set; }
        public string AssemblyReleationship { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialActual> MaterialActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialCapability> MaterialCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialConsumedActual> MaterialConsumedActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialConsumedRequirement> MaterialConsumedRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialInformation> MaterialInformation { get; set; }
        public virtual MaterialLot MaterialLot { get; set; }
        public virtual MaterialLot MaterialLot1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialLotProperty> MaterialLotProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialProducedActual> MaterialProducedActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialProducedRequirement> MaterialProducedRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialRequirement> MaterialRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSubLot> MaterialSubLot1 { get; set; }
        public virtual MaterialSubLot MaterialSubLot2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSubLotLinks> MaterialSubLotLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSubLotLinks> MaterialSubLotLinks1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialActual> OpMaterialActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialCapability> OpMaterialCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialRequirement> OpMaterialRequirement { get; set; }
    }
}
