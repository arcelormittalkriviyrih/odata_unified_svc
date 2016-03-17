using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Restier.WebApi;
using ODataWebApp.Models;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Threading.Tasks;
using System.Web.Http;

namespace ODataWebApp.Controllers
{
	public class DynamicController : ODataController
	{
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