using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;

namespace ODataApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
			//config.MapHttpAttributeRoutes();

			//config.Routes.MapHttpRoute(
			//	name: "DefaultApi",
			//	routeTemplate: "api/{controller}/{id}",
			//	defaults: new { id = RouteParameter.Optional }
			//);

			config.AddODataQueryFilter();

			DynamicModelHelper.CustomMapODataServiceRoute(
                config.Routes,
                "odata",
                null);
        }
    }
}
