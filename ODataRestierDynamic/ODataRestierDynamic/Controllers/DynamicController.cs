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

namespace ODataRestierDynamic.Controllers
{
	public class DynamicController : ODataController
	{
		public DynamicController()
		{
			//Func<int, int> func = x => 2 * x;
			//this.Foo = func;
			//int i = obj.Foo(123); // now you see it
			//obj.Foo = null; // now you don't
		}

		private DynamicApi api;

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

		private DynamicContext DbContext
		{
			get
			{
				return Api.Context;
			}
		}

		//// Attribute routing to enable $count
		//[ODataRoute("Equipment/$count")]
		//public IHttpActionResult GetCount()
		//{
		//	return Ok(DbContext.Equipment.Count());
		//}

		//[HttpPost]
		//[ODataRoute("fn_diagramobjects")]
		//public IHttpActionResult fn_diagramobjects(ODataActionParameters parameters)
		//{
		//	return this.CallAction("fn_diagramobjects", parameters);
		//}

		[HttpPost]
		[ODataRoute("ins_MaterialLotByController")]
		public IHttpActionResult ins_MaterialLotByController(ODataActionParameters parameters)
		{
			return this.CallAction("ins_MaterialLotByController", parameters);
		}

		public IHttpActionResult CallAction(string name, ODataActionParameters parameters)
		{
			//int controllerID = (int)parameters["controllerID"];
			//int result = -1; //DbContext.ins_MaterialLotByController(controllerID);

			//result = DbContext.DynamicActions.ins_MaterialLotByController(1);

			//var controllerIDParameter = new ObjectParameter("ControllerID", 1);
			//var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;
			//result = objContext.ExecuteFunction("ins_MaterialLotByController", controllerIDParameter);

			int result = -1;

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
			try
			{
				result = objContext.ExecuteStoreCommand(commandText, paramList.ToArray());
				//var objectResult = objContext.ExecuteStoreQuery<int>(commandText.ToString(), paramList.ToArray());
			}
			catch
			{
				throw;
			}

			return Ok(result);
		}

		public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
		{
			return base.ExecuteAsync(controllerContext, cancellationToken);
		} 

		/// <summary>
		/// Disposes the API and the controller.
		/// </summary>
		/// <param name="disposing">Indicates whether disposing is happening.</param>
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