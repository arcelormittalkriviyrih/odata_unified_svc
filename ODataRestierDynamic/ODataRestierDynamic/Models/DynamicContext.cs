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

namespace ODataRestierDynamic.Models
{
	public partial class DynamicContext : DbContext
	{
		#region Const

		public const string cDbContextKey = "Microsoft.Restier.EntityFramework.DbContext";

		private const string cConfigConnectionName = "DefaultDataSource"; //"B2MML-BatchML";

		public const string cDefaultSchemaName = "dbo";

		private static readonly ConnectionStringSettings cConnectionStringSettings = WebConfigurationManager.ConnectionStrings[cConfigConnectionName];

		#endregion

		#region Fields

		private Assembly _currentDynamicModelAssembly = null;

		private static readonly DbCompiledModel _dbCompiledModel = null;

		private static Type _dynamicActions = null;

		#endregion

		#region Property

		public Type DynamicActions
		{
			get
			{
				return _dynamicActions;
			}
		}

		#endregion

		#region Constructor

		static DynamicContext()
		{
			_dbCompiledModel = CreateModel();
		}

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

		private static DbCompiledModel CreateModel()
		{
			// Create a model + register types to it.
			var modelBuilder = new DbModelBuilder();
			modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>(); //For generating Entities without 's' at the end

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
						//if (!cTestTableNames.Contains(table.Name))
						//	continue;

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

						try
						{
							var tableType = CreateType(dynamicClassFactory, table.Name, property);
							var entity = modelBuilder.Entity(tableType);
							//foreach (var item in property)
							//{
							//	modelBuilder.Entity<String>().Property(p => p == item.Key).IsOptional();

							//	if (item.Value.Nullable)
							//		entity.TypeConfiguration.Property(p => p == item.Key).IsOptional();
							//}

							//_tables.Add(table.Name, tableType);
						}
						catch
						{
							throw;
						}
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
						//_views.Add(view.Name, viewType);
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
			}
			catch
			{
				throw;
			}

			DbCompiledModel compiledDatabaseModel = null;

			try
			{
				var databaseModel = modelBuilder.Build(new System.Data.SqlClient.SqlConnection(cConnectionStringSettings.ConnectionString));
				compiledDatabaseModel = databaseModel.Compile();
			}
			catch
			{
				throw;
			}

			return compiledDatabaseModel;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			throw new UnintentionalCodeFirstException();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			//TODO: Unload Dynamic dll or kill process???
			_currentDynamicModelAssembly = null;
		}

		private static Type CreateType(DynamicClassFactory dynamicClassFactory, string name, Dictionary<string, DynamicPropertyData> property)
		{
			var dynamicType = dynamicClassFactory.CreateDynamicType<DynamicEntity>(name, property);
			return dynamicType;
		}

		private static Type CreateTypeAction(DynamicClassFactory dynamicClassFactory, string name, Dictionary<string, DynamicMethodData> methods)
		{
			var dynamicType = dynamicClassFactory.CreateDynamicTypeAction<DynamicAction>(name, methods);
			return dynamicType;
		}

		public Type GetModelType(string name)
		{
			string lvNamespace = string.Format("{0}.{1}", DynamicClassFactory.cDefaultNamespace, name);
			Type type = _currentDynamicModelAssembly.GetType(lvNamespace);
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			//var dynamicModelAssembly = loadedAssemblies.LastOrDefault(assembly => assembly.GetName().Name == DynamicClassFactory.cDefaultNamespace);
			//Type type = dynamicModelAssembly.GetType(lvNamespace);

			return type;
		}

		public bool TryGetRelevantType(string name, out Type relevantType)
		{
			relevantType = GetModelType(name);

			return true;
		}

		public void LogQuery(string query)
		{
			System.Diagnostics.Debug.Write(query);
		}

		#endregion
	}
}
