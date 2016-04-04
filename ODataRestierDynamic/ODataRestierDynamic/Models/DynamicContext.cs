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

namespace ODataRestierDynamic.Models
{
	public partial class DynamicContext : DbContext
	{
		#region Const

		private const string cConfigConnectionName = "DefaultDataSource"; //"B2MML-BatchML";

		private const string cDefaultSchemaName = "dbo";

		private static readonly ConnectionStringSettings cConnectionStringSettings = WebConfigurationManager.ConnectionStrings[cConfigConnectionName];

		#endregion

		#region Fields

		private Assembly _currentDynamicModelAssembly = null;

		#endregion

		#region Property



		#endregion

		#region Constructor

		public DynamicContext()
			: base(cConnectionStringSettings.ConnectionString, CreateModel())
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
							//if (col.Nullable)
							//{
							//	if (col.DataType.IsInt)
							//	{
							//		property.Add(col.Name, typeof(int?).GetType());
							//	}
							//	else if (col.DataType.IsDateTime)
							//	{
							//		property.Add(col.Name, typeof(Nullable<DateTime>).GetType());
							//	}
							//	else if (col.DataType.IsFloat)
							//	{
							//		property.Add(col.Name, typeof(Nullable<float>).GetType());
							//	}
							//	else if (col.DataType.IsNumeric)
							//	{
							//		property.Add(col.Name, typeof(Nullable<decimal>).GetType());
							//	}
							//	else if (col.DataType.NetDataTypeCSharpName == "bool")
							//	{
							//		property.Add(col.Name, typeof(Nullable<bool>).GetType());
							//	}
							//	else
							//	{
							//		property.Add(col.Name, Type.GetType(col.DataType.NetDataType));
							//	}
							//}
							//else
							{
								Type colType = Type.GetType(col.DataType.NetDataType);
								DynamicPropertyData dynamicPropertyData = new DynamicPropertyData()
								{
									IsPrimaryKey = col.IsPrimaryKey,
									IsForeignKey = col.IsForeignKey,
									Order = table.Columns.IndexOf(col) + 1,
									Nullable = col.Nullable,
									Type = colType
								};
								property.Add(col.Name, dynamicPropertyData);
							}
						}
					}

					try
					{
						var tableType = CreateType(dynamicClassFactory, table.Name, property);
						var entity = modelBuilder.Entity(tableType);
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
							int colNum = view.Columns.IndexOf(col) + 1;
							DynamicPropertyData dynamicPropertyData = new DynamicPropertyData()
							{
								IsPrimaryKey = colNum == 1,
								Order = colNum,
								Nullable = col.Nullable,
								Type = colType
							};
							property.Add(col.Name, dynamicPropertyData);
						}
					}

					var viewType = CreateType(dynamicClassFactory, view.Name, property);
					var entity = modelBuilder.Entity(viewType);
					//_views.Add(view.Name, viewType);
				}

				#endregion
			}

			#endregion

			var databaseModel = modelBuilder.Build(new System.Data.SqlClient.SqlConnection(cConnectionStringSettings.ConnectionString));
			var compiledDatabaseModel = databaseModel.Compile();

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
