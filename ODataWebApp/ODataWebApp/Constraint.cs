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
    
    public partial class Constraint
    {
        public int ID { get; set; }
        public string Condition { get; set; }
        public Nullable<int> EquipmentRequirement { get; set; }
    
        public virtual BatchEquipmentRequirement BatchEquipmentRequirement { get; set; }
    }
}
