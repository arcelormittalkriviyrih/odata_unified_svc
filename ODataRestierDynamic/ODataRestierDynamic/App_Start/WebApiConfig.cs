using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Restier.WebApi;
using Microsoft.Restier.WebApi.Batch;
using ODataRestierDynamic.Models;
using System.Web.OData.Routing.Conventions;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Formatter.Deserialization;
using System.Web.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Core;
using System.IO;
using System.Net.Http.Headers;
using System.Globalization;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Net.Http;
using ODataRestierDynamic.DynamicFactory;

namespace ODataRestierDynamic
{
	/// <summary>	A web API configuration. </summary>
	public static class WebApiConfig
	{
		/// <summary>	Registers the specified configuration. </summary>
		///
		/// <param name="config">	The configuration. </param>
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();
			config.EnableUnqualifiedNameCall(true);

			RegisterDynamic(config, GlobalConfiguration.DefaultServer);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}

		/// <summary>	Registers the dynamic. </summary>
		///
		/// <param name="config">	The configuration. </param>
		/// <param name="server">	The server. </param>
		public static async void RegisterDynamic(HttpConfiguration config, HttpServer server)
		{
			System.Web.OData.Routing.ODataRoute odataRoute = await config.MapRestierRoute<DynamicApi>(
				"DynamicApi", "api/Dynamic",
				new RestierBatchHandler(server));

			//for overriding standart metadata class
			//System.Web.OData.MetadataController
			odataRoute.PathRouteConstraint.RoutingConventions.Add(new MediadataRoutingConvention());

			// Register an Action selector that can include template parameters in the name
			IHttpActionSelector actionSelectorService = config.Services.GetActionSelector();
			config.Services.Replace(typeof(IHttpActionSelector), new DynamicODataActionSelector(actionSelectorService));
			// Register an Action invoker that can include template parameters in the name
			IHttpActionInvoker actionInvokerService = config.Services.GetActionInvoker();
			config.Services.Replace(typeof(IHttpActionInvoker), new DynamicODataActionInvoker(actionInvokerService));
		}
	}
}
