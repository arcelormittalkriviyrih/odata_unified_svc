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
    
    public partial class ProductProductionRuleIDGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductProductionRuleIDGroup()
        {
            this.ProductDefinition = new HashSet<ProductDefinition>();
        }
    
        public int ProductProductionRuleID { get; set; }
        public string Version { get; set; }
        public Nullable<int> ProductionRequest { get; set; }
        public Nullable<int> ProductionResponse { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductDefinition> ProductDefinition { get; set; }
        public virtual ProductionRequest ProductionRequest1 { get; set; }
        public virtual ProductionResponse ProductionResponse1 { get; set; }
    }
}
