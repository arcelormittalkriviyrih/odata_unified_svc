using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>
	/// Hook for building DB model dynamically.
	/// </summary>
	public class DynamicModelBuilder : IModelBuilder, IDelegateHookHandler<IModelBuilder>
	{
		public IModelBuilder InnerHandler { get; set; }

		public async Task<IEdmModel> GetModelAsync(InvocationContext context, System.Threading.CancellationToken cancellationToken)
		{
			//Create EDM DB model dynamically if you need
			EdmModel model = null;
			if (this.InnerHandler != null)
			{
				model = await this.InnerHandler.GetModelAsync(context, cancellationToken) as EdmModel;
			}

			//extend model here

			return model;
		}
	}
}