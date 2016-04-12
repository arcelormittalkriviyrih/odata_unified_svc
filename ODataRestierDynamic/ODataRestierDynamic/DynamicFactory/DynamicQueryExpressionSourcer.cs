using Microsoft.Restier.Core.Query;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>
	/// Hook for query expression source dynamically.
	/// </summary>
	public class DynamicQueryExpressionSourcer : IQueryExpressionSourcer, IDelegateHookHandler<IQueryExpressionSourcer>
	{
		public IQueryExpressionSourcer InnerHandler { get; set; }

		public System.Linq.Expressions.Expression Source(QueryExpressionContext context, bool embedded)
		{
			//Source query expression dynamically
			var dbContext = context.QueryContext.ApiContext.GetProperty<DynamicContext>(DynamicContext.cDbContextKey);
			if (dbContext != null)
			{
				string name = context.ModelReference.EntitySet.Name;
				Type type = dbContext.GetModelType(name);
				var dbSet = dbContext.Set(type);

				return Expression.Constant(dbSet);
			}

			if (this.InnerHandler != null)
				return this.InnerHandler.Source(context, embedded);
			else
				throw new NotImplementedException();
		}
	}
}