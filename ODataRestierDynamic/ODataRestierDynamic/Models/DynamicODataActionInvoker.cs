using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.Restier.WebApi;
using ODataRestierDynamic.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter.Deserialization;
using System.Web.OData.Routing;

namespace ODataRestierDynamic.Models
{
	/// <summary>
	/// Contains method that is used to invoke HTTP operation.
	/// </summary>
	public class DynamicODataActionInvoker : IHttpActionInvoker
	{
		internal const string cPostActionName = "PostAction";

		private readonly IHttpActionInvoker _innerInvoker;

		/// <summary>
		/// Initializes a new instance of the <see cref="ODataActionSelector" /> class.
		/// </summary>
		/// <param name="innerInvoker">The inner controller selector to call.</param>
		public DynamicODataActionInvoker(IHttpActionInvoker innerInvoker)
		{
			_innerInvoker = innerInvoker;
		}

		/// <summary>
		/// Executes asynchronously the HTTP operation.
		/// </summary>
		/// <param name="actionContext">The execution context.</param>
		/// <param name="cancellationToken">The cancellation token assigned for the HTTP operation.</param>
		/// <returnsThe newly started task.></returns>
		public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
		{
			System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> result = null;

			try
			{
				if (_innerInvoker != null)
				{
					var controllerContext = actionContext.ControllerContext;
					if (controllerContext.Controller is RestierController)
					{
						var request = controllerContext.Request;
						IEdmModel model = request.ODataProperties().Model;
						ODataPath odataPath = request.ODataProperties().Path;
						var routeData = controllerContext.RouteData;

						if (routeData != null)
						{
							var actionMethodName = routeData.Values["action"] as string;
							if (actionMethodName == cPostActionName)
							{
                                if (odataPath.Segments.Last() is UnboundActionPathSegment actionPathSegment)
                                {
                                    Stream stream = request.Content.ReadAsStreamAsync().Result;
                                    ODataMessageWrapper message = new ODataMessageWrapper(stream);
                                    message.SetHeader("Content-Type", request.Content.Headers.ContentType.MediaType);
                                    ODataMessageReader reader = new ODataMessageReader(message as IODataRequestMessage, new ODataMessageReaderSettings(), model);
                                    ODataDeserializerContext readContext = new ODataDeserializerContext { Path = odataPath, Model = model };
                                    ODataActionParameters payload = ReadParams(reader, actionPathSegment.Action.Operation, readContext);

                                    var dynamicController = new Controllers.DynamicController
                                    {
                                        ControllerContext = controllerContext
                                    };
                                    result = dynamicController.CallAction(actionPathSegment.ActionName, payload, cancellationToken);
                                }
                            }

							if (odataPath != null && odataPath.Segments.Count == 3 && 
								odataPath.Segments[0].ToString() == Controllers.MediaDataController.cFilesEntityName && 
								odataPath.Segments.Last() is ValuePathSegment)
							{
                                var mediaDataController = new Controllers.MediaDataController
                                {
                                    ControllerContext = controllerContext
                                };
                                var keyValuePathSegment = odataPath.Segments.First(x => x is KeyValuePathSegment);
								int key = int.Parse(((KeyValuePathSegment)keyValuePathSegment).Value);

								if (actionMethodName.Equals(System.Net.WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
								{
									result = mediaDataController.GetMediaResource(key);
								}
								else if (actionMethodName.Equals(cPostActionName, StringComparison.InvariantCultureIgnoreCase))
								{
									result = mediaDataController.PostMediaResource(key);
								}
							}
						}
					}

					if (result == null)
					{
						result = _innerInvoker.InvokeActionAsync(actionContext, cancellationToken);
					}
				}
			}
			catch (Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("InvokeActionAsync", exception);
				throw;
			}

			return result;
		}

		/// <summary>
		/// Reads Parameter names and values provided by a client in a POST request to invoke a particular Action.
		/// </summary>
		/// <param name="messageReader">Reader used to read all OData payloads (entries, feeds, metadata documents, service documents, etc.).</param>
		/// <param name="action">Represents an EDM operation.</param>
		/// <param name="readContext">
		/// Encapsulates the state and settings that get passed to System.Web.OData.Formatter.Deserialization.ODataDeserializer from the System.Web.OData.Formatter.ODataMediaTypeFormatter.
		/// </param>
		/// <returns>ActionPayload holds the Parameter names and values provided by a client in a POST request to invoke a particular Action.</returns>
		private ODataActionParameters ReadParams(ODataMessageReader messageReader, IEdmOperation action, ODataDeserializerContext readContext)
		{
			// Create the correct resource type;
			ODataActionParameters payload = new ODataActionParameters();

			try
			{
				ODataParameterReader reader = messageReader.CreateODataParameterReader(action);

				while (reader.Read())
				{
					string parameterName = null;
					IEdmOperationParameter parameter = null;

					switch (reader.State)
					{
						case ODataParameterReaderState.Value:
							parameterName = reader.Name;
							parameter = action.Parameters.SingleOrDefault(p => p.Name == parameterName);
							// ODataLib protects against this but asserting just in case.
							Contract.Assert(parameter != null, String.Format(CultureInfo.InvariantCulture, "Parameter '{0}' not found.", parameterName));
							if (parameter.Type.IsPrimitive())
							{
								payload[parameterName] = reader.Value;
							}
							else
							{
								ODataEdmTypeDeserializer deserializer = DefaultODataDeserializerProvider.Instance.GetEdmTypeDeserializer(parameter.Type);
								payload[parameterName] = deserializer.ReadInline(reader.Value, parameter.Type, readContext);
							}
							break;

						case ODataParameterReaderState.Collection:
							parameterName = reader.Name;
							parameter = action.Parameters.SingleOrDefault(p => p.Name == parameterName);
							// ODataLib protects against this but asserting just in case.
							Contract.Assert(parameter != null, String.Format(CultureInfo.InvariantCulture, "Parameter '{0}' not found.", parameterName));
							IEdmCollectionTypeReference collectionType = parameter.Type as IEdmCollectionTypeReference;
							Contract.Assert(collectionType != null);
							ODataCollectionValue value = ReadCollection(reader.CreateCollectionReader());
							ODataCollectionDeserializer collectionDeserializer = DefaultODataDeserializerProvider.Instance.GetEdmTypeDeserializer(collectionType) as ODataCollectionDeserializer;
							payload[parameterName] = collectionDeserializer.ReadInline(value, collectionType, readContext);
							break;

						default:
							break;
					}
				}
			}
			catch (Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("ReadParams", exception);
				throw;
			}

			return payload;
		}

		/// <summary>
		/// Reads Collection Parameter values provided by a client in a POST request to invoke a particular Action.
		/// </summary>
		/// <param name="reader">OData collection reader.</param>
		/// <returns>OData representation of a Collection.</returns>
		private ODataCollectionValue ReadCollection(ODataCollectionReader reader)
		{
			ArrayList items = new ArrayList();
			string typeName = null;

			while (reader.Read())
			{
				if (ODataCollectionReaderState.Value == reader.State)
				{
					items.Add(reader.Item);
				}
				else if (ODataCollectionReaderState.CollectionStart == reader.State)
				{
					typeName = reader.Item.ToString();
				}
			}

			return new ODataCollectionValue { Items = items, TypeName = typeName };
		}
	}
}