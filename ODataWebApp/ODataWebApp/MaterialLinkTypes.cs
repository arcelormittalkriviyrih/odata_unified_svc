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
    
    public partial class MaterialLinkTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaterialLinkTypes()
        {
            this.MaterialDefinitionLinks = new HashSet<MaterialDefinitionLinks>();
            this.MaterialLotLinks = new HashSet<MaterialLotLinks>();
            this.MaterialSubLotLinks = new HashSet<MaterialSubLotLinks>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
    
        public virtual MaterialClassLinks MaterialClassLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialDefinitionLinks> MaterialDefinitionLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialLotLinks> MaterialLotLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialSubLotLinks> MaterialSubLotLinks { get; set; }
    }
}
