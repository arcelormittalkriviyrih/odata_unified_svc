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
    
    public partial class PhysicalAssetClassProperty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhysicalAssetClassProperty()
        {
            this.PhysicalAssetClassProperty11 = new HashSet<PhysicalAssetClassProperty>();
            this.PhysicalAssetProperty = new HashSet<PhysicalAssetProperty>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Nullable<int> PhysicalAssetClassProperty1 { get; set; }
        public string PhysicalAssetCapabilityTestSpecification { get; set; }
        public Nullable<int> PhysicalAssetClassID { get; set; }
    
        public virtual PhysicalAssetCapabilityTestSpesification PhysicalAssetCapabilityTestSpesification { get; set; }
        public virtual PhysicalAssetClass PhysicalAssetClass { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhysicalAssetClassProperty> PhysicalAssetClassProperty11 { get; set; }
        public virtual PhysicalAssetClassProperty PhysicalAssetClassProperty2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhysicalAssetProperty> PhysicalAssetProperty { get; set; }
    }
}
