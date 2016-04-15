using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Restier.WebApi;
using ODataRestierDynamic.Models;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Threading;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Text;
using DatabaseSchemaReader;
using ODataRestierDynamic.DynamicFactory;
using ODataRestierDynamic.Log;

namespace ODataRestierDynamic.Controllers
{
	/// <summary>
	/// Dynamic OData controllers that support writing and reading data using the OData formats.
	/// </summary>
	public class DynamicController : ODataController
	{
		/// <summary>	The API. </summary>
		private DynamicApi api;

		/// <summary>	Gets the API. </summary>
		///
		/// <value>	The API. </value>
		private DynamicApi Api
		{
			get
			{
				if (api == null)
				{
					api = new DynamicApi();
				}

				return api;
			}
		}

		/// <summary>	Gets a context for the database. </summary>
		///
		/// <value>	The database context. </value>
		private DynamicContext DbContext
		{
			get
			{
				return Api.Context;
			}
		}

		/// <summary>	Method for getting object for user rules custom metadata. </summary>
		///
		/// <returns>	The user metadata. </returns>
		[HttpGet]
		[ODataRoute("GetUserMetadata")]
		public IHttpActionResult GetUserMetadata()
		{
			List<DynamicMetadataObject> result = new List<DynamicMetadataObject>();

			// https://www.nuget.org/packages/DatabaseSchemaReader/
			using (var dbReader = new DatabaseReader(DynamicContext.cConnectionStringSettings.ConnectionString, DynamicContext.cConnectionStringSettings.ProviderName, DynamicContext.cDefaultSchemaName))
			{
				var schema = dbReader.ReadAll();

				#region Read Tables

				foreach (var table in schema.Tables)
				{
					result.Add(new DynamicMetadataObject() { Name = table.Name, ObjectType = DBObjectType.Table, Schema = table.SchemaOwner });
				}

				#endregion

				#region Read Views

				foreach (var view in schema.Views)
				{
					result.Add(new DynamicMetadataObject() { Name = view.Name, ObjectType = DBObjectType.View, Schema = view.SchemaOwner });
				}

				#endregion

				#region Read Actions

				foreach (var function in schema.Functions)
				{
					result.Add(new DynamicMetadataObject() { Name = function.Name, ObjectType = DBObjectType.Function, Schema = function.SchemaOwner });
				}

				foreach (var procedure in schema.StoredProcedures)
				{
					result.Add(new DynamicMetadataObject() { Name = procedure.Name, ObjectType = DBObjectType.Procedure, Schema = procedure.SchemaOwner });
				}

				#endregion
			}

			return Ok(result);
		}

		/// <summary>	Method for calling all Dynamic Actions which exist in DbContext. </summary>
		///
		/// <param name="name">		 	action name. </param>
		/// <param name="parameters">	Parameter names and values provided by a client in a POST request
		/// 							to invoke a particular Action. </param>
		///
		/// <returns>
		/// Command that asynchronously creates an System.Net.Http.HttpResponseMessage.
		/// </returns>
		public IHttpActionResult CallAction(string name, ODataActionParameters parameters)
		{
			//int controllerID = (int)parameters["controllerID"];
			//int result = -1; //DbContext.ins_MaterialLotByController(controllerID);

			//result = DbContext.DynamicActions.ins_MaterialLotByController(1);

			//var controllerIDParameter = new ObjectParameter("ControllerID", 1);
			//var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;
			//result = objContext.ExecuteFunction("ins_MaterialLotByController", controllerIDParameter);

			int result = -1;

			try
			{
				List<SqlParameter> paramList = new List<SqlParameter>();
				List<string> parameterNames = new List<string>();
				if (parameters != null)
				{
					foreach (var item in parameters)
					{
						paramList.Add(new SqlParameter(item.Key, item.Value));
						parameterNames.Add("@" + item.Key);
					}
				}

				//EXECUTE {schema}[{functionName}]({string.Join(", ", parameterNames)})
				string commandText = string.Format(@"EXECUTE [{0}].[{1}] {2}", DynamicContext.cDefaultSchemaName, name, string.Join(", ", parameterNames));

				var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;

				result = objContext.ExecuteStoreCommand(commandText, paramList.ToArray());
			}
			catch(Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("ReadParams", exception);
				throw exception;
			}

			return Ok(result);
		}

		/// <summary>	Disposes the API and the controller. </summary>
		///
		/// <param name="disposing">	Indicates whether disposing is happening. </param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.api != null)
				{
					this.api.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}