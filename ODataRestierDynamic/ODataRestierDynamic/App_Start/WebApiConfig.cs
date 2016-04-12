using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Restier.WebApi;
using Microsoft.Restier.WebApi.Batch;
using ODataRestierDynamic.Models;
using System.Web.OData.Routing.Conventions;

namespace ODataRestierDynamic
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();
			config.EnableUnqualifiedNameCall(true);

			//System.Web.OData.MetadataController

			RegisterDynamic(config, GlobalConfiguration.DefaultServer);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}

		public static async void RegisterDynamic(HttpConfiguration config, HttpServer server)
		{
			System.Web.OData.Routing.ODataRoute odataRoute = await config.MapRestierRoute<DynamicApi>(
				"DynamicApi", "api/Dynamic",
				new RestierBatchHandler(server));

			//var attributeRoutingConvention = odataRoute.PathRouteConstraint.RoutingConventions.First(t => t is AttributeRoutingConvention);
			//var idx = odataRoute.PathRouteConstraint.RoutingConventions.IndexOf(attributeRoutingConvention);

			//var attributeRoutingConvention = odataRoute.PathRouteConstraint.RoutingConventions.First(t => t is AttributeRoutingConvention) as AttributeRoutingConvention;
			//var idx = odataRoute.PathRouteConstraint.RoutingConventions.IndexOf(attributeRoutingConvention);
			var dynamicActionRoutingConvention = new DynamicAttributeRoutingConvention();
			odataRoute.PathRouteConstraint.RoutingConventions.Add(dynamicActionRoutingConvention);

			//var attributeRoutingConvention = odataRoute.PathRouteConstraint.RoutingConventions.First(t => t is ActionRoutingConvention) as ActionRoutingConvention;
			//var idx = odataRoute.PathRouteConstraint.RoutingConventions.IndexOf(attributeRoutingConvention);
			//var dynamicActionRoutingConvention = new DynamicActionRoutingConvention() { InnerHandler = attributeRoutingConvention };
			//odataRoute.PathRouteConstraint.RoutingConventions.RemoveAt(idx);
			//odataRoute.PathRouteConstraint.RoutingConventions.Insert(idx, dynamicActionRoutingConvention);
		}
	}

	public class DynamicAttributeRoutingConvention : IODataRoutingConvention
	{
		public string SelectAction(System.Web.OData.Routing.ODataPath odataPath, System.Web.Http.Controllers.HttpControllerContext controllerContext, ILookup<string, System.Web.Http.Controllers.HttpActionDescriptor> actionMap)
		{
			throw new NotImplementedException();
		}

		public string SelectController(System.Web.OData.Routing.ODataPath odataPath, System.Net.Http.HttpRequestMessage request)
		{
			throw new NotImplementedException();
		}
	}

	public class DynamicActionRoutingConvention : ActionRoutingConvention
	{
		public ActionRoutingConvention InnerHandler { get; set; }

		public override string SelectController(System.Web.OData.Routing.ODataPath odataPath, System.Net.Http.HttpRequestMessage request)
		{
			return this.InnerHandler.SelectController(odataPath, request);
		}

		public override string SelectAction(System.Web.OData.Routing.ODataPath odataPath, System.Web.Http.Controllers.HttpControllerContext controllerContext, ILookup<string, System.Web.Http.Controllers.HttpActionDescriptor> actionMap)
		{
			var action = this.InnerHandler.SelectAction(odataPath, controllerContext, actionMap);

			return action;
		}
	}
}
