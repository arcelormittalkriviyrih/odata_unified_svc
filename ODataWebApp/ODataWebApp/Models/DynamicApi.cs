using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Microsoft.OData.Edm;
using Microsoft.Restier.EntityFramework;
using Microsoft.Restier.Core;
using ODataWebApp.DynamicFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Microsoft.Restier.Core.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OData.Edm.Library;
using Microsoft.Restier.Core.Query;
using System.Linq.Expressions;

namespace ODataWebApp.Models
{
	/// <summary>
	/// Class API for dynamically odata communication with DB.
	/// </summary>
	public class DynamicApi : DbApi<DynamicContext>
	{
		#region Fields

		private DynamicModelBuilder _dynamicModelBuilder = null;
		private DynamicModelMapper _dynamicModelMapper = null;

		private DynamicQueryExpressionExpander _dynamicQueryExpressionExpander = null;
		private DynamicQueryExpressionSourcer _dynamicQueryExpressionSourcer = null;

		#endregion

		#region Properties

		public DynamicContext Context
		{
			get
			{
				return DbContext;
			}
		} 

		#endregion

		#region Methods

		protected override ApiConfiguration CreateApiConfiguration()
		{
			var apiConfiguration = base.CreateApiConfiguration();

			try
			{
				if (_dynamicModelBuilder == null)
				{
					_dynamicModelBuilder = new DynamicModelBuilder();
					_dynamicModelBuilder.InnerHandler = apiConfiguration.GetHookHandler<IModelBuilder>();
					apiConfiguration.AddHookHandler<IModelBuilder>(_dynamicModelBuilder);
				}

				if (_dynamicModelMapper == null)
				{
					_dynamicModelMapper = new DynamicModelMapper();
					_dynamicModelMapper.InnerHandler = apiConfiguration.GetHookHandler<IModelMapper>();
					apiConfiguration.AddHookHandler<IModelMapper>(_dynamicModelMapper);
				}

				if (_dynamicQueryExpressionExpander == null)
				{
					_dynamicQueryExpressionExpander = new DynamicQueryExpressionExpander();
					_dynamicQueryExpressionExpander.InnerHandler = apiConfiguration.GetHookHandler<IQueryExpressionExpander>();
					apiConfiguration.AddHookHandler<IQueryExpressionExpander>(_dynamicQueryExpressionExpander);
				}

				if (_dynamicQueryExpressionSourcer == null)
				{
					_dynamicQueryExpressionSourcer = new DynamicQueryExpressionSourcer();
					_dynamicQueryExpressionSourcer.InnerHandler = apiConfiguration.GetHookHandler<IQueryExpressionSourcer>();
					apiConfiguration.AddHookHandler<IQueryExpressionSourcer>(_dynamicQueryExpressionSourcer);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return apiConfiguration;
		}

		//protected override DynamicContext CreateDbContext()
		//{
		//	var context = base.CreateDbContext();

		//	System.Data.Entity.DbSet set = context.Set(typeof(KEP_logger));

		//	return context;
		//}

		//protected override DynamicContext CreateDbContext()
		//{
		//	DynamicContext context = null;//base.CreateDbContext();

		//	System.Configuration.ConnectionStringSettings connectionStringSettings = WebConfigurationManager.ConnectionStrings["KEPServer"];

		//	if (context == null)
		//	{
		//		context = new DynamicContext();//string.Format("name={0}", "KEPServer"/*"B2MML-BatchML"*/));

		//		// https://www.nuget.org/packages/DatabaseSchemaReader/
		//		using (var dbReader = new DatabaseReader(connectionStringSettings.ConnectionString, "System.Data.SqlClient", "dbo"))
		//		{
		//			var schema = dbReader.ReadAll();
		//			var dcf = new DynamicClassFactory();

		//			#region Read Tables

		//			foreach (var table in schema.Tables)
		//			{
		//				if (table.Name != "__MigrationHistory")
		//				{
		//					var property = new Dictionary<string, Type>();
		//					foreach (var col in table.Columns)
		//					{
		//						property.Add(col.Name, Type.GetType(col.DataType.NetDataType));
		//						if (col.IsPrimaryKey)
		//						{
		//							//tableType.AddKeys(prop);
		//						}
		//					}
		//					var type = CreateType(dcf, table.Name, property);
		//					context.AddTable(type);

		//					var tets = context.Set(type);
		//				}
		//			}

		//			#endregion
		//		}
		//	}

		//	return context;
		//}

		//private Type CreateType(DynamicClassFactory dcf, string name, Dictionary<string, Type> property)
		//{
		//	var t = dcf.CreateDynamicType<DynamicEntity>(name, property);
		//	return t;
		//} 

		#endregion
	}
}