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
    
    public partial class WorkMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkMaster()
        {
            this.JobOrder = new HashSet<JobOrder>();
            this.WorkDirective = new HashSet<WorkDirective>();
            this.WorkMasterCapability = new HashSet<WorkMasterCapability>();
            this.WorkMasterRecord = new HashSet<WorkMasterRecord>();
        }
    
        public int WorkMaster1 { get; set; }
        public Nullable<int> WorkDefinitionInformation { get; set; }
        public int WorkDefinitionID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobOrder> JobOrder { get; set; }
        public virtual WorkDefinition WorkDefinition { get; set; }
        public virtual WorkDefinitionInformation WorkDefinitionInformation1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkDirective> WorkDirective { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkMasterCapability> WorkMasterCapability { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkMasterRecord> WorkMasterRecord { get; set; }
    }
}
