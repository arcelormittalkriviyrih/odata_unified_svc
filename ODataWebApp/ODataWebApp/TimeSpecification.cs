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
    
    public partial class TimeSpecification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TimeSpecification()
        {
            this.DataSet = new HashSet<DataSet>();
        }
    
        public int ID { get; set; }
        public string Relative { get; set; }
        public Nullable<System.DateTimeOffset> OffsetTime { get; set; }
        public Nullable<System.DateTimeOffset> OffsetTimeFormat { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataSet> DataSet { get; set; }
    }
}
