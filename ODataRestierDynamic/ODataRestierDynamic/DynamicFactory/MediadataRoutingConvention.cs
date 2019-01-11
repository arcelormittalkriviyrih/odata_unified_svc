using Microsoft.Restier.WebApi;
using ODataRestierDynamic.Models;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.OData.Routing.Conventions;
using System.Web.OData.Routing;

namespace ODataRestierDynamic.DynamicFactory
{
    /// <summary>	A media data routing convention. </summary>
    internal class MediadataRoutingConvention : IODataRoutingConvention
	{
		/// <summary>	Default constructor. </summary>
		public MediadataRoutingConvention()
		{
		}

		/// <summary>
		/// Selects OData controller based on parsed OData URI
		/// </summary>
		/// <param name="odataPath">Parsed OData URI</param>
		/// <param name="request">Incoming HttpRequest</param>
		/// <returns>Prefix for controller name</returns>
		public string SelectController(ODataPath odataPath, HttpRequestMessage request)
		{
			string result = null;
			if (IsMediadataPath(odataPath))
			{
				result = "Restier";
			}

			return result;
		}

		/// <summary>
		/// Selects the appropriate action based on the parsed OData URI.
		/// </summary>
		/// <param name="odataPath">Parsed OData URI</param>
		/// <param name="controllerContext">Context for HttpController</param>
		/// <param name="actionMap">Mapping from action names to HttpActions</param>
		/// <returns>String corresponding to controller action name</returns>
		public string SelectAction(
			ODataPath odataPath,
			HttpControllerContext controllerContext,
			ILookup<string, HttpActionDescriptor> actionMap)
		{
			string result = null;

			// RESTier cannot select action on controller which is not RestierController.
			if ((controllerContext.Controller is RestierController))
			{
				if (IsMediadataPath(odataPath))
				{
					result = DynamicODataActionInvoker.cPostActionName;
				}
			}

			// Let WebAPI select default action
			return result;
		}

		/// <summary>	Query if 'odataPath' is mediadata path. </summary>
		///
		/// <param name="odataPath">	Parsed OData URI. </param>
		///
		/// <returns>	true if mediadata path, false if not. </returns>
		private static bool IsMediadataPath(ODataPath odataPath)
		{
			return odataPath.PathTemplate == "~/entityset/key/$value";
		}
	}
}