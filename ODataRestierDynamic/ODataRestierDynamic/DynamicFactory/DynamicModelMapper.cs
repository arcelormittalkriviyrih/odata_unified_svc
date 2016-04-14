using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>	Hook for building DB model mapping dynamically. </summary>
	public class DynamicModelMapper : IModelMapper, IDelegateHookHandler<IModelMapper>
	{
		/// <summary>	Gets or sets the inner handler. </summary>
		///
		/// <value>	The inner handler. </value>
		public IModelMapper InnerHandler { get; set; }

		/// <summary>	Tries to get the relevant type of a composable function. </summary>
		///
		/// <param name="context">			An API context. </param>
		/// <param name="namespaceName">	The name of a namespace containing a composable function. </param>
		/// <param name="name">				The name of composable function. </param>
		/// <param name="relevantType"> 	[out] When this method returns, provides the relevant type of
		/// 								the composable function. </param>
		///
		/// <returns>	<c>true</c> if the relevant type was provided; otherwise, <c>false</c>. </returns>
		public bool TryGetRelevantType(ApiContext context, string namespaceName, string name, out Type relevantType)
		{
			//Dynamic DB model mapping with check type existence
			if (this.InnerHandler != null)
			{
				bool result = false;

				if (string.IsNullOrEmpty(namespaceName))
					result = this.InnerHandler.TryGetRelevantType(context, name, out relevantType);
				else
					result =  this.InnerHandler.TryGetRelevantType(context, namespaceName, name, out relevantType);

				if (!result)
				{
					var dbContext = context.GetProperty<DynamicContext>(DynamicContext.cDbContextKey);
					if (dbContext != null)
					{
						result = dbContext.TryGetRelevantType(name, out relevantType);
					}
				}

				return result;
			}
			else
				throw new NotImplementedException();
		}

		/// <summary>
		/// Tries to get the relevant type of an entity set, singleton, or composable function import.
		/// </summary>
		///
		/// <param name="context">	   	An API context. </param>
		/// <param name="name">		   	The name of an entity set, singleton or composable function
		/// 							import. </param>
		/// <param name="relevantType">	[out] When this method returns, provides the relevant type of the
		/// 							queryable source. </param>
		///
		/// <returns>	<c>true</c> if the relevant type was provided; otherwise, <c>false</c>. </returns>
		public bool TryGetRelevantType(ApiContext context, string name, out Type relevantType)
		{
			return this.TryGetRelevantType(context, null, name, out relevantType);
		}
	}
}