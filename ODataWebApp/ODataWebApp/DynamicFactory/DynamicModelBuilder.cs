using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ODataWebApp.DynamicFactory
{
	/// <summary>
	/// Hook for building DB model dynamically.
	/// </summary>
	public class DynamicModelBuilder : IModelBuilder, IDelegateHookHandler<IModelBuilder>
	{
		public IModelBuilder InnerHandler { get; set; }

		public async Task<IEdmModel> GetModelAsync(InvocationContext context, CancellationToken cancellationToken)
		{
			//TODO: Create DB model dynamically
			EdmModel model = null;
			if (this.InnerHandler != null)
			{
				model = await this.InnerHandler.GetModelAsync(context, cancellationToken) as EdmModel;
			}

			if (model == null)
			{
				// We don't plan to extend an empty model with operations.
				return null;
			}

			// some model extender
			return model;
		}
	}
}