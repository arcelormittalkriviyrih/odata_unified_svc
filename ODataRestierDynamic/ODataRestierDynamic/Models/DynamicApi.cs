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
	/// <summary>	Class API for dynamically odata communication with DB. </summary>
	public class DynamicApi : DbApi<DynamicContext>
	{
		#region Fields

		/// <summary>	The dynamic model builder. </summary>
		private DynamicModelBuilder _dynamicModelBuilder = null;
		/// <summary>	The dynamic model mapper. </summary>
		private DynamicModelMapper _dynamicModelMapper = null;

		/// <summary>	The dynamic query expression expander. </summary>
		private DynamicQueryExpressionExpander _dynamicQueryExpressionExpander = null;
		/// <summary>	The dynamic query expression sourcer. </summary>
		private DynamicQueryExpressionSourcer _dynamicQueryExpressionSourcer = null;

		//private static DynamicContext _dynamicContext = null;

		/// <summary>	List of names of the test tables. </summary>
		private static string[] cTestTableNames = new string[] { "TestDataTypesOffset", "PropertyTypes", "KEP_logger", "Equipment" };

		#endregion

		#region Properties

		/// <summary>	Gets the context. </summary>
		///
		/// <value>	The context. </value>
		public DynamicContext Context
		{
			get
			{
				return DbContext;
			}
		}

		#endregion

		#region Methods

		/// <summary>	Creates API configuration. </summary>
		///
		/// <returns>	The new API configuration. </returns>
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

		/// <summary>	Gets user metadata. </summary>
		///
		/// <returns>	An array of dynamic metadata object. </returns>
		[Function]
		public DynamicMetadataObject[] GetUserMetadata()
		{
			return new DynamicMetadataObject[]{};
		}

		/// <summary>
		/// Releases the unmanaged resources used by the ODataRestierDynamic.Models.DynamicApi and
		/// optionally releases the managed resources.
		/// </summary>
		///
		/// <param name="disposing">	true to release both managed and unmanaged resources; false to
		/// 							release only unmanaged resources. </param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		#endregion
	}
}