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
    
    public partial class MaterialClassProperty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaterialClassProperty()
        {
            this.MaterialClassProperty11 = new HashSet<MaterialClassProperty>();
            this.MaterialDefinitionProperty = new HashSet<MaterialDefinitionProperty>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Nullable<int> MaterialClassProperty1 { get; set; }
        public string MaterialTestSpecificationID { get; set; }
        public Nullable<int> PropertyType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialClassProperty> MaterialClassProperty11 { get; set; }
        public virtual MaterialClassProperty MaterialClassProperty2 { get; set; }
        public virtual MaterialTestSpecification MaterialTestSpecification { get; set; }
        public virtual PropertyTypes PropertyTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialDefinitionProperty> MaterialDefinitionProperty { get; set; }
    }
}
