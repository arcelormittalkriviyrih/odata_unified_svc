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
    
    public partial class MaterialSegmentSpecificftion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaterialSegmentSpecificftion()
        {
            this.MaterialSegmentSpecificationProperty = new HashSet<MaterialSegmentSpecificationProperty>();
            this.MaterialSegmentSpecificftion1 = new HashSet<MaterialSegmentSpecificftion>();
        }
    
        public int ID { get; set; }
        public Nullable<int> MaterialDefinitionID { get; set; }
        public string Description { get; set; }
        public string AssemblyType { get; set; }
        public string AssemblyRelationship { get; set; }
        public Nullable<int> AssemblySpecificationID { get; set; }
        public string MaterialUse { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> ProccesSegmentID { get; set; }
    
        public virtual MaterialDefinition MaterialDefinition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSegmentSpecificationProperty> MaterialSegmentSpecificationProperty { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSegmentSpecificftion> MaterialSegmentSpecificftion1 { get; set; }
        public virtual MaterialSegmentSpecificftion MaterialSegmentSpecificftion2 { get; set; }
        public virtual ProcessSegment ProcessSegment { get; set; }
    }
}
