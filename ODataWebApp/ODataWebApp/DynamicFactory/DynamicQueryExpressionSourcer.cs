using Microsoft.Restier.Core.Query;
using ODataWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ODataWebApp.DynamicFactory
{
	/// <summary>
	/// Hook for query expression source dynamically.
	/// </summary>
	public class DynamicQueryExpressionSourcer : IQueryExpressionSourcer, IDelegateHookHandler<IQueryExpressionSourcer>
	{
		public IQueryExpressionSourcer InnerHandler { get; set; }

		public System.Linq.Expressions.Expression Source(QueryExpressionContext context, bool embedded)
		{
			//TODO: Source query expression dynamically
			if (this.InnerHandler != null)
			{
				try
				{
					var dbContext = context.QueryContext.ApiContext.GetProperty<DynamicContext>("Microsoft.Restier.EntityFramework.DbContext");
					Type type = typeof(TestTable);
					var dbSet = dbContext.Set(type);
					//context.ModelReference.EntitySet
					//context.ModelReference.EntityType

					return Expression.Constant(dbSet);
					//return this.InnerHandler.Source(context, embedded);
				}
				catch
				{
					throw;
				}
			}
			else
				throw new NotImplementedException();
		}
	}
}