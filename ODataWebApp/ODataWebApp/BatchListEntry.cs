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
    
    public partial class BatchListEntry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BatchListEntry()
        {
            this.BatchListEntry11 = new HashSet<BatchListEntry>();
            this.BatchParameter = new HashSet<BatchParameter>();
        }
    
        public int ID { get; set; }
        public string Description { get; set; }
        public Nullable<int> BatchListEntryType { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public Nullable<int> ExternalID { get; set; }
        public Nullable<int> RecipeID { get; set; }
        public string RecipeVersion { get; set; }
        public Nullable<int> BatchID { get; set; }
        public Nullable<int> LotID { get; set; }
        public string CampaingID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string OrderID { get; set; }
        public string StartCondition { get; set; }
        public Nullable<System.DateTimeOffset> RequestedStartTime { get; set; }
        public Nullable<System.DateTimeOffset> ActualStartTime { get; set; }
        public Nullable<System.DateTimeOffset> RequestedEndTime { get; set; }
        public Nullable<System.DateTimeOffset> ActualEndTime { get; set; }
        public string BatchPriority { get; set; }
        public string RequestedBatchSize { get; set; }
        public string ActualBatchSize { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Note { get; set; }
        public Nullable<int> EquipmentID { get; set; }
        public Nullable<int> EquipmentClassID { get; set; }
        public Nullable<int> ActualEquipmentID { get; set; }
        public Nullable<int> BatchListEntry1 { get; set; }
        public Nullable<int> BatchList { get; set; }
    
        public virtual BatchList BatchList1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchListEntry> BatchListEntry11 { get; set; }
        public virtual BatchListEntry BatchListEntry2 { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual EquipmentClass EquipmentClass { get; set; }
        public virtual GRecipe GRecipe { get; set; }
        public virtual GRecipeProductInformation GRecipeProductInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchParameter> BatchParameter { get; set; }
    }
}
