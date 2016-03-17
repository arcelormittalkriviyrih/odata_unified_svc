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
    
    public partial class OperationsPerfomance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OperationsPerfomance()
        {
            this.OperationsPerformanceRecord = new HashSet<OperationsPerformanceRecord>();
            this.OperationsResponse = new HashSet<OperationsResponse>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public string OperationsType { get; set; }
        public Nullable<int> OperationsScheduleID { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string PerformanceState { get; set; }
        public string PublishedDate { get; set; }
    
        public virtual OperationsScedule OperationsScedule { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OperationsPerformanceRecord> OperationsPerformanceRecord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OperationsResponse> OperationsResponse { get; set; }
    }
}
