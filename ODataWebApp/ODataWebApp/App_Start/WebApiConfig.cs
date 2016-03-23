using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Restier.WebApi;
using Microsoft.Restier.WebApi.Batch;
using ODataWebApp.Models;
using System.Web.Configuration;

namespace ODataWebApp
{
    public static class WebApiConfig
    {
		//public static void Register(HttpConfiguration config)
		//{
		//	// Web API configuration and services

		//	// Web API routes
		//	config.MapHttpAttributeRoutes();

		//	config.Routes.MapHttpRoute(
		//		name: "DefaultApi",
		//		routeTemplate: "api/{controller}/{id}",
		//		defaults: new { id = RouteParameter.Optional }
		//	);
		//}

		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.EnableUnqualifiedNameCall(true);

			bool useDynamicApi = bool.Parse(WebConfigurationManager.AppSettings["UseDynamicApi"]);

			if (useDynamicApi)
				RegisterDynamic(config, GlobalConfiguration.DefaultServer);
			else
				RegisterStatic(config, GlobalConfiguration.DefaultServer);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}

		public static async void RegisterDynamic(HttpConfiguration config, HttpServer server)
		{
			await config.MapRestierRoute<DynamicApi>(
				"DynamicApi", "api/Dynamic",
				new RestierBatchHandler(server));
		}

		public static async void RegisterStatic(HttpConfiguration config, HttpServer server)
		{
			await config.MapRestierRoute<StaticApi>(
				"StaticApi", "api/Static",
				new RestierBatchHandler(server));
		}
    }
}
