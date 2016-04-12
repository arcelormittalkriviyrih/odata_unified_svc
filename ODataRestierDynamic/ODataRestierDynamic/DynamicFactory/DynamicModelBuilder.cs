using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
			if (model != null)
			{
				var dbContext = context.ApiContext.GetProperty<DynamicContext>(DynamicContext.cDbContextKey);

				//var edmAction = new EdmAction("ODataRestierDynamic.Models", "ins_MaterialLotByController", EdmCoreModel.Instance.GetInt32(false));
				//edmAction.AddParameter("controllerID", EdmCoreModel.Instance.GetInt32(false));

				//model.AddElement(edmAction);

				try
				{
					this.BuildActions(model, dbContext.DynamicActions);
				}
				catch
				{
					throw;
				}
			}

			return model;
		}

		private static bool TryGetEntityType(IEdmModel model, Type type, out IEdmEntityType entityType)
		{
			var edmType = model.FindDeclaredType(type.FullName);
			entityType = edmType as IEdmEntityType;
			return entityType != null;
		}

		private static void BuildOperationParameters(EdmOperation operation, MethodInfo method, IEdmModel model)
		{
			foreach (ParameterInfo parameter in method.GetParameters())
			{
				var parameterTypeReference = GetTypeReference(parameter.ParameterType, model);
				var operationParam = new EdmOperationParameter(
					operation,
					parameter.Name,
					parameterTypeReference);

				operation.AddParameter(operationParam);
			}
		}

		private static bool TryGetBindingParameter(
			MethodInfo method, IEdmModel model, out ParameterInfo bindingParameter)
		{
			bindingParameter = null;
			var firstParameter = method.GetParameters().FirstOrDefault();
			if (firstParameter == null)
			{
				return false;
			}

			Type parameterType;
			if (!firstParameter.ParameterType.TryGetElementType(out parameterType))
			{
				parameterType = firstParameter.ParameterType;
			}

			if (!GetTypeReference(parameterType, model).IsEntity())
			{
				return false;
			}

			bindingParameter = firstParameter;
			return true;
		}

		private static IEdmTypeReference GetReturnTypeReference(Type type, IEdmModel model)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
			{
				// if the action returns a Task<T>, map that to just be returning a T
				type = type.GetGenericArguments()[0];
			}
			else if (type == typeof(Task))
			{
				// if the action returns a concrete Task, map that to being a void return type.
				type = typeof(void);
			}

			return GetTypeReference(type, model);
		}

		private static IEdmTypeReference GetTypeReference(Type type, IEdmModel model)
		{
			Type elementType;
			if (type.TryGetElementType(out elementType))
			{
				return EdmCoreModel.GetCollection(GetTypeReference(elementType, model));
			}

			IEdmEntityType entityType;
			if (TryGetEntityType(model, type, out entityType))
			{
				return new EdmEntityTypeReference(entityType, true);
			}

			return GetPrimitiveTypeReference(type);
		}

		public static EdmTypeReference GetPrimitiveTypeReference(Type type)
		{
			// Only handle primitive type right now
			bool isNullable;
			EdmPrimitiveTypeKind? primitiveTypeKind = GetPrimitiveTypeKind(type, out isNullable);

			if (!primitiveTypeKind.HasValue)
			{
				return null;
			}

			return new EdmPrimitiveTypeReference(
				EdmCoreModel.Instance.GetPrimitiveType(primitiveTypeKind.Value),
				isNullable);
		}

		private static EdmPrimitiveTypeKind? GetPrimitiveTypeKind(Type type, out bool isNullable)
		{
			isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
			if (isNullable)
			{
				type = type.GetGenericArguments()[0];
			}

			if (type == typeof(string))
			{
				return EdmPrimitiveTypeKind.String;
			}

			if (type == typeof(byte[]))
			{
				return EdmPrimitiveTypeKind.Binary;
			}

			if (type == typeof(bool))
			{
				return EdmPrimitiveTypeKind.Boolean;
			}

			if (type == typeof(byte))
			{
				return EdmPrimitiveTypeKind.Byte;
			}

			if (type == typeof(DateTime))
			{
				return EdmPrimitiveTypeKind.DateTimeOffset;
				// TODO GitHubIssue#49 : how to map DateTime's in OData v4?  there is no Edm.DateTime type anymore
				//return null;
			}

			if (type == typeof(DateTimeOffset))
			{
				return EdmPrimitiveTypeKind.DateTimeOffset;
			}

			if (type == typeof(decimal))
			{
				return EdmPrimitiveTypeKind.Decimal;
			}

			if (type == typeof(double))
			{
				return EdmPrimitiveTypeKind.Double;
			}

			if (type == typeof(Guid))
			{
				return EdmPrimitiveTypeKind.Guid;
			}

			if (type == typeof(short))
			{
				return EdmPrimitiveTypeKind.Int16;
			}

			if (type == typeof(int))
			{
				return EdmPrimitiveTypeKind.Int32;
			}

			if (type == typeof(long))
			{
				return EdmPrimitiveTypeKind.Int64;
			}

			if (type == typeof(sbyte))
			{
				return EdmPrimitiveTypeKind.SByte;
			}

			if (type == typeof(float))
			{
				return EdmPrimitiveTypeKind.Single;
			}

			if (type == typeof(TimeSpan))
			{
				// TODO GitHubIssue#49 : this should really be TimeOfDay,
				// but EdmPrimitiveTypeKind doesn't support that type.
				////return EdmPrimitiveTypeKind.TimeOfDay;
				return EdmPrimitiveTypeKind.Duration;
			}

			if (type == typeof(void))
			{
				return null;
			}

			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Not Supported Type = {0}", type.FullName));
		}

		private static EdmPathExpression BuildEntitySetPathExpression(
			IEdmTypeReference returnTypeReference, ParameterInfo bindingParameter)
		{
			if (returnTypeReference != null &&
				returnTypeReference.IsEntity() &&
				bindingParameter != null)
			{
				return new EdmPathExpression(bindingParameter.Name);
			}

			return null;
		}

		private static EdmEntitySetReferenceExpression BuildEntitySetReferenceExpression(
			IEdmModel model, string entitySetName, IEdmTypeReference returnTypeReference)
		{
			IEdmEntitySet entitySet = null;
			if (entitySetName != null)
			{
				entitySet = model.EntityContainer.FindEntitySet(entitySetName);
			}

			if (entitySet == null && returnTypeReference != null)
			{
				entitySet = FindDeclaredEntitySetByTypeReference(model, returnTypeReference);
			}

			if (entitySet != null)
			{
				return new EdmEntitySetReferenceExpression(entitySet);
			}

			return null;
		}

		private static IEdmEntitySet FindDeclaredEntitySetByTypeReference(IEdmModel model, IEdmTypeReference typeReference)
		{
			IEdmTypeReference elementTypeReference;
			if (!TryGetElementTypeReference(typeReference, out elementTypeReference))
			{
				elementTypeReference = typeReference;
			}

			if (!elementTypeReference.IsEntity())
			{
				return null;
			}

			return model.EntityContainer.EntitySets().SingleOrDefault(e => e.EntityType().FullTypeName() == elementTypeReference.FullName());
		}

		private static bool TryGetElementTypeReference(IEdmTypeReference typeReference, out IEdmTypeReference elementTypeReference)
		{
			if (!typeReference.IsCollection())
			{
				elementTypeReference = null;
				return false;
			}

			elementTypeReference = typeReference.AsCollection().ElementType();
			return true;
		}

		private void BuildActions(EdmModel model, Type targetType)
		{
			var methods = targetType.GetMethods(
				BindingFlags.NonPublic |
				BindingFlags.Public |
				BindingFlags.Static |
				BindingFlags.Instance);

			ICollection<ActionMethodInfo> actionInfos = new List<ActionMethodInfo>();

			foreach (var method in methods)
			{
				//var functionAttribute = method.GetCustomAttributes<FunctionAttribute>(true).FirstOrDefault();
				//if (functionAttribute != null)
				//{
				//	functionInfos.Add(new FunctionMethodInfo
				//	{
				//		Method = method,
				//		FunctionAttribute = functionAttribute
				//	});
				//}

				var actionAttribute = method.GetCustomAttributes<ActionAttribute>(true).FirstOrDefault();
				if (actionAttribute != null)
				{
					actionInfos.Add(new ActionMethodInfo
					{
						Method = method,
						ActionAttribute = actionAttribute
					});
				}
			}

			foreach (ActionMethodInfo actionInfo in actionInfos)
			{
				var returnTypeReference = GetReturnTypeReference(actionInfo.Method.ReturnType, model);

				ParameterInfo bindingParameter;
				bool isBound = TryGetBindingParameter(actionInfo.Method, model, out bindingParameter);

				var action = new EdmAction(
					actionInfo.Namespace,
					actionInfo.Name,
					returnTypeReference,
					isBound,
					BuildEntitySetPathExpression(returnTypeReference, bindingParameter));
				BuildOperationParameters(action, actionInfo.Method, model);
				model.AddElement(action);

				if (!isBound)
				{
					var entitySetReferenceExpression = BuildEntitySetReferenceExpression(model, actionInfo.EntitySet, returnTypeReference);
					var entityContainer = EnsureEntityContainer(model, targetType);
					entityContainer.AddActionImport(action.Name, action, entitySetReferenceExpression);
				}
			}
		}

		private const string DefaultEntityContainerName = "DefaultContainer";

		public static EdmEntityContainer EnsureEntityContainer(EdmModel model, Type apiType)
		{
			var container = (EdmEntityContainer)model.EntityContainer;
			if (container == null)
			{
				container = new EdmEntityContainer(apiType.Namespace, DefaultEntityContainerName);
				model.AddElement(container);
			}

			return container;
		}

		#region Class ActionMethodInfo

		private class ActionMethodInfo
		{
			public MethodInfo Method { get; set; }

			public ActionAttribute ActionAttribute { get; set; }

			public string Name
			{
				get { return this.ActionAttribute.Name ?? this.Method.Name; }
			}

			public string Namespace
			{
				get { return this.ActionAttribute.Namespace ?? this.Method.DeclaringType.Namespace; }
			}

			public string EntitySet
			{
				get { return this.ActionAttribute.EntitySet; }
			}
		}

		#endregion
	}

	#region Class TypeExtensions

	internal static partial class TypeExtensions
	{
		private const BindingFlags QualifiedMethodBindingFlags = BindingFlags.NonPublic |
																 BindingFlags.Static |
																 BindingFlags.Instance |
																 BindingFlags.IgnoreCase |
																 BindingFlags.DeclaredOnly;

		/// <summary>
		/// Find a base type or implemented interface which has a generic definition
		/// represented by the parameter, <c>definition</c>.
		/// </summary>
		/// <param name="type">
		/// The subject type.
		/// </param>
		/// <param name="definition">
		/// The generic definition to check with.
		/// </param>
		/// <returns>
		/// The base type or the interface found; otherwise, <c>null</c>.
		/// </returns>
		public static Type FindGenericType(this Type type, Type definition)
		{
			if (type == null)
			{
				return null;
			}

			// If the type conforms the given generic definition, no further check required.
			if (type.IsGenericDefinition(definition))
			{
				return type;
			}

			// If the definition is interface, we only need to check the interfaces implemented by the current type
			if (definition.IsInterface)
			{
				foreach (var interfaceType in type.GetInterfaces())
				{
					if (interfaceType.IsGenericDefinition(definition))
					{
						return interfaceType;
					}
				}
			}
			else if (!type.IsInterface)
			{
				// If the definition is not an interface, then the current type cannot be an interface too.
				// Otherwise, we should only check the parent class types of the current type.

				// no null check for the type required, as we are sure it is not an interface type
				while (type != typeof(object))
				{
					if (type.IsGenericDefinition(definition))
					{
						return type;
					}

					type = type.BaseType;
				}
			}

			return null;
		}

		public static MethodInfo GetQualifiedMethod(this Type type, string methodName)
		{
			return type.GetMethod(methodName, QualifiedMethodBindingFlags);
		}

		public static bool TryGetElementType(this Type type, out Type elementType)
		{
			// Special case: string implements IEnumerable<char> however it should
			// NOT be treated as a collection type.
			if (type == typeof(string))
			{
				elementType = null;
				return false;
			}

			var interfaceType = type.FindGenericType(typeof(IEnumerable<>));
			if (interfaceType != null)
			{
				elementType = interfaceType.GetGenericArguments()[0];
				return true;
			}

			elementType = null;
			return false;
		}

		private static bool IsGenericDefinition(this Type type, Type definition)
		{
			return type.IsGenericType &&
				   type.GetGenericTypeDefinition() == definition;
		}
	}

	#endregion
}