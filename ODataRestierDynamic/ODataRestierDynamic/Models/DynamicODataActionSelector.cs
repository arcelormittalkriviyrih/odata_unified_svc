using System;
using System.Linq;
using System.Web.Http.Controllers;

namespace ODataRestierDynamic.Models
{
    /// <summary>
    /// Contains the logic for selecting an action method.
    /// </summary>
    public class DynamicODataActionSelector : IHttpActionSelector
	{
		/// <summary>
		/// The inner controller selector to call
		/// </summary>
		private readonly IHttpActionSelector _innerSelector;

		/// <summary>
		/// Initializes a new instance of the DynamicODataActionSelector class.
		/// </summary>
		/// <param name="innerSelector">The inner controller selector to call.</param>
		public DynamicODataActionSelector(IHttpActionSelector innerSelector)
		{
			_innerSelector = innerSelector;
		}

		/// <summary>
		/// Returns a map, keyed by action string, of all System.Web.Http.Controllers.HttpActionDescriptor
		/// that the selector can select. This is primarily called by System.Web.Http.Description.IApiExplorer
		/// to discover all the possible actions in the controller.
		/// </summary>
		/// <param name="controllerDescriptor">The controller descriptor.</param>
		/// <returns>
		/// A map of System.Web.Http.Controllers.HttpActionDescriptor that the selector
		/// can select, or null if the selector does not have a well-defined mapping
		/// of System.Web.Http.Controllers.HttpActionDescriptor.
		/// </returns>
		public ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
		{
			if (_innerSelector != null)
			{
				var result = _innerSelector.GetActionMapping(controllerDescriptor);
				return result;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		/// Selects the action for the controller.
		/// </summary>
		/// <param name="controllerContext">The context of the controller.</param>
		/// <returns>The action for the controller.</returns>
		public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
		{
			if (_innerSelector != null)
			{
				var result = _innerSelector.SelectAction(controllerContext);
				return result;
			}
			throw new NotImplementedException();
		}
	}
}