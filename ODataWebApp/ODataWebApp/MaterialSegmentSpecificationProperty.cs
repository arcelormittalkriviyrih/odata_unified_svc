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
    
    public partial class MaterialSegmentSpecificationProperty
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> MaterialSegmentSpecification { get; set; }
    
        public virtual MaterialSegmentSpecificftion MaterialSegmentSpecificftion { get; set; }
    }
}
