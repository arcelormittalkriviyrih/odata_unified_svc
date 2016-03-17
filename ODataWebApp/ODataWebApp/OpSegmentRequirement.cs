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
    
    public partial class OpSegmentRequirement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OpSegmentRequirement()
        {
            this.OpEquipmentRequirement = new HashSet<OpEquipmentRequirement>();
            this.OpMaterialRequirement = new HashSet<OpMaterialRequirement>();
            this.OpPersonnelRequirement = new HashSet<OpPersonnelRequirement>();
            this.OpPhysicalAssetRequirement = new HashSet<OpPhysicalAssetRequirement>();
            this.OpSegmentRequirement1 = new HashSet<OpSegmentRequirement>();
            this.SegmentParameter = new HashSet<SegmentParameter>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public string OperationsType { get; set; }
        public Nullable<int> ProcessSegmentID { get; set; }
        public Nullable<System.DateTime> EarliestStartTime { get; set; }
        public Nullable<System.DateTime> LatestEndTime { get; set; }
        public string Duration { get; set; }
        public Nullable<int> OperationsDefinitionID { get; set; }
        public string SegmentState { get; set; }
        public Nullable<int> SegmentRequirement { get; set; }
        public string RequiredByrequestedSegment { get; set; }
        public Nullable<int> OperationsRequest { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpEquipmentRequirement> OpEquipmentRequirement { get; set; }
        public virtual OperationsDefinition OperationsDefinition { get; set; }
        public virtual OperationsRequest OperationsRequest1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialRequirement> OpMaterialRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpPersonnelRequirement> OpPersonnelRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpPhysicalAssetRequirement> OpPhysicalAssetRequirement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpSegmentRequirement> OpSegmentRequirement1 { get; set; }
        public virtual OpSegmentRequirement OpSegmentRequirement2 { get; set; }
        public virtual ProcessSegment ProcessSegment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SegmentParameter> SegmentParameter { get; set; }
    }
}
