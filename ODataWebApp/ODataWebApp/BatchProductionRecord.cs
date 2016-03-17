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
    
    public partial class BatchProductionRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BatchProductionRecord()
        {
            this.BatchProductionRecord11 = new HashSet<BatchProductionRecord>();
        }
    
        public int ID { get; set; }
        public int BatchProductionRecordEntry { get; set; }
        public System.DateTime PublishedDate { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string BatchID { get; set; }
        public string BatchProductionRecordSpec { get; set; }
        public string CampaginID { get; set; }
        public string ChangeIndication { get; set; }
        public string Delimiter { get; set; }
        public int EquipmentID { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public string Language { get; set; }
        public string LastChangedDate { get; set; }
        public int LotID { get; set; }
        public int MaterialDefinitionID { get; set; }
        public int PhysicalAssetID { get; set; }
        public string RecordStatus { get; set; }
        public string Version { get; set; }
        public int ChangeHistory { get; set; }
        public int Comments { get; set; }
        public int ControlRecipes { get; set; }
        public int DataSets { get; set; }
        public int Events { get; set; }
        public int MasterRecipes { get; set; }
        public int PersonnelIdentification { get; set; }
        public int OperationsDefinitions { get; set; }
        public int OperationsPerformances { get; set; }
        public int OperationsScedules { get; set; }
        public int ProductDefinitions { get; set; }
        public int ProductionPerformances { get; set; }
        public int ProductionScedules { get; set; }
        public int RecipeElements { get; set; }
        public int ResourceQualifications { get; set; }
        public int Samples { get; set; }
        public int WorkDirectives { get; set; }
        public int WorkMasters { get; set; }
        public int WorkPerformances { get; set; }
        public int WorkScedules { get; set; }
        public int BatchProductionRecord1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchProductionRecord> BatchProductionRecord11 { get; set; }
        public virtual BatchProductionRecord BatchProductionRecord2 { get; set; }
        public virtual BatchProductionRecordEntry BatchProductionRecordEntry1 { get; set; }
        public virtual Change Change { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual ControlRecipeRecord ControlRecipeRecord { get; set; }
        public virtual DataSet DataSet { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual Event Event { get; set; }
        public virtual MasterRecipeRecord MasterRecipeRecord { get; set; }
        public virtual MaterialDefinition MaterialDefinition { get; set; }
        public virtual MaterialLot MaterialLot { get; set; }
        public virtual OperationsDefinitionRecord OperationsDefinitionRecord { get; set; }
        public virtual OperationsPerformanceRecord OperationsPerformanceRecord { get; set; }
        public virtual OperationsSceduleRecord OperationsSceduleRecord { get; set; }
        public virtual PersonnelIdentificationManifest PersonnelIdentificationManifest { get; set; }
        public virtual PhysicalAsset PhysicalAsset { get; set; }
        public virtual ProductDefinitionRecord ProductDefinitionRecord { get; set; }
        public virtual ProductionPerformanceRecord ProductionPerformanceRecord { get; set; }
        public virtual ProductionSceduleRecord ProductionSceduleRecord { get; set; }
        public virtual RecipeElementRecord RecipeElementRecord { get; set; }
        public virtual ResourceQualificationsManifest ResourceQualificationsManifest { get; set; }
        public virtual Sample Sample { get; set; }
        public virtual WorkDirectiveRecord WorkDirectiveRecord { get; set; }
        public virtual WorkMasterRecord WorkMasterRecord { get; set; }
        public virtual WorkPerformanceRecord WorkPerformanceRecord { get; set; }
        public virtual WorkSceduleRecord WorkSceduleRecord { get; set; }
    }
}
