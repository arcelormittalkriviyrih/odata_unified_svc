using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>
	/// Hook for building DB model mapping dynamically.
	/// </summary>
	public class DynamicModelMapper : IModelMapper, IDelegateHookHandler<IModelMapper>
	{
		public IModelMapper InnerHandler { get; set; }

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

		public bool TryGetRelevantType(ApiContext context, string name, out Type relevantType)
		{
			return this.TryGetRelevantType(context, null, name, out relevantType);
		}
	}
}