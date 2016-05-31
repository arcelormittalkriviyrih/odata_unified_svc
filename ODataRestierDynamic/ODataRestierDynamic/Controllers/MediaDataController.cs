using Microsoft.OData.Core;
using ODataRestierDynamic.DynamicFactory;
using ODataRestierDynamic.Log;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace ODataRestierDynamic.Controllers
{
	/// <summary>	A controller for handling media data. </summary>
	public class MediaDataController : ODataController
	{
		/// <summary>	Name of the files entity. </summary>
		internal const string cFilesEntityName = "Files";

		/// <summary>	The API. </summary>
		private DynamicApi api;

		/// <summary>	Gets the API. </summary>
		///
		/// <value>	The API. </value>
		private DynamicApi Api
		{
			get
			{
				if (api == null)
				{
					api = new DynamicApi();
				}

				return api;
			}
		}

		/// <summary>	Gets a context for the database. </summary>
		///
		/// <value>	The database context. </value>
		private DynamicContext DbContext
		{
			get
			{
				return Api.Context;
			}
		}

		/// <summary>
		/// Returns a single entity by its key.
		/// </summary>
		/// <param name="key">The key of the entity to retrieve.</param>
		/// <returns>A <see cref="Task{T}">task</see> representing the asynchronous operation to retrieve an DynamicEntity</see>.</returns>
		private async Task<DynamicEntity> GetEntityByKeyAsync(int key)
		{
			DynamicEntity entity = null;

			Type relevantType = null;
			var task = Task.Run(() => DbContext.TryGetRelevantType(cFilesEntityName, out relevantType));
			var typeFound = await task;

			if (typeFound)
			{
				var dbSet = DbContext.Set(relevantType);
				if (dbSet != null)
				{
					entity = dbSet.Find(key) as DynamicEntity;
				}
			}

			return entity;
		}

		/// <summary>
		/// Creates a single entity.
		/// </summary>
		/// <returns>A <see cref="Task{T}">task</see> representing the asynchronous operation to retrieve an DynamicEntity</see>.</returns>
		private async Task<DynamicEntity> CreateNewEntity()
		{
			DynamicEntity entity = null;

			Type relevantType = null;
			var task = Task.Run(() => DbContext.TryGetRelevantType(cFilesEntityName, out relevantType));
			var typeFound = await task;

			if (typeFound)
			{
				entity = (DynamicEntity)Activator.CreateInstance(relevantType);
				var dbSet = DbContext.Set(relevantType);
				if (dbSet != null)
				{
					dbSet.Add(entity);
				}
			}

			return entity;
		}

		/// <summary>
		/// Ensures that stream can seek.
		/// </summary>
		/// <param name="stream">The stream for chack.</param>
		/// <returns></returns>
		private static Stream EnsureStreamCanSeek(Stream stream)
		{
			Contract.Requires(stream != null);
			Contract.Ensures(Contract.Result<Stream>() != null);
			Contract.Ensures(Contract.Result<Stream>().CanSeek);

			// stream is seekable
			if (stream.CanSeek)
				return stream;

			// stream is not seekable, so copy it into a memory stream so we can seek on it
			var copy = new MemoryStream();

			stream.CopyTo(copy);
			stream.Dispose();
			copy.Flush();
			copy.Seek(0L, SeekOrigin.Begin);

			return copy;
		}

		/// <summary>
		/// Gets the content type for the specified entity.
		/// </summary>
		/// <param name="entity">The <see cref="Image">image</see> whose stream to get the content type for.</param>
		/// <returns>The <see cref="MediaTypeHeaderValue">content type</see> of the stream associated with the <see cref="Image">image</see>.</returns>
		private MediaTypeHeaderValue GetContentTypeForStream(string mediaType, string mediaName)
		{
			return new MediaTypeHeaderValue(mediaType)
			{
				Parameters =
                {
                    new NameValueHeaderValue("name", string.Format(CultureInfo.InvariantCulture, "\"{0}\"", mediaName))
                }
			};
		}

		/// <summary>
		/// Gets a media resource for an entity with the specified key.
		/// </summary>
		/// <param name="key">The key of the entity to retrieve the media resource for.</param>
		/// <returns>A <see cref="Task{T}">task</see> containing the <see cref="HttpResponseMessage">response</see> for the request.</returns>
		/// <remarks>Media resources can be buffered if the appropriate HTTP range headers are specified in the request.</remarks>
		public async Task<System.Net.Http.HttpResponseMessage> GetMediaResource(int key)
		{
			Contract.Ensures(Contract.Result<Task<HttpResponseMessage>>() != null);

			// look up the entity
			var entity = await this.GetEntityByKeyAsync(key);
			if (entity == null)
				return this.Request.CreateResponse(HttpStatusCode.NotFound);

			// get the media resource stream from the entity
			var dataPropInfo = entity.GetType().GetProperty("Data");
			var bytes = dataPropInfo.GetValue(entity) as byte[];
			if (bytes == null)
				return this.Request.CreateResponse(HttpStatusCode.NotFound);
			var stream = new MemoryStream(bytes);
			if (stream == null)
				return this.Request.CreateResponse(HttpStatusCode.NotFound);

			var mediaType = new MediaTypeHeaderValue("application/octet-stream");

			// there should never be a stream without a corresponding entity, but if it somehow happens,
			// defensively use 'application/octet-stream' will represents any generic, binary stream
			if (entity != null)
			{
				var fileNamePropInfo = entity.GetType().GetProperty("FileName");
				var mediaNameStr = fileNamePropInfo.GetValue(entity) as string;
				//File.WriteAllBytes(@"D:\TestMediaData\test010101.xls", bytes);
				var mimeTypePropInfo = entity.GetType().GetProperty("MIMEType");
				string mediaTypeStr = mimeTypePropInfo.GetValue(entity) as string;
				mediaType = this.GetContentTypeForStream(mediaTypeStr, mediaNameStr);
			}

			// get the range and stream media type
			var range = this.Request.Headers.Range;
			HttpResponseMessage response;

			if (range == null)
			{
				// if the range header is present but null, then the header value must be invalid
				if (this.Request.Headers.Contains("Range"))
				{
					//var error = new ODataError
					//{
					//	Message = ExceptionMessage.ControllerRangeNotSatisfiable,
					//	MessageLanguage = ExceptionMessage.ErrorMessageLanguage,
					//	ErrorCode = string.Format(CultureInfo.CurrentCulture, ExceptionMessage.ControllerRangeNotSatisfiableErrorCode, stream.Length - 1L)
					//};
					return this.Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "");
				}

				// if no range was requested, return the entire stream
				response = this.Request.CreateResponse(HttpStatusCode.OK);

				response.Headers.AcceptRanges.Add("bytes");
				response.Content = new StreamContent(stream);
				response.Content.Headers.ContentType = mediaType;

				return response;
			}

			var partialStream = EnsureStreamCanSeek(stream);

			response = this.Request.CreateResponse(HttpStatusCode.PartialContent);
			response.Headers.AcceptRanges.Add("bytes");

			try
			{
				// return the requested range(s)
				response.Content = new ByteRangeStreamContent(partialStream, range, mediaType);
			}
			catch (InvalidByteRangeException exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("GetMediaResource", exception);
				response.Dispose();
				return Request.CreateErrorResponse(exception);
			}

			// change status code if the entire stream was requested
			if (response.Content.Headers.ContentLength.Value == partialStream.Length)
				response.StatusCode = HttpStatusCode.OK;

			return response;
		}

		/// <summary>	Posts a media resource. </summary>
		///
		/// <exception cref="HttpResponseException">	Thrown when a HTTP Response error condition
		/// 											occurs. </exception>
		///
		/// <param name="key">	The key of the entity to retrieve the media resource for. </param>
		///
		/// <returns>	A Task&lt;System.Net.Http.HttpResponseMessage&gt; </returns>
		public async Task<System.Net.Http.HttpResponseMessage> PostMediaResource(int key)
		{
			// Check whether the POST operation is MultiPart?
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			// Prepare CustomMultipartFormDataStreamProvider in which our multipart form data will be loaded.
			string fileSaveLocation = Path.GetTempPath();//HttpContext.Current.Server.MapPath("~/App_Data");
			//MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();
			MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(fileSaveLocation);
			HttpResponseMessage response;

			try
			{
				// Look up the entity
				DynamicEntity entity = await this.GetEntityByKeyAsync(key);
				if (entity == null)
				{
					entity = await this.CreateNewEntity();
					if (key != -1)
					{
						var idPropInfo = entity.GetType().GetProperty("ID");
						idPropInfo.SetValue(entity, key);
					}
				}

				// Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
				await Request.Content.ReadAsMultipartAsync(provider);

				foreach (var keyProp in provider.FormData.AllKeys)
				{
					var fieldPropInfo = entity.GetType().GetProperty(keyProp);
					if (fieldPropInfo != null)
					{
						var value = provider.FormData[keyProp];
						fieldPropInfo.SetValue(entity, value);
					}
				}

				foreach (MultipartFileData file in provider.FileData)
				{
					var dataPropInfo = entity.GetType().GetProperty(file.Headers.ContentDisposition.Name.Trim('"'));
					if (dataPropInfo != null)
					{
						byte[] fileContent = File.ReadAllBytes(file.LocalFileName);
						dataPropInfo.SetValue(entity, fileContent);
					}
					File.Delete(file.LocalFileName);
				}

				await this.DbContext.SaveChangesAsync();

				if (key == -1)
				{
					var idPropInfo = entity.GetType().GetProperty("ID");
					key = (int)idPropInfo.GetValue(entity);
				}

				// Send OK Response along with saved file names to the client.
				response = Request.CreateResponse(HttpStatusCode.NoContent);
				response.Headers.Add("ID", key.ToString());
				//response.Content = new StringContent(key.ToString());
			}
			catch (System.Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("PostMediaResource", exception);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
			}

			return response;
		}
	}
}
