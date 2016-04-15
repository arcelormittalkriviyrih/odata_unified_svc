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

		/// <summary>	The connection string settings. </summary>
		internal static readonly ConnectionStringSettings cConnectionStringSettings = WebConfigurationManager.ConnectionStrings[cConfigConnectionName];

		#endregion

		#region Fields

		/// <summary>	The current dynamic model assembly. </summary>
		private Assembly _currentDynamicModelAssembly = null;

		/// <summary>	The database compiled model. </summary>
		private static readonly DbCompiledModel _dbCompiledModel = null;

		/// <summary>	The dynamic actions. </summary>
		private static Type _dynamicActions = null;

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
				using (var dbReader = new DatabaseReader(cConnectionStringSettings.ConnectionString, cConnectionStringSettings.ProviderName, DynamicContext.cDefaultSchemaName))
				{
					var schema = dbReader.ReadAll();
					var dynamicClassFactory = new DynamicClassFactory();

					#region Read Tables

					List<string> failedTableColumns = new List<string>();

					foreach (var table in schema.Tables)
					{
						var property = new Dictionary<string, DynamicPropertyData>();
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
								DynamicPropertyData dynamicPropertyData = new DynamicPropertyData()
								{
									IsPrimaryKey = col.IsPrimaryKey,
									IsForeignKey = col.IsForeignKey,
									Order = table.Columns.IndexOf(col) + 1,
									Nullable = col.Nullable,
									Type = colType,
									MaxLength = col.Length
								};
								property.Add(col.Name, dynamicPropertyData);
							}
						}

						var tableType = CreateType(dynamicClassFactory, table.Name, property);
						var entity = modelBuilder.Entity(tableType);
					}

					#endregion

					#region Read Views

					List<string> failedViewColumns = new List<string>();

					foreach (var view in schema.Views)
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
								DynamicPropertyData dynamicPropertyData = new DynamicPropertyData()
								{
									IsPrimaryKey = col.IsPrimaryKey,
									IsForeignKey = col.IsForeignKey,
									Order = view.Columns.IndexOf(col) + 1,
									Nullable = col.Nullable,
									Type = colType,
									MaxLength = col.Length
								};
								property.Add(col.Name, dynamicPropertyData);
							}
						}

						var viewType = CreateType(dynamicClassFactory, view.Name, property);
						var entity = modelBuilder.Entity(viewType);
					}

					#endregion

					#region Read Actions

					Dictionary<string, DynamicMethodData> methods = new Dictionary<string, DynamicMethodData>();

					foreach (var function in schema.Functions)
					{
						if (function.ReturnType != null)
						{
							var dynamicMethodData = new DynamicMethodData();
							dynamicMethodData.FunctionType = FunctionType.ModelDefinedFunction;
							dynamicMethodData.ReturnType = typeof(Int32);
							dynamicMethodData.Schema = function.SchemaOwner;
							if (function.Arguments.Count > 0)
							{
								dynamicMethodData.Params = new Type[function.Arguments.Count];
								dynamicMethodData.ParamNames = new string[function.Arguments.Count];
								for (int i = 0; i < function.Arguments.Count; i++)
								{
									dynamicMethodData.Params[i] = function.Arguments[i].DataType.GetNetType();
									dynamicMethodData.ParamNames[i] = function.Arguments[i].Name;
								}
							}
							methods.Add(function.Name, dynamicMethodData);
						}
					}

					foreach (var procedure in schema.StoredProcedures)
					{
						//if (procedure..ReturnType != null)
						//if (procedure.Name != "ins_MaterialLotByController")
						{
							var dynamicMethodData = new DynamicMethodData();
							dynamicMethodData.FunctionType = FunctionType.ModelDefinedFunction;
							dynamicMethodData.ReturnType = typeof(Int32);
							dynamicMethodData.Schema = procedure.SchemaOwner;
							if (procedure.Arguments.Count > 0)
							{
								dynamicMethodData.Params = new Type[procedure.Arguments.Count];
								dynamicMethodData.ParamNames = new string[procedure.Arguments.Count];
								for (int i = 0; i < procedure.Arguments.Count; i++)
								{
									dynamicMethodData.Params[i] = procedure.Arguments[i].DataType.GetNetType();
									dynamicMethodData.ParamNames[i] = procedure.Arguments[i].Name;
								}
							}
							methods.Add(procedure.Name, dynamicMethodData);
						}
					}

					_dynamicActions = CreateTypeAction(dynamicClassFactory, "DbActions", methods);
					modelBuilder.AddFunctions(_dynamicActions, false);

					#endregion
				}

				#endregion

				#region Add Metadata object for user rules custom metadata

				var metadataObject = modelBuilder.Entity<DynamicMetadataObject>();
				metadataObject.HasKey(a => a.Name);
				metadataObject.Property(a => a.Type);
				metadataObject.Property(a => a.Schema); 

				#endregion

				var databaseModel = modelBuilder.Build(new System.Data.SqlClient.SqlConnection(cConnectionStringSettings.ConnectionString));
				compiledDatabaseModel = databaseModel.Compile();
			}
			catch (Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("CreateModel", exception);
				throw exception;
			}

			return compiledDatabaseModel;
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
		private static Type CreateType(DynamicClassFactory dynamicClassFactory, string name, Dictionary<string, DynamicPropertyData> property)
		{
			var dynamicType = dynamicClassFactory.CreateDynamicType<DynamicEntity>(name, property);
			return dynamicType;
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
