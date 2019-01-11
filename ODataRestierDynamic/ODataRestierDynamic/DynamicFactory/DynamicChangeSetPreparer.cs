using Microsoft.OData.Edm.Library;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Query;
using Microsoft.Restier.Core.Submit;
using ODataRestierDynamic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ODataRestierDynamic.DynamicFactory
{
    /// <summary>	Hook for change set preparer./ </summary>
    public class DynamicChangeSetPreparer : IChangeSetPreparer, IDelegateHookHandler<IChangeSetPreparer>
	{
		/// <summary>	Gets or sets the inner handler. </summary>
		///
		/// <value>	The inner handler. </value>
		public IChangeSetPreparer InnerHandler { get; set; }

		/// <summary>	Prepare asynchronous. </summary>
		///
		/// <param name="context">				The submit context. </param>
		/// <param name="cancellationToken">	A cancellation token. </param>
		///
		/// <returns>	A System.Threading.Tasks.Task. </returns>
		public async Task PrepareAsync(SubmitContext context, CancellationToken cancellationToken)
		{
			//Source query expression dynamically
			var dbContext = context.ApiContext.GetProperty<DynamicContext>(DynamicContext.cDbContextKey);
			if (dbContext != null)
			{
				foreach (var entry in context.ChangeSet.Entries.OfType<DataModificationEntry>())
				{
					Type entityType = dbContext.GetModelType(entry.EntitySetName);
					System.Data.Entity.DbSet set = dbContext.Set(entityType);

					object entity = null;

					if (entry.IsNew)
					{
						entity = set.Create();

						SetValues(entity, entityType, entry.LocalValues);

						set.Add(entity);
					}
					else if (entry.IsDelete)
					{
						entity = await FindEntity(context, entry, cancellationToken);
						set.Remove(entity);
					}
					else if (entry.IsUpdate)
					{
						entity = await FindEntity(context, entry, cancellationToken);

						DbEntityEntry dbEntry = dbContext.Entry(entity);
						SetValues(dbEntry, entry, entityType);
					}
					else
					{
						throw new NotSupportedException();
					}

					entry.Entity = entity;
				}
			}
			else if (this.InnerHandler != null)
			{
				await this.InnerHandler.PrepareAsync(context, cancellationToken);
			}
			else
				throw new NotImplementedException();
		}

		/// <summary>	Searches for the first entity. </summary>
		///
		/// <exception cref="InvalidOperationException">	Thrown when the requested operation is
		/// 												invalid. </exception>
		///
		/// <param name="context">				The submit context. </param>
		/// <param name="entry">				The entry. </param>
		/// <param name="cancellationToken">	A cancellation token. </param>
		///
		/// <returns>	The found entity. </returns>
		private static async Task<object> FindEntity(SubmitContext context, DataModificationEntry entry, CancellationToken cancellationToken)
		{
			IQueryable query = Api.Source(context.ApiContext, entry.EntitySetName);
			query = entry.ApplyTo(query);

			QueryResult result = await Api.QueryAsync(
				context.ApiContext,
				new QueryRequest(query),
				cancellationToken);

			object entity = result.Results.SingleOrDefault();
			if (entity == null)
			{
				// TODO GitHubIssue#38 : Handle the case when entity is resolved
				// there are 2 cases where the entity is not found:
				// 1) it doesn't exist
				// 2) concurrency checks have failed
				// we should account for both - I can see 3 options:
				// a. always return "PreConditionFailed" result
				//  - this is the canonical behavior of WebAPI OData, see the following post:
				//    "Getting started with ASP.NET Web API 2.2 for OData v4.0" on http://blogs.msdn.com/b/webdev/.
				//  - this makes sense because if someone deleted the record, then you still have a concurrency error
				// b. possibly doing a 2nd query with just the keys to see if the record still exists
				// c. only query with the keys, and then set the DbEntityEntry's OriginalValues to the ETag values,
				//    letting the save fail if there are concurrency errors

				////throw new EntityNotFoundException
				throw new InvalidOperationException("Could not find the specified resource.");
			}

			return entity;
		}

		/// <summary>	Sets the values. </summary>
		///
		/// <exception cref="NotSupportedException">	Thrown when the requested operation is not
		/// 											supported. </exception>
		///
		/// <param name="dbEntry">   	The database entry. </param>
		/// <param name="entry">	 	The entry. </param>
		/// <param name="entityType">	Type of the entity. </param>
		private static void SetValues(DbEntityEntry dbEntry, DataModificationEntry entry, Type entityType)
		{
			if (entry.IsFullReplaceUpdate)
			{
				// The algorithm for a "FullReplaceUpdate" is taken from ObjectContextServiceProvider.ResetResource
				// in WCF DS, and works as follows:
				//  - Create a new, blank instance of the entity.
				//  - Copy over the key values and set any updated values from the client on the new instance.
				//  - Then apply all the properties of the new instance to the instance to be updated.
				//    This will set any unspecified properties to their default value.
				object newInstance = Activator.CreateInstance(entityType);

				SetValues(newInstance, entityType, entry.EntityKey);
				SetValues(newInstance, entityType, entry.LocalValues);

				dbEntry.CurrentValues.SetValues(newInstance);
			}
			else
			{
				foreach (KeyValuePair<string, object> propertyPair in entry.LocalValues)
				{
					DbPropertyEntry propertyEntry = dbEntry.Property(propertyPair.Key);
					object value = propertyPair.Value;
					if (value == null)
					{
						// If the property value is null, we set null in the entry too.
						propertyEntry.CurrentValue = null;
						continue;
					}

                    Type type = typeof(string);
                    if (propertyEntry.EntityEntry != null && propertyEntry.EntityEntry.Entity != null)
                    {
                        type = propertyEntry.EntityEntry.Entity.GetType().GetProperty(propertyPair.Key).PropertyType;
                    }
                    else if (propertyEntry.CurrentValue != null)
                    {
                        type = propertyEntry.CurrentValue.GetType();
                    }

					if (propertyEntry is DbComplexPropertyEntry)
					{
						var dic = value as IReadOnlyDictionary<string, object>;
						if (dic == null)
						{
							throw new NotSupportedException(string.Format("Unsupported type for property: {0}.", propertyPair.Key));
						}

						value = Activator.CreateInstance(type);
						SetValues(value, type, dic);
					}

					propertyEntry.CurrentValue = ConvertToEfValue(type, value);
				}
			}
		}

		/// <summary>	Sets the values. </summary>
		///
		/// <exception cref="NotSupportedException">	Thrown when the requested operation is not
		/// 											supported. </exception>
		///
		/// <param name="instance">	The instance. </param>
		/// <param name="type">	   	The type. </param>
		/// <param name="values">  	The values. </param>
		private static void SetValues(object instance, Type type, IReadOnlyDictionary<string, object> values)
		{
			foreach (KeyValuePair<string, object> propertyPair in values)
			{
				object value = propertyPair.Value;
				PropertyInfo propertyInfo = type.GetProperty(propertyPair.Key);
				if (value == null)
				{
					// If the property value is null, we set null in the object too.
					propertyInfo.SetValue(instance, null);
					continue;
				}

				value = ConvertToEfValue(propertyInfo.PropertyType, value);
				if (value != null && !propertyInfo.PropertyType.IsInstanceOfType(value))
				{
					var dic = value as IReadOnlyDictionary<string, object>;
					if (dic == null)
					{
						throw new NotSupportedException(string.Format("Unsupported type for property: {0}.", propertyPair.Key));
					}

					value = Activator.CreateInstance(propertyInfo.PropertyType);
					SetValues(value, propertyInfo.PropertyType, dic);
				}

				propertyInfo.SetValue(instance, value);
			}
		}

		/// <summary>	Converts this object to an ef value. </summary>
		///
		/// <param name="type"> 	The type. </param>
		/// <param name="value">	The value. </param>
		///
		/// <returns>	The given data converted to an ef value. </returns>
		private static object ConvertToEfValue(Type type, object value)
		{
			// string[EdmType = Enum] => System.Enum
			if (TypeHelper.IsEnum(type))
			{
				return Enum.Parse(TypeHelper.GetUnderlyingTypeOrSelf(type), (string)value);
			}

			// Edm.Date => System.DateTime[SqlType = Date]
			if (value is Date)
			{
				var dateValue = (Date)value;
				return (DateTime)dateValue;
			}

			// System.DateTimeOffset => System.DateTime[SqlType = DateTime or DateTime2]
			if (value is DateTimeOffset && TypeHelper.IsDateTime(type))
			{
				var dateTimeOffsetValue = (DateTimeOffset)value;
				return dateTimeOffsetValue.DateTime;
			}

			// Edm.TimeOfDay => System.TimeSpan[SqlType = Time]
			if (value is TimeOfDay && TypeHelper.IsTimeSpan(type))
			{
				var timeOfDayValue = (TimeOfDay)value;
				return (TimeSpan)timeOfDayValue;
			}

			return value;
		}
	}

	/// <summary>	A type helper. </summary>
	internal static class TypeHelper
	{
		/// <summary>	Gets underlying type or self. </summary>
		///
		/// <param name="type">	The type. </param>
		///
		/// <returns>	The underlying type or self. </returns>
		public static Type GetUnderlyingTypeOrSelf(Type type)
		{
			return Nullable.GetUnderlyingType(type) ?? type;
		}

		/// <summary>	Query if 'type' is enum. </summary>
		///
		/// <param name="type">	The type. </param>
		///
		/// <returns>	true if enum, false if not. </returns>
		public static bool IsEnum(Type type)
		{
			Type underlyingTypeOrSelf = GetUnderlyingTypeOrSelf(type);
			return underlyingTypeOrSelf.IsEnum;
		}

		/// <summary>	Query if 'type' is date time. </summary>
		///
		/// <param name="type">	The type. </param>
		///
		/// <returns>	true if date time, false if not. </returns>
		public static bool IsDateTime(Type type)
		{
			Type underlyingTypeOrSelf = GetUnderlyingTypeOrSelf(type);
			return underlyingTypeOrSelf == typeof(DateTime);
		}

		/// <summary>	Query if 'type' is time span. </summary>
		///
		/// <param name="type">	The type. </param>
		///
		/// <returns>	true if time span, false if not. </returns>
		public static bool IsTimeSpan(Type type)
		{
			Type underlyingTypeOrSelf = GetUnderlyingTypeOrSelf(type);
			return underlyingTypeOrSelf == typeof(TimeSpan);
		}

		/// <summary>	Query if 'type' is date time offset. </summary>
		///
		/// <param name="type">	The type. </param>
		///
		/// <returns>	true if date time offset, false if not. </returns>
		public static bool IsDateTimeOffset(Type type)
		{
			Type underlyingTypeOrSelf = GetUnderlyingTypeOrSelf(type);
			return underlyingTypeOrSelf == typeof(DateTimeOffset);
		}
	}

	internal static class EnumerableExtensions
	{
		public static object SingleOrDefault(this IEnumerable enumerable)
		{
			IEnumerator enumerator = enumerable.GetEnumerator();
			object result = enumerator.MoveNext() ? enumerator.Current : null;

			if (enumerator.MoveNext())
			{
				throw new InvalidOperationException("A query for a single entity resulted in more than one record.");
			}

			return result;
		}
	}
}