using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ODataRestierDynamic
{
    /// <summary>	A web API application. </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>	Application start. </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
