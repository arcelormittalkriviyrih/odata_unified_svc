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
    
    public partial class OrderedData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderedData()
        {
            this.DataValue = new HashSet<DataValue>();
        }
    
        public int ID { get; set; }
        public string OrderIndex { get; set; }
        public Nullable<System.DateTime> TimeValue { get; set; }
        public Nullable<int> DataSet { get; set; }
    
        public virtual DataSet DataSet1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataValue> DataValue { get; set; }
    }
}
