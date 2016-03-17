using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ODataApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

			//for json : http://localhost:47503/api/?type=json
			GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
				new System.Net.Http.Formatting.QueryStringMapping("type", "json", new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")));
			//for xml: http://localhost:47503/api/?type=xml
			GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
				new System.Net.Http.Formatting.QueryStringMapping("type", "xml", new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml")));
        }
    }
}
