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
    
    public partial class ConsumableActualProperty
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string RequiredByRequestedSegmentResponse { get; set; }
        public Nullable<int> ConsumableActual { get; set; }
    
        public virtual ConsumableActual ConsumableActual1 { get; set; }
    }
}
