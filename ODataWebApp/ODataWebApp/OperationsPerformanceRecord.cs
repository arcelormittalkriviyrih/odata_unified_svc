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
    
    public partial class OperationsPerformanceRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OperationsPerformanceRecord()
        {
            this.BatchProductionRecord = new HashSet<BatchProductionRecord>();
        }
    
        public int ID { get; set; }
        public int BatchProductionRecordEntry { get; set; }
        public int OperationsPerformance { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchProductionRecord> BatchProductionRecord { get; set; }
        public virtual BatchProductionRecordEntry BatchProductionRecordEntry1 { get; set; }
        public virtual OperationsPerfomance OperationsPerfomance { get; set; }
    }
}
