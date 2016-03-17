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
    
    public partial class ConsumableActual
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConsumableActual()
        {
            this.ConsumableActualProperty = new HashSet<ConsumableActualProperty>();
        }
    
        public int ID { get; set; }
        public Nullable<int> MaterialDefinitionID { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string RequiredByRequestedSegmentResponse { get; set; }
        public int SegmentResponseID { get; set; }
    
        public virtual MaterialDefinition MaterialDefinition { get; set; }
        public virtual SegmentResponse SegmentResponse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumableActualProperty> ConsumableActualProperty { get; set; }
    }
}
