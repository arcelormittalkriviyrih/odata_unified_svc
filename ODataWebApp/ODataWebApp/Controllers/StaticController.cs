using Microsoft.OData.Core;
using ODataWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace ODataWebApp.Controllers
{
	public class StaticController : ODataController
	{
		private StaticApi api;

		private StaticApi Api
		{
			get
			{
				if (api == null)
				{
					api = new StaticApi();
				}

				return api;
			}
		}

		private Entities DbContext
		{
			get
			{
				return Api.Context;
			}
		}

		//// Attribute routing to enable $count
		//[ODataRoute("Equipment/$count")]
		//public IHttpActionResult GetEquipmentCount()
		//{
		//	return Ok(DbContext.Equipment.Count());
		//}

		[HttpPost]
		//[ODataRoute("ins_MaterialLotByController()")]
		public IHttpActionResult ins_MaterialLotByController(ODataActionParameters parameters)
		{
			int controllerID = (int)parameters["controllerID"];
			int result = DbContext.ins_MaterialLotByController(controllerID);

			return Ok(result);
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