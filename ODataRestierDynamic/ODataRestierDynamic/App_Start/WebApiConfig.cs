using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Restier.WebApi;
using Microsoft.Restier.WebApi.Batch;
using ODataRestierDynamic.Models;

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

			RegisterDynamic(config, GlobalConfiguration.DefaultServer);

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
    }
}
