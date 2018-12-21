using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;
using MagicDbModelBuilder;
using System.Data.Entity.Infrastructure;
using System.Web.OData.Builder;
using System.Reflection;
using DatabaseSchemaReader;
using System.Data.Entity.ModelConfiguration.Conventions;
using ODataRestierDynamic.DynamicFactory;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;
using System.Configuration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using EntityFramework.Functions;
using ODataRestierDynamic.Log;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using ODataRestierDynamic.CustomAttributes;
using DatabaseSchemaReader.DataSchema;
using System.Reflection.Emit;
using ODataRestierDynamic.Helpers;

namespace ODataRestierDynamic.Models
{
    /// <summary>
    /// A DynamicContext instance represents a combination of the Unit Of Work and Repository
    ///    patterns such that it can be used to query from a database and group together changes that
    ///    will then be written back to the store as a unit. DynamicContext is conceptually similar
    ///    to DbContext.
    /// </summary>

    public partial class DynamicContext : DbContext
    {
        #region Const

        /// <summary>	The database context key. </summary>
        public const string cDbContextKey = "Microsoft.Restier.EntityFramework.DbContext";

        /// <summary>	"B2MML-BatchML"; </summary>
        private const string cConfigConnectionName = "DefaultDataSource";

        /// <summary>	The default schema name. </summary>
        public const string cDefaultSchemaName = "dbo";

        /// <summary>	The default schema name. </summary>
        private const string cDatabaseCommandTimeoutName = "DatabaseCommandTimeout";

        /// <summary>	The connection string settings. </summary>
        internal static readonly ConnectionStringSettings cConnectionStringSettings = WebConfigurationManager.ConnectionStrings[cConfigConnectionName];

        /// <summary>	The database command timeout in seconds. </summary>
        private static readonly int cDatabaseCommandTimeout = int.Parse(WebConfigurationManager.AppSettings[cDatabaseCommandTimeoutName]);

        #endregion

        #region Fields

        /// <summary>	The current dynamic model assembly. </summary>
        private Assembly _currentDynamicModelAssembly = null;

        /// <summary>	The database compiled model. </summary>
        private static readonly DbCompiledModel _dbCompiledModel = null;

        /// <summary>	The dynamic actions. </summary>
        private static Type _dynamicActions = null;

        /// <summary>	The dynamic action methods. </summary>
        private static Dictionary<string, DynamicMethodData> _dynamicActionMethods = new Dictionary<string, DynamicMethodData>();

        #endregion

        #region Property

        /// <summary>	Gets the dynamic actions. </summary>
        ///
        /// <value>	The dynamic actions. </value>
        public Type DynamicActions
        {
            get
            {
                return _dynamicActions;
            }
        }

        /// <summary>	Gets the dynamic action methods. </summary>
        ///
        /// <value>	The dynamic action methods. </value>
        public Dictionary<string, DynamicMethodData> DynamicActionMethods
        {
            get
            {
                return _dynamicActionMethods;
            }
        }

        #endregion

        #region Constructor

        /// <summary>	Static constructor. </summary>
        static DynamicContext()
        {
            _dbCompiledModel = CreateModel();
        }

        /// <summary>	Default constructor. </summary>
        public DynamicContext()
            : base(cConnectionStringSettings.ConnectionString, _dbCompiledModel)
        {
            //Database.SetInitializer(new NullDatabaseInitializer<DynamicContext>()); // Never create a database
            Database.SetInitializer<DynamicContext>(null); // Prevent creation of migration table
            Database.Log = LogQuery;
            Database.CommandTimeout = cDatabaseCommandTimeout;
            this._currentDynamicModelAssembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(assembly => assembly.GetName().Name == DynamicClassFactory.cDefaultNamespace);
        }

        #endregion

        #region Methods

