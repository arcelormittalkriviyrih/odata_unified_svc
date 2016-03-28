using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;
using MagicDbModelBuilder;
using System.Data.Entity.Infrastructure;
using ODataWebApp.DynamicFactory;
using System.Web.OData.Builder;
using System.Reflection;
using DatabaseSchemaReader;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ODataWebApp.Models
{
	public partial class DynamicContext : DbContext
	{
		#region Const

		public const string cConfigConnectionName = "TestEntities";//"B2MML-BatchML";

		public const string cDefaultSchemaName = "dbo";

		#endregion

		#region Fields

		private Dictionary<string, Type> _tables = new Dictionary<string, Type>();
		private Dictionary<string, Type> _views = new Dictionary<string, Type>();
		private Dictionary<string, Type> _actions = new Dictionary<string, Type>();

		#endregion

		#region Property

		#endregion

		#region Constructor

		public DynamicContext()
			: base("name=" + cConfigConnectionName)
		{
			Database.SetInitializer(new NullDatabaseInitializer<DynamicContext>()); // Never create a database
			Database.Log = LogQuery;
		}

		#endregion

		#region Methods

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();

			// https://www.nuget.org/packages/DatabaseSchemaReader/
			using (var dbReader = new DatabaseReader(this.Database.Connection.ConnectionString, "System.Data.SqlClient", cDefaultSchemaName))
			{
				var schema = dbReader.ReadAll();
				var dynamicClassFactory = new DynamicClassFactory();

				#region Read Tables

				List<string> failedTableColumns = new List<string>();

				foreach (var table in schema.Tables)
				{
					if (schema.Tables[0] != table)
						continue;

					var property = new Dictionary<string, Type>();
					foreach (var col in table.Columns)
					{
						if (col.DataType == null)
						{
							failedTableColumns.Add(string.Format("{0} - {1}", table.Name, col.ToString()));
						}
						else
						{
							property.Add(col.Name, Type.GetType(col.DataType.NetDataType));
						}

						if (col.IsPrimaryKey)
						{
							//tableType.AddKeys(prop);
						}
					}

					var type = CreateType(dynamicClassFactory, table.Name, property);
					this.AddTable(table.Name, type);
					var entity = modelBuilder.Entity(type);

					foreach (var pi in type.GetProperties())
					{
						try
						{
							var column = table.Columns.Find(x => x.Name == pi.Name);

							if (column != null && column.IsPrimaryKey)
								entity.HasKey(pi.PropertyType, pi.Name);
							else if (pi.PropertyType == typeof(string))
								entity.StringProperty(pi.Name);
							else
								entity.PrimitiveProperty(pi.PropertyType, pi.Name);
						}
						catch
						{ 
						
						}
					}

					entity.TypeConfiguration.ToTable(table.Name, cDefaultSchemaName);
					entity.TypeConfiguration.HasEntitySetName(table.Name);
					//modelBuilder.Entity<Type>()
					//entity.ToTable(table.Name, cDefaultSchemaName);
					//entity.HasEntitySetName("TestTable");
				}

				#endregion
			}
		}

		public void AddTable(string name, Type type)
		{
			this._tables.Add(type.Name, type);
		}

		public void AddView(string name, Type type)
		{
			this._views.Add(type.Name, type);
		}

		public void AddAction(string name, Type type)
		{
			this._actions.Add(type.Name, type);
		}

		public Type GetModelType(string name)
		{
			Type type = null;

			if (this._tables.ContainsKey(name))
			{
				type = this._tables[name];
			}

			if (this._views.ContainsKey(name))
			{
				type = this._views[name];
			}

			if (this._actions.ContainsKey(name))
			{
				type = this._actions[name];
			}

			return type;
		}

		public bool TryGetRelevantType(string name, out Type relevantType)
		{
			relevantType = null;

			if (this._tables.ContainsKey(name))
			{
				relevantType = this._tables[name];
				return true;
			}

			if (this._views.ContainsKey(name))
			{
				relevantType = this._views[name];
				return true;
			}

			if (this._actions.ContainsKey(name))
			{
				relevantType = this._actions[name];
				return true;
			}

			return false;
		}

		private Type CreateType(DynamicClassFactory dynamicClassFactory, string name, Dictionary<string, Type> property)
		{
			var dynamicType = dynamicClassFactory.CreateDynamicType<DynamicEntity>(name, property);
			return dynamicType;
		}

		public void LogQuery(string query)
		{
			System.Diagnostics.Debug.Write(query);
		}

		#endregion
	}

	public static class Extensions
	{
		public static MethodInfo GetGenericMethod(this Type t, string name)
		{
			MethodInfo foo1 = (from m in t.GetMethods()
							   where m.Name == name
							   select m).First();

			return foo1;
		}
	}
}
