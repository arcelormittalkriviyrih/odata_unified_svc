using Microsoft.Restier.Core.Query;
using System;

namespace ODataRestierDynamic.DynamicFactory
{
    /// <summary>	Hook for query expression expand dynamically. </summary>
    public class DynamicQueryExpressionExpander : IQueryExpressionExpander, IDelegateHookHandler<IQueryExpressionExpander>
	{
		/// <summary>	Gets or sets the inner handler. </summary>
		///
		/// <value>	The inner handler. </value>
		public IQueryExpressionExpander InnerHandler { get; set; }

		/// <summary>	Expands an expression. </summary>
		///
		/// <param name="context">	The query expression context. </param>
		///
		/// <returns>
		/// An expanded expression of the same type as the visited node, or if expansion did not apply,
		/// the visited node or <c>null</c>.
		/// </returns>
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