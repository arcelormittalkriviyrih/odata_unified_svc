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
using System.Net;
using System.Xml.Serialization;
using Newtonsoft.Json;
using ODataRestierDynamic.Actions;
using System.Data;

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

		/// <summary>	Method for getting object for user rules custom procedures. </summary>
		///
		/// <returns>	The user procedure. </returns>
		[HttpGet]
		[ODataRoute("GetUserProcedure")]
		public IHttpActionResult GetUserProcedure()
		{
			List<DynamicMetadataObject> result = new List<DynamicMetadataObject>();

			using (SqlConnection connection = new SqlConnection(DynamicContext.cConnectionStringSettings.ConnectionString))
			{
				string queryUserProcedures = @"
select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE, CREATED, LAST_ALTERED 
  from INFORMATION_SCHEMA.ROUTINES 
 where HAS_PERMS_BY_NAME(SPECIFIC_NAME,'OBJECT','EXECUTE') = 1
 order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME";
				SqlCommand commandUserProcedures = new SqlCommand(queryUserProcedures, connection);
				connection.Open();

				DataTable dataTable = new DataTable("UserProcedures");

				// create data adapter
				using (SqlDataAdapter dataAdapter = new SqlDataAdapter(commandUserProcedures))
				{
					// this will query your database and return the result to your datatable
					dataAdapter.Fill(dataTable);
					connection.Close();
				}

				foreach (DataRow row in dataTable.Rows)
				{
					result.Add(new DynamicMetadataObject()
					{
						Name = row["ROUTINE_NAME"].ToString(),
						ObjectType = row["ROUTINE_TYPE"].ToString() == "PROCEDURE" ? DBObjectType.Procedure : DBObjectType.Function,
						Schema = row["SPECIFIC_SCHEMA"].ToString()
					});
				}
			}

			return Ok(result);
		}

		/// <summary>	Call action. </summary>
		///
		/// <param name="name">					action name. </param>
		/// <param name="parameters">			Parameter names and values provided by a client in a POST
		/// 									request to invoke a particular Action. </param>
		/// <param name="cancellationToken">	The cancellation token. </param>
		///
		/// <returns>	A Task&lt;System.Net.Http.HttpResponseMessage&gt; </returns>
		public Task<System.Net.Http.HttpResponseMessage> CallAction(string name, ODataActionParameters parameters, System.Threading.CancellationToken cancellationToken)
		{
			if (!DbContext.DynamicActionMethods.ContainsKey(name))
				return NotFound().ExecuteAsync(cancellationToken);

			bool hasOutputParams = DbContext.DynamicActionMethods[name].Params.Where(p => p.isOut).Count() > 0;
			if (hasOutputParams)
				return CallActionOutput(name, parameters);
			else
				return CallActionResult(name, parameters).ExecuteAsync(cancellationToken);
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
		public IHttpActionResult CallActionResult(string name, ODataActionParameters parameters)
		{
			int result = -1;

			try
			{
				List<SqlParameter> paramList = new List<SqlParameter>();
				List<string> parameterNames = new List<string>();
				if (parameters != null)
				{
					foreach (var item in parameters)
					{
						string paramName = string.Format("{0}_param", item.Key.ToLower());
						paramList.Add(new SqlParameter(paramName, item.Value == null ? DBNull.Value : item.Value));
						parameterNames.Add(string.Format("@{0} = @{1}", item.Key, paramName));
					}
				}

				//EXECUTE {schema}[{functionName}]({string.Join(", ", parameterNames)})
				string commandText = string.Format(@"EXECUTE [{0}].[{1}] {2}", DynamicContext.cDefaultSchemaName, name, string.Join(", ", parameterNames));
				var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;
				result = objContext.ExecuteStoreCommand(commandText, paramList.ToArray());
			}
			catch (Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("CallAction", exception);
				throw exception;
			}

			return Ok(result);
		}

		/// <summary>	Call action output. </summary>
		///
		/// <exception cref="Exception">	Thrown when an exception error condition occurs. </exception>
		///
		/// <param name="name">		 	action name. </param>
		/// <param name="parameters">	Parameter names and values provided by a client in a POST request
		/// 							to invoke a particular Action. </param>
		///
		/// <returns>	A Task&lt;System.Net.Http.HttpResponseMessage&gt; </returns>
		public async Task<System.Net.Http.HttpResponseMessage> CallActionOutput(string name, ODataActionParameters parameters)
		{
			HttpResponseMessage response;

			try
			{
				ActionResult returnActionType = new ActionResult();
				returnActionType.Name = name;
				returnActionType.ActionParameters = new List<ActionParameter>();

				var actionMethod = DbContext.DynamicActionMethods[name];

				List<SqlParameter> paramList = new List<SqlParameter>();
				List<string> parameterNames = new List<string>();
				if (parameters != null)
				{
					foreach (var item in parameters)
					{
						string paramName = string.Format("{0}_param", item.Key.ToLower());
						var sqlParameter = new SqlParameter(paramName, item.Value == null ? DBNull.Value : item.Value);
						var paramInfo = actionMethod.Params.First(p => p.Name == ("@" + item.Key));
						if (paramInfo.isOut)
						{
							sqlParameter.Direction = paramInfo.isIn ? System.Data.ParameterDirection.InputOutput : System.Data.ParameterDirection.Output;
							returnActionType.ActionParameters.Add(new ActionParameter() { Name = item.Key, Value = item.Value, ParamName = paramName });
							paramName += " OUTPUT";
						}
						if (paramInfo.Length.HasValue)
						{
							sqlParameter.Size = paramInfo.Length.Value;
						}

						paramList.Add(sqlParameter);
						parameterNames.Add(string.Format("@{0} = @{1}", item.Key, paramName));
					}
				}

				//EXECUTE {schema}[{functionName}]({string.Join(", ", parameterNames)})
				string commandText = string.Format(@"EXECUTE [{0}].[{1}] {2}", DynamicContext.cDefaultSchemaName, name, string.Join(", ", parameterNames));
				var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;
				var paramArray = paramList.ToArray();
				returnActionType.Return = await objContext.ExecuteStoreCommandAsync(commandText, paramArray);

				foreach (var sqlParameter in paramArray)
				{
					if (sqlParameter.Direction == System.Data.ParameterDirection.InputOutput ||
						sqlParameter.Direction == System.Data.ParameterDirection.Output)
					{
						var parameter = returnActionType.ActionParameters.First(p => p.ParamName == sqlParameter.ParameterName);
						parameter.Value = sqlParameter.Value;
					}
				}

				string json = JsonConvert.SerializeObject(returnActionType);
				response = this.Request.CreateResponse(HttpStatusCode.OK);
				response.Content = new StringContent(json, Encoding.UTF8, "application/json");
			}
			catch (Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("CallActionOutput", exception);
				throw exception;
			}

			return response;
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