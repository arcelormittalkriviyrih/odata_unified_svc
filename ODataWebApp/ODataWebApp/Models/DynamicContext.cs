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
		#region Fields

		private Dictionary<string, Type> _tables = new Dictionary<string, Type>();
		private Dictionary<string, Type> _views = new Dictionary<string, Type>();
		private Dictionary<string, Type> _actions = new Dictionary<string, Type>();

		#endregion

		#region Property

		//public DbSet<TestTable> TestTable { get; set; }

		public DbSet<KEP_logger> KEP_logger { get; set; } 

		#endregion

		#region Constructor

		public DynamicContext()
			: base("name=KEPServer")//"name=B2MML-BatchML")
		{
			Database.SetInitializer(new NullDatabaseInitializer<DynamicContext>()); // Never create a database
			Database.Log = LogQuery;
		}

		#endregion

		#region Methods

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			var test2 = modelBuilder.Entity<TestTable>();
			test2.ToTable("TestTable", "dbo");
			test2.HasEntitySetName("TestTable");

			var result = this.Set<TestTable>();

			//modelBuilder.Entity<TestTable>().Map(m =>
			//{
			//	m.Properties(t => new { t.ID, t.Name }); 
			//	//m.MapInheritedProperties();
			//	m.ToTable("TestTable", "dbo");
			//});

			//((EntityTypeConfiguration)test2.Configuration).IsExplicitEntity = false;

			base.OnModelCreating(modelBuilder);

			//GetType().Se

			//base.OnModelCreating(modelBuilder);

			//var entityMethod = modelBuilder.GetType().GetMethod("Entity");

			//// https://www.nuget.org/packages/DatabaseSchemaReader/
			//using (var dbReader = new DatabaseReader(this.Database.Connection.ConnectionString, "System.Data.SqlClient", "dbo"))
			//{
			//	var schema = dbReader.ReadAll();
			//	var dcf = new DynamicClassFactory();

			//	#region Read Tables

			//	foreach (var table in schema.Tables)
			//	{
			//		if (table.Name != "__MigrationHistory")
			//		{
			//			var property = new Dictionary<string, Type>();
			//			foreach (var col in table.Columns)
			//			{
			//				property.Add(col.Name, Type.GetType(col.DataType.NetDataType));
			//				if (col.IsPrimaryKey)
			//				{
			//					//tableType.AddKeys(prop);
			//				}
			//			}
			//			var type = CreateType(dcf, table.Name, property);
			//			this.AddTable(type);

			//			var entityTypeConfiguration = entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] { });
			//			foreach (var pi in (type).GetProperties())
			//			{
			//				var column = table.Columns.Find(x => x.Name == pi.Name);

			//				if (column != null && column.IsPrimaryKey)
			//					modelBuilder.Entity(type).HasKey(pi.PropertyType, pi.Name);
			//				else
			//					modelBuilder.Entity(type).PrimitiveProperty(pi.PropertyType, pi.Name);
			//				//else if (pi.PropertyType == typeof(string))
			//				//	modelBuilder.Entity(table.Value).StringProperty(pi.Name);
			//			}
			//			var toTableMethod = entityTypeConfiguration.GetType().GetGenericMethod("ToTable");
			//			toTableMethod.Invoke(entityTypeConfiguration, new object[] { table.Name });
			//			var setMethod = this.GetType().GetGenericMethod("Set");
			//			var test1 = setMethod.MakeGenericMethod(type).Invoke(this, new object[] {});
			//			//modelBuilder.Entity<Type>().ToTable()
			//			//((EntityTypeConfiguration)returnType)..ToT(table.Key, "dbo");
			//			//Entry()

			//			var tets = Set(type);
			//		}
			//	}

			//	#endregion
			//}
		}

		//public void AddTable(Type type)
		//{
		//	_tables.Add(type.Name, type);
		//}

		private Type CreateType(DynamicClassFactory dcf, string name, Dictionary<string, Type> property)
		{
			var t = dcf.CreateDynamicType<DynamicEntity>(name, property);
			return t;
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

	public class TestTable
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}

	public class KEP_logger
	{
		public int ID { get; set; }
		public int Controller_ID { get; set; }
		public Single Weight_VALUE { get; set; }
		public DateTime Weight_TIMESTAMP { get; set; }
		public Boolean Weight_Status { get; set; }
	}
}
