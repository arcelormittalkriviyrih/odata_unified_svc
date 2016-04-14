using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Microsoft.OData.Edm;
using Microsoft.Restier.EntityFramework;
using Microsoft.Restier.Core;
using ODataRestierDynamic.DynamicFactory;
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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using MagicDbModelBuilder;

namespace ODataRestierDynamic.Models
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

		//private static DynamicContext _dynamicContext = null;

		private static string[] cTestTableNames = new string[] { "TestDataTypesOffset", "PropertyTypes", "KEP_logger", "Equipment" };

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

			return apiConfiguration;
		}

		[Function]
		public DynamicMetadataObject[] GetUserMetadata()
		{
			return new DynamicMetadataObject[]{};
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		#endregion
	}
}