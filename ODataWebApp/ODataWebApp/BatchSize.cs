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
    
    public partial class BatchSize
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BatchSize()
        {
            this.GRecipeProductInformation = new HashSet<GRecipeProductInformation>();
            this.Header = new HashSet<Header>();
        }
    
        public string Nominal { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string ScaleReference { get; set; }
        public string ScaledSize { get; set; }
        public string UnitOfMeasure { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRecipeProductInformation> GRecipeProductInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Header> Header { get; set; }
    }
}
