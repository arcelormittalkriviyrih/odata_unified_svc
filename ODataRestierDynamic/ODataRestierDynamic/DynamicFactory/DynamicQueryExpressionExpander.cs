using Microsoft.Restier.Core.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>
	/// Hook for query expression expand dynamically.
	/// </summary>
	public class DynamicQueryExpressionExpander : IQueryExpressionExpander, IDelegateHookHandler<IQueryExpressionExpander>
	{
		public IQueryExpressionExpander InnerHandler { get; set; }

		public System.Linq.Expressions.Expression Expand(QueryExpressionContext context)
		{
			//TODO: Expand query expression dynamically
			if (this.InnerHandler != null)
			{
				return this.InnerHandler.Expand(context);
			}
			else
				throw new NotImplementedException();
		}
	}
}