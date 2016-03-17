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
    
    public partial class WorkDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkDefinition()
        {
            this.OpEquipmentSpecification = new HashSet<OpEquipmentSpecification>();
            this.OpMaterialSpecification = new HashSet<OpMaterialSpecification>();
            this.OpPersonnelSpecification = new HashSet<OpPersonnelSpecification>();
            this.OpPhysicalAssetSpecification = new HashSet<OpPhysicalAssetSpecification>();
            this.WorkDirective = new HashSet<WorkDirective>();
            this.WorkflowSpecificationNode = new HashSet<WorkflowSpecificationNode>();
            this.WorkMaster = new HashSet<WorkMaster>();
        }
    
        public int ID { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string HierarchyScope { get; set; }
        public string WorkType { get; set; }
        public Nullable<System.DateTime> Duration { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public Nullable<int> OperationsDefinitionID { get; set; }
        public string Parameter { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpEquipmentSpecification> OpEquipmentSpecification { get; set; }
        public virtual OperationsDefinition OperationsDefinition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpMaterialSpecification> OpMaterialSpecification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpPersonnelSpecification> OpPersonnelSpecification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpPhysicalAssetSpecification> OpPhysicalAssetSpecification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkDirective> WorkDirective { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkflowSpecificationNode> WorkflowSpecificationNode { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkMaster> WorkMaster { get; set; }
    }
}
