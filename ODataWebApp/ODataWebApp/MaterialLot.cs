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
    
    public partial class MaterialLot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaterialLot()
        {
            this.BatchProductionRecord = new HashSet<BatchProductionRecord>();
            this.MaterialActual = new HashSet<MaterialActual>();
            this.MaterialCapability = new HashSet<MaterialCapability>();
            this.MaterialConsumedActual = new HashSet<MaterialConsumedActual>();
            this.MaterialConsumedRequirement = new HashSet<MaterialConsumedRequirement>();
            this.MaterialInformation = new HashSet<MaterialInformation>();
            this.MaterialLot1 = new HashSet<MaterialLot>();
            this.MaterialLotLinks = new HashSet<MaterialLotLinks>();
            this.MaterialLotLinks1 = new HashSet<MaterialLotLinks>();
            this.MaterialLotProperty = new HashSet<MaterialLotProperty>();
            this.MaterialProducedActual = new HashSet<MaterialProducedActual>();
            this.MaterialProducedRequirement = new HashSet<MaterialProducedRequirement>();
            this.MaterialRequirement = new HashSet<MaterialRequirement>();
            this.MaterialSubLot = new HashSet<MaterialSubLot>();
            this.MaterialSubLot1 = new HashSet<MaterialSubLot>();
            this.OpMaterialActual = new HashSet<OpMaterialActual>();
            this.OpMaterialCapability = new HashSet<OpMaterialCapability>();
            this.OpMaterialRequirement = new HashSet<OpMaterialRequirement>();
        }
    
        public int ID { get; set; }
        public string FactoryNumber { get; set; }
        public Nullable<int> MaterialDefinitionID { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StorageLocation { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Location { get; set; }
        public Nullable<int> AssemblyLotID { get; set; }
        public string AssemblyType { get; set; }
        public string AssemblyRelationship { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchProductionRecord> BatchProductionRecord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialActual> MaterialActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialCapability> MaterialCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialConsumedActual> MaterialConsumedActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialConsumedRequirement> MaterialConsumedRequirement { get; set; }
        public virtual MaterialDefinition MaterialDefinition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialInformation> MaterialInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialLot> MaterialLot1 { get; set; }
        public virtual MaterialLot MaterialLot2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialLotLinks> MaterialLotLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialLotLinks> MaterialLotLinks1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialLotProperty> MaterialLotProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialProducedActual> MaterialProducedActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialProducedRequirement> MaterialProducedRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialRequirement> MaterialRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSubLot> MaterialSubLot { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSubLot> MaterialSubLot1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialActual> OpMaterialActual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialCapability> OpMaterialCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialRequirement> OpMaterialRequirement { get; set; }
    }
}