        /// <summary>	Creates the model. </summary>
        ///
        /// <returns>	The new model. </returns>
        private static DbCompiledModel CreateModel()
        {
            // Create a model + register types to it.
            var modelBuilder = new DbModelBuilder();
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>(); //For generating Entities without 's' at the end
            DbCompiledModel compiledDatabaseModel = null;

            try
            {
                #region Read Schema and Add to Model

                // https://www.nuget.org/packages/DatabaseSchemaReader/
                using (var dbReader = new DatabaseReader(cConnectionStringSettings.ConnectionString, cConnectionStringSettings.ProviderName))
                {
                    var schema = dbReader.ReadAll();
                    var dynamicClassFactory = new DynamicClassFactory();

                    #region Read Tables

                    var tableBuildHelperList = new List<TableBuildHelper>();
                    List<string> failedTableColumns = new List<string>();
                    foreach (var table in schema.Tables)
                    {
                        try
                        {
                            var tableBuildHelper = new TableBuildHelper
                            {
                                Name = table.Name,
                                Properties = new List<TablePropertyBuildHelper>()
                            };

                            #region Field properties

                            foreach (var col in table.Columns)
                            {
                                if (col.DataType == null)
                                {
                                    failedTableColumns.Add(string.Format("{0} - {1}", table.Name, col.ToString()));
                                }
                                else
                                {
                                    Type colType = Type.GetType(col.DataType.NetDataType);
                                    if (col.Nullable)
                                    {
                                        if (col.DataType.IsInt)
                                        {
                                            colType = typeof(Nullable<int>);
                                        }
                                        else if (col.DataType.IsDateTime)
                                        {
                                            colType = typeof(Nullable<DateTime>);
                                        }
                                        else if (col.DataType.IsFloat)
                                        {
                                            colType = typeof(Nullable<float>);
                                        }
                                        else if (col.DataType.IsNumeric)
                                        {
                                            colType = typeof(Nullable<decimal>);
                                        }
                                        else if (col.DataType.TypeName == "datetimeoffset")
                                        {
                                            colType = typeof(Nullable<DateTimeOffset>);
                                        }
                                        else if (col.DataType.NetDataTypeCSharpName == "bool")
                                        {
                                            colType = typeof(Nullable<bool>);
                                        }
                                    }

                                    //Sequense logic
                                    string sequenceScript = null;
                                    if (col.IsPrimaryKey && !string.IsNullOrEmpty(col.DefaultValue) && col.DefaultValue.StartsWith("(NEXT VALUE FOR"))
                                        sequenceScript = col.DefaultValue.Substring(1, col.DefaultValue.Length - 2);

                                    FieldPropertyData fieldPropertyData = new FieldPropertyData()
                                    {
                                        IsPrimaryKey = col.IsPrimaryKey,
                                        IsForeignKey = col.IsForeignKey,
                                        Order = table.Columns.IndexOf(col) + 1,
                                        Nullable = col.Nullable,
                                        Type = colType,
                                        MaxLength = col.Length,
                                        IsComputedID = col.IsPrimaryKey && col.IdentityDefinition == null,
                                        SequenceScript = sequenceScript,
                                        ColumnName = col.Name
                                    };

                                    string name = col.Name;
                                    while (table.Name == name || tableBuildHelper.Properties.Exists(p => p.Name == name))
                                        name = name + "1";

                                    var tablePropertyBuildHelper = new TablePropertyBuildHelper
                                    {
                                        Name = name,
                                        Data = fieldPropertyData
                                    };
                                    tableBuildHelper.Properties.Add(tablePropertyBuildHelper);
                                }
                            }

                            #endregion

                            //Make all existing foreign keys as primary key if entity has no primary key
                            if (tableBuildHelper.Properties.FirstOrDefault(x => ((FieldPropertyData)x.Data).IsPrimaryKey) == null)
                            {
                                var foreignRows = tableBuildHelper.Properties.Where(x => (((FieldPropertyData)x.Data).IsForeignKey)).ToList();
                                foreignRows.ForEach(p => ((FieldPropertyData)p.Data).IsPrimaryKey = true);
                            }

                            var tableTypeBuilder = CreateTypeBuilder(dynamicClassFactory, table.SchemaOwner, table.Name, null);
                            tableBuildHelper.TypeBuilder = tableTypeBuilder;
                            tableBuildHelperList.Add(tableBuildHelper);
                        }
                        catch (Exception exception)
                        {
                            DynamicLogger.Instance.WriteLoggerLogError(string.Format("CreateModel: table '{0}'", table.Name), exception);
                        }
                    }

                    #region Navigation properties

                    foreach (var table in schema.Tables)
                    {
                        #region Foreign Keys

                        foreach (var foreignKey in table.ForeignKeys)
                        {
                            try
                            {
                                var tableBuildHelper = tableBuildHelperList.Find(table.Name);
                                var columnType = tableBuildHelperList.Find(foreignKey.RefersToTable).TypeBuilder;

                                //Foreign Key property
                                {
                                    ForeignKeyPropertyData foreignKeyPropertyData = new ForeignKeyPropertyData()
                                    {
                                        Type = columnType,
                                        ColumnName = tableBuildHelper.Properties.Find(foreignKey.Columns[0]).Name
                                    };

                                    string propName = foreignKey.RefersToTable;
                                    while (table.Name == propName || tableBuildHelper.Properties.Exists(propName))
                                    {
                                        propName = propName + "1";
                                    }

                                    var tablePropertyBuildHelper = new TablePropertyBuildHelper
                                    {
                                        Name = propName,
                                        Data = foreignKeyPropertyData
                                    };
                                    tableBuildHelper.Properties.Add(tablePropertyBuildHelper);
                                }

                                var fkTableBuildHelper = tableBuildHelperList.Find(foreignKey.RefersToTable);
                                var fkColumnType = typeof(ICollection<>).MakeGenericType(tableBuildHelper.TypeBuilder);

                                //if (table.Name == "BatchProductionRecord") //foreignKey.Columns[0] == "Events")
                                //Inverse property
                                {
                                    InversePropertyData inversePropertyData = new InversePropertyData()
                                    {
                                        Type = fkColumnType,
                                        ColumnName = tableBuildHelper.Properties.Last().Name //propName
                                    };

                                    string propName = foreignKey.TableName;
                                    while (foreignKey.RefersToTable == propName || fkTableBuildHelper.Properties.Exists(propName))
                                    {
                                        propName = propName + "1";
                                    }

                                    var tablePropertyBuildHelper = new TablePropertyBuildHelper
                                    {
                                        Name = propName,
                                        Data = inversePropertyData
                                    };
                                    fkTableBuildHelper.Properties.Add(tablePropertyBuildHelper);
                                }
                            }
                            catch (Exception exception)
                            {
                                DynamicLogger.Instance.WriteLoggerLogError(string.Format("CreateModel: foreignKey '{0}'", foreignKey.Name), exception);
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region Create properties and table types from type builder and add to DB model

                    foreach (var table in tableBuildHelperList)
                    {
                        foreach (var property in table.Properties)
                        {
                            try
                            {
                                dynamicClassFactory.CreateProperty(table.TypeBuilder, new KeyValuePair<string, DynamicPropertyData>(property.Name, property.Data), null);
                            }
                            catch (Exception exception)
                            {
                                DynamicLogger.Instance.WriteLoggerLogError(string.Format("CreateModel: property '{0}'", property.Name), exception);
                            }
                        }

                        try
                        {
                            var tableType = table.TypeBuilder.CreateType();
                            var entity = modelBuilder.Entity(tableType);
                        }
                        catch (Exception exception)
                        {
                            DynamicLogger.Instance.WriteLoggerLogError("CreateModel", exception);
                        }
                    }

                    #endregion

                    #endregion

                    #region Read Views

                    List<string> failedViewColumns = new List<string>();
                    foreach (var view in schema.Views)
                    {
                        try
                        {
                            AddViewToModel(modelBuilder, dynamicClassFactory, failedViewColumns, view);
                        }
                        catch (Exception exception)
                        {
                            DynamicLogger.Instance.WriteLoggerLogError(string.Format("CreateModel: view '{0}'", view.Name), exception);
                        }
                    }

                    #endregion

                    #region Read Actions

                    AddActionsToModel(modelBuilder, schema, dynamicClassFactory);

                    #endregion
                }

                #endregion

                #region Add Metadata object for user rules custom metadata

                try
                {
                    var metadataObject = modelBuilder.Entity<DynamicMetadataObject>();
                    metadataObject.HasKey(a => a.Name);
                    metadataObject.Property(a => a.Type);
                    metadataObject.Property(a => a.Schema);
                }
                catch (Exception exception)
                {
                    DynamicLogger.Instance.WriteLoggerLogError("CreateModel", exception);
                }

                #endregion

                #region Add service info object to model

                try
                {
                    var serviceInfoObject = modelBuilder.Entity<DynamicServiceInfoObject>();
                    serviceInfoObject.HasKey(a => a.IISVersion);
                    serviceInfoObject.Property(a => a.TargetFramework);
                    serviceInfoObject.Property(a => a.AppDomainAppPath);
                    serviceInfoObject.Property(a => a.AssemblyDictionary);
                }
                catch (Exception exception)
                {
                    DynamicLogger.Instance.WriteLoggerLogError("CreateModel", exception);
                }

                #endregion

                var databaseModel = modelBuilder.Build(new System.Data.SqlClient.SqlConnection(cConnectionStringSettings.ConnectionString));
                compiledDatabaseModel = databaseModel.Compile();
            }
            catch (Exception exception)
            {
                DynamicLogger.Instance.WriteLoggerLogError("CreateModel", exception);
                throw;
            }

            return compiledDatabaseModel;
        }

        /// <summary>	Adds a view to model. </summary>
        ///
        /// <param name="modelBuilder">		  	The builder that defines the model for the context being
        /// 									created. </param>
        /// <param name="dynamicClassFactory">	The dynamic class factory. </param>
        /// <param name="failedViewColumns">  	The failed view columns. </param>
        /// <param name="view">				  	The view. </param>
        ///
        /// <returns>	A Type. </returns>
        private static Type AddViewToModel(
            DbModelBuilder modelBuilder,
            DynamicClassFactory dynamicClassFactory,
            List<string> failedViewColumns,
            DatabaseView view)
        {
            var property = new Dictionary<string, DynamicPropertyData>();
            foreach (var col in view.Columns)
            {
                if (col.DataType == null)
                {
                    failedViewColumns.Add(string.Format("{0} - {1}", view.Name, col.ToString()));
                }
                else
                {
                    Type colType = Type.GetType(col.DataType.NetDataType);
                    if (col.Nullable)
                    {
                        if (col.DataType.IsInt)
                        {
                            colType = typeof(Nullable<int>);
                        }
                        else if (col.DataType.IsDateTime)
                        {
                            colType = typeof(Nullable<DateTime>);
                        }
                        else if (col.DataType.IsFloat)
                        {
                            colType = typeof(Nullable<float>);
                        }
                        else if (col.DataType.IsNumeric)
                        {
                            colType = typeof(Nullable<decimal>);
                        }
                        else if (col.DataType.TypeName == "datetimeoffset")
                        {
                            colType = typeof(Nullable<DateTimeOffset>);
                        }
                        else if (col.DataType.NetDataTypeCSharpName == "bool")
                        {
                            colType = typeof(Nullable<bool>);
                        }
                    }
                    DynamicPropertyData dynamicPropertyData = new FieldPropertyData()
                    {
                        IsPrimaryKey = col.IsPrimaryKey,
                        IsForeignKey = col.IsForeignKey,
                        Order = view.Columns.IndexOf(col) + 1,
                        Nullable = col.Nullable,
                        Type = colType,
                        MaxLength = col.Length,
                        ColumnName = col.Name
                    };

                    string name = col.Name;
                    while (property.ContainsKey(name) || view.Name == name)
                    {
                        name = name + "1";
                    }
                    property.Add(name, dynamicPropertyData);
                }
            }

            //Make all existing foreign keys as primary key if entity has no primary key
            if (property.Values.FirstOrDefault(x => ((FieldPropertyData)x).IsPrimaryKey) == null)
            {
                var foreignRows = property.Values.Where(x => (((FieldPropertyData)x).IsForeignKey)).ToList();
                foreignRows.ForEach(p => ((FieldPropertyData)p).IsPrimaryKey = true);
            }

            var viewType = CreateType(dynamicClassFactory, view.SchemaOwner, view.Name, property);
            var entity = modelBuilder.Entity(viewType);
            modelBuilder.HasDefaultSchema("Ordering");
            var methodInfoMap = entity.TypeConfiguration.GetType().GetMethod("MapToStoredProcedures", new Type[] { });
            methodInfoMap.Invoke(entity.TypeConfiguration, new object[] { });

            return viewType;
        }

        /// <summary>	Adds the actions to model. </summary>
        ///
        /// <param name="modelBuilder">		  	The builder that defines the model for the context being
        /// 									created. </param>
        /// <param name="schema">			  	The schema. </param>
        /// <param name="dynamicClassFactory">	The dynamic class factory. </param>
        ///
        /// <returns>	A Type. </returns>
        private static void AddActionsToModel(
            DbModelBuilder modelBuilder,
            DatabaseSchema schema,
            DynamicClassFactory dynamicClassFactory)
        {
            foreach (var function in schema.Functions)
            {
                try
                {
                    if (function.ReturnType != null)
                    {
                        var dynamicMethodData = new DynamicMethodData
                        {
                            FunctionType = FunctionType.ModelDefinedFunction,
                            ReturnType = typeof(Int32),
                            Schema = function.SchemaOwner
                        };
                        if (function.Arguments.Count > 0)
                        {
                            dynamicMethodData.Params = new DynamicParameterData[function.Arguments.Count];
                            for (int i = 0; i < function.Arguments.Count; i++)
                            {
                                dynamicMethodData.Params[i] = new DynamicParameterData()
                                {
                                    Name = function.Arguments[i].Name,
                                    Type = function.Arguments[i].DataType.GetNetType(),
                                    isIn = function.Arguments[i].In,
                                    isOut = function.Arguments[i].Out,
                                    Length = function.Arguments[i].Length
                                };
                            }
                        }
                        _dynamicActionMethods.Add(function.Name, dynamicMethodData);
                    }
                }
                catch (Exception exception)
                {
                    DynamicLogger.Instance.WriteLoggerLogError(string.Format("AddActionsToModel: function '{0}'", function.Name), exception);
                }
            }

            foreach (var procedure in schema.StoredProcedures)
            {
                try
                {
                    var dynamicMethodData = new DynamicMethodData
                    {
                        FunctionType = FunctionType.ModelDefinedFunction,
                        ReturnType = typeof(Int32),
                        Schema = procedure.SchemaOwner
                    };
                    if (procedure.Arguments.Count > 0)
                    {
                        dynamicMethodData.Params = new DynamicParameterData[procedure.Arguments.Count];
                        for (int i = 0; i < procedure.Arguments.Count; i++)
                        {
                            dynamicMethodData.Params[i] = new DynamicParameterData()
                            {
                                Name = procedure.Arguments[i].Name,
                                Type = procedure.Arguments[i].DataType.GetNetType(),
                                isIn = procedure.Arguments[i].In,
                                isOut = procedure.Arguments[i].Out,
                                Length = procedure.Arguments[i].Length
                            };
                        }
                    }
                    _dynamicActionMethods.Add(procedure.Name, dynamicMethodData);
                }
                catch (Exception exception)
                {
                    DynamicLogger.Instance.WriteLoggerLogError(string.Format("AddActionsToModel: procedure '{0}'", procedure.Name), exception);
                }
            }

            try
            {
                _dynamicActions = CreateTypeAction(dynamicClassFactory, "DbActions", _dynamicActionMethods);
                // https://www.nuget.org/packages/EntityFramework.Functions
                modelBuilder.AddFunctions(_dynamicActions, false);
            }
            catch (Exception exception)
            {
                DynamicLogger.Instance.WriteLoggerLogError("AddActionsToModel", exception);
            }
        }

        /// <summary>	Saves the changes asynchronous. </summary>
        ///
        /// <param name="cancellationToken">	A <see cref="T:System.Threading.CancellationToken"/> to
        /// 									observe while waiting for the task to complete. </param>
        ///
        /// <returns>	A System.Threading.Tasks.Task&lt;int&gt; </returns>
        public override System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            var pendingChanges = ChangeTracker.Entries();
            foreach (var entry in pendingChanges)
            {
                if (entry.State == EntityState.Added)
                {
                    var entity = entry.Entity;
                    foreach (var property in entity.GetType().GetProperties())
                    {
                        var sequenceAttribute = (SequenceAttribute)property.GetCustomAttribute(typeof(SequenceAttribute));
                        if (sequenceAttribute != null)
                        {
                            string sequenceScript = sequenceAttribute.Script;
                            var value = property.GetValue(entity);
                            if (value is int && (int)value == 0)
                            {
                                value = this.GetNextID(sequenceScript);
                                property.SetValue(entity, value);
                            }
                        }
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>	Gets the next identifier. </summary>
        ///
        /// <param name="sequenceScript">	The sequence script. </param>
        ///
        /// <returns>	The next identifier. </returns>
        private int GetNextID(string sequenceScript)
        {
            var objContext = ((IObjectContextAdapter)this).ObjectContext;
            var result = objContext.ExecuteStoreQuery<int>(string.Format("SELECT {0}", sequenceScript));

            return result.First();
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but before
        /// the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class such
        /// that the model can be further configured before it is locked down.
        /// </summary>
        ///
        /// <exception cref="UnintentionalCodeFirstException">	Thrown when an Unintentional Code First
        /// 													error condition occurs. </exception>
        ///
        /// <param name="modelBuilder">	The builder that defines the model for the context being created. </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        /// <summary>
        /// Disposes the context. The underlying
        /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext"/> is also disposed if it was
        /// created is by this context or ownership was passed to this context when this context was
        /// created. The connection to the database (<see cref="T:System.Data.Common.DbConnection"/>
        /// object) is also disposed if it was created is by this context or ownership was passed to this
        /// context when this context was created.
        /// </summary>
        ///
        /// <param name="disposing">	<c>true</c> to release both managed and unmanaged resources;
        /// 							<c>false</c> to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //TODO: Unload Dynamic dll or kill process???
            _currentDynamicModelAssembly = null;
        }

        /// <summary>	Creates a type. </summary>
        ///
        /// <param name="dynamicClassFactory">	The dynamic class factory. </param>
        /// <param name="name">				  	The name. </param>
        /// <param name="property">			  	The property. </param>
        ///
        /// <returns>	The new type. </returns>
        private static Type CreateType(DynamicClassFactory dynamicClassFactory, string schema, string name, Dictionary<string, DynamicPropertyData> property)
        {
            var dynamicType = dynamicClassFactory.CreateDynamicType<DynamicEntity>(schema, name, property);
            return dynamicType;
        }

        /// <summary>	Creates a type builder. </summary>
        ///
        /// <param name="dynamicClassFactory">	The dynamic class factory. </param>
        /// <param name="name">				  	The name. </param>
        /// <param name="property">			  	The property. </param>
        ///
        /// <returns>	The new type builder. </returns>
        private static TypeBuilder CreateTypeBuilder(DynamicClassFactory dynamicClassFactory, string schema, string name, Dictionary<string, DynamicPropertyData> property)
        {
            var dynamicTypeBuilder = dynamicClassFactory.CreateDynamicTypeBuilder<DynamicEntity>(schema, name, property);
            return dynamicTypeBuilder;
        }

        /// <summary>	Creates type action. </summary>
        ///
        /// <param name="dynamicClassFactory">	The dynamic class factory. </param>
        /// <param name="name">				  	The name. </param>
        /// <param name="methods">			  	The methods. </param>
        ///
        /// <returns>	The new type action. </returns>
        private static Type CreateTypeAction(DynamicClassFactory dynamicClassFactory, string name, Dictionary<string, DynamicMethodData> methods)
        {
            var dynamicType = dynamicClassFactory.CreateDynamicTypeAction<DynamicAction>(name, methods);
            return dynamicType;
        }

        /// <summary>	Gets model type. </summary>
        ///
        /// <param name="name">	The name. </param>
        ///
        /// <returns>	The model type. </returns>
        public Type GetModelType(string name)
        {
            string lvNamespace = string.Format("{0}.{1}", DynamicClassFactory.cDefaultNamespace, name);
            Type type = _currentDynamicModelAssembly.GetType(lvNamespace);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            //var dynamicModelAssembly = loadedAssemblies.LastOrDefault(assembly => assembly.GetName().Name == DynamicClassFactory.cDefaultNamespace);
            //Type type = dynamicModelAssembly.GetType(lvNamespace);

            if (type == null)
            {
                PropertyInfo property = this.GetType().GetProperty(name);
                if (property != null)
                {
                    object strongTypedDbSet = property.GetValue(this);
                    type = strongTypedDbSet.GetType().GetGenericArguments()[0];
                }
            }

            return type;
        }

        /// <summary>	Attempts to get relevant type from the given data. </summary>
        ///
        /// <param name="name">		   	The name. </param>
        /// <param name="relevantType">	[out] Type of the relevant. </param>
        ///
        /// <returns>	true if it succeeds, false if it fails. </returns>
        public bool TryGetRelevantType(string name, out Type relevantType)
        {
            relevantType = GetModelType(name);

            return true;
        }

        /// <summary>	Logs a query. </summary>
        ///
        /// <param name="query">	The query. </param>
        public void LogQuery(string query)
        {
            System.Diagnostics.Debug.Write(query);
        }

        #endregion
    }
}
