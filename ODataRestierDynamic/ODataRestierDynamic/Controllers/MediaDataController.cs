﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.OData.Core;
using ODataRestierDynamic.DynamicFactory;
using ODataRestierDynamic.Log;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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

		/// <summary>	Name of the template entity. </summary>
		private const string cTemplateEntityName = "v_PrintTemplate";

		/// <summary>	Field of the id. </summary>
		private const string cFieldID = "ID";

		/// <summary>	Field name. </summary>
		private const string cFieldName = "Name";

		/// <summary>	Field file name. </summary>
		private const string cFieldFileName = "FileName";

		/// <summary>	Field file type. </summary>
		private const string cFieldFileType = "FileType";

		/// <summary>	Field of the data. </summary>
		private const string cFieldData = "Data";

		/// <summary>	Field of the preview id. </summary>
		private const string cFieldPreviewID = "PreviewID";

		/// <summary>	File type excel label. </summary>
		private const string cExcelLabelFileType = "Excel label";

		/// <summary>	File type excel preview. </summary>
		private const string cExcelPreviewFileType = "Excel preview";
        		
		/// <summary> Excel preview temp path. </summary>
		private static string m_ExcelPreviewTempPath = ConfigurationManager.AppSettings["ExcelPreviewTempPath"];

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

		/// <summary>	Gets template entities. </summary>
		///
		/// <returns>	The template entities. </returns>
		private async Task<IQueryable<DynamicEntity>> GetTemplateEntities()
		{
			IQueryable<DynamicEntity> templateEntities = null;

			Type relevantType = null;
			var task = Task.Run(() => DbContext.TryGetRelevantType(cTemplateEntityName, out relevantType));
			var typeFound = await task;

			if (typeFound)
			{
				var dbSet = DbContext.Set(relevantType);
				if (dbSet != null)
				{
					templateEntities = dbSet as IQueryable<DynamicEntity>;
				}
			}

			return templateEntities;
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
					return this.Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "GetMediaResource");
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
				// Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
				await Request.Content.ReadAsMultipartAsync(provider);

				// Look up the entity
				DynamicEntity entity = await this.GetEntityByKeyAsync(key);
				if (entity == null)
				{
					entity = await this.CreateNewEntity();
					if (key != -1)
					{
						var idPropInfo = entity.GetType().GetProperty(cFieldID);
						idPropInfo.SetValue(entity, key);
					}
				}

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
				}

				await this.DbContext.SaveChangesAsync();

				if (key == -1)
				{
					var idPropInfo = entity.GetType().GetProperty(cFieldID);
					key = (int)idPropInfo.GetValue(entity);
				}

				// Generate Excel preview if Excel Label file type
				if (provider.FormData.AllKeys.Contains(cFieldFileType) && provider.FormData[cFieldFileType].ToString() == cExcelLabelFileType)
				{
					int previewId = -1;
					if (entity != null)
					{
						var previewIdPropInfo = entity.GetType().GetProperty(cFieldPreviewID);
						if (previewIdPropInfo != null)
						{
							object value = previewIdPropInfo.GetValue(entity);
							if (value != null && value is int)
							{
								previewId = (int)value;
							}
						}
					}
					previewId = await this.GenerateExcelPreview(previewId, provider.FormData, provider.FileData.First());
					if (previewId != -1)
					{
						var previewIdPropInfo = entity.GetType().GetProperty(cFieldPreviewID);
						if (previewIdPropInfo != null)
						{
							previewIdPropInfo.SetValue(entity, previewId);
							await this.DbContext.SaveChangesAsync();
						}
					}
				}

				// Send OK Response along with saved file names to the client.
				response = Request.CreateResponse(HttpStatusCode.NoContent);
				response.Headers.Add(cFieldID, key.ToString());
				//response.Content = new StringContent(key.ToString());
			}
			catch (System.Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("PostMediaResource", exception);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
			}
			finally
			{
				if (provider != null)
				{
					foreach (MultipartFileData file in provider.FileData)
					{
						if (File.Exists(file.LocalFileName))
							File.Delete(file.LocalFileName);
					}
				}
			}

			return response;
		}

		private async Task<int> GenerateExcelPreview(int previewId, System.Collections.Specialized.NameValueCollection formData, MultipartFileData file)
		{
			try
			{
				//if (!File.Exists(m_GenerateExcelPreviewUtilityName))
				//{
				//	throw new FileNotFoundException(
				//		string.Format("GenerateExcelPreview: Excel preview generate utility not found in path {0}.", m_GenerateExcelPreviewUtilityName));
				//}

				string fileNameGuid = formData.AllKeys.Contains(cFieldFileName) ? Path.GetFileNameWithoutExtension(formData[cFieldFileName]) : Path.GetRandomFileName();
				string outputFileName = Path.Combine(m_ExcelPreviewTempPath, fileNameGuid + ".png");

				xlsConverter.Program.Convert(file.LocalFileName, outputFileName);

				//var startInfo = new System.Diagnostics.ProcessStartInfo(m_GenerateExcelPreviewUtilityName);
				//startInfo.UseShellExecute = false;
				//startInfo.Arguments = "\"" + file.LocalFileName + "\" \"" + outputFileName + "\"";
				//startInfo.RedirectStandardInput = true;
				//startInfo.RedirectStandardOutput = true;
				//startInfo.CreateNoWindow = true;

				//var process = System.Diagnostics.Process.Start(startInfo);
				//process.WaitForExit(30000);
				//if (process.HasExited == false)
				//	process.Kill();
				//int exitcode = process.ExitCode;
				//process.Close();

				if (!File.Exists(outputFileName))
				{
					throw new Exception("GenerateExcelPreview: Something gone wrong in Excel Preview Utility");
				}

				// Look up the entity
				DynamicEntity entity = await this.GetEntityByKeyAsync(previewId);
				if (entity == null)
				{
					entity = await this.CreateNewEntity();
					if (previewId != -1)
					{
						var idPropInfo = entity.GetType().GetProperty(cFieldID);
						idPropInfo.SetValue(entity, previewId);
					}
				}

				foreach (var keyProp in formData.AllKeys)
				{
					var fieldPropInfo = entity.GetType().GetProperty(keyProp);
					if (fieldPropInfo != null)
					{
						var value = formData[keyProp];
						if (keyProp == cFieldFileName)
							value = Path.GetFileName(outputFileName);
						if (keyProp == cFieldFileType)
							value = cExcelPreviewFileType;
						if (keyProp == cFieldName)
							value = "Preview_" + value;
						fieldPropInfo.SetValue(entity, value);
					}
				}

				var dataPropInfo = entity.GetType().GetProperty(cFieldData);
				if (dataPropInfo != null)
				{
					byte[] fileContent = File.ReadAllBytes(outputFileName);
					dataPropInfo.SetValue(entity, fileContent);
				}
				File.Delete(outputFileName);

				await this.DbContext.SaveChangesAsync();

				if (previewId == -1)
				{
					var idPropInfo = entity.GetType().GetProperty(cFieldID);
					previewId = (int)idPropInfo.GetValue(entity);
				}
			}
			catch (System.Exception exception)
			{
				DynamicLogger.Instance.WriteLoggerLogError("GenerateExcelPreview", exception);
			}

			return previewId;
		}

		/// <summary>	(An Action that handles HTTP GET requests) generates a template. </summary>
		///
		/// <returns>	The template. </returns>
		[HttpGet]
		public async Task<System.Net.Http.HttpResponseMessage> GenerateTemplate()
		{
			Contract.Ensures(Contract.Result<Task<HttpResponseMessage>>() != null);

			// get template entities
			var templateEntities = await this.GetTemplateEntities();
			if (templateEntities == null)
				return this.Request.CreateResponse(HttpStatusCode.NotFound);

			#region Generate Excel File OpenXMLSDK-MOT

			string fileSaveLocation = Path.GetTempFileName();
			var properties = templateEntities.ElementType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.Name != "ID");
			using (var workbook = SpreadsheetDocument.Create(fileSaveLocation, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
			{
				var workbookPart = workbook.AddWorkbookPart();
				WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
				stylesPart.Stylesheet = CreateStylesheet();
				stylesPart.Stylesheet.Save();
				workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
				workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

				var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
				var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
				sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
				SheetFormatProperties sheetFormatProperties = new SheetFormatProperties() { DefaultColumnWidth = 25.00D, DefaultRowHeight = 0D };
				sheetPart.Worksheet.SheetFormatProperties = sheetFormatProperties;

				DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
				string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

				uint sheetId = 1;
				if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
				{
					sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
				}

				DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = cTemplateEntityName };
				sheets.Append(sheet);

				DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
				List<String> columns = new List<string>();
				foreach (PropertyInfo column in properties)
				{
					columns.Add(column.Name);

					DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
					cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
					cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.Name);
					cell.StyleIndex = Convert.ToUInt32(1);
					headerRow.AppendChild(cell);
				}

				sheetData.AppendChild(headerRow);

				foreach (var entity in templateEntities)
				{
					DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
					foreach (string col in columns)
					{
						DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
						cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
						object value = templateEntities.ElementType.GetProperty(col).GetValue(entity);
						cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value == null ? string.Empty : value.ToString());
						newRow.AppendChild(cell);
					}

					sheetData.AppendChild(newRow);
				}
			}

			var bytes = File.ReadAllBytes(fileSaveLocation);
			File.Delete(fileSaveLocation);
			var stream = new MemoryStream(bytes);
			if (stream == null)
				return this.Request.CreateResponse(HttpStatusCode.NotFound);

			#endregion

			var mediaNameStr = "Template.xlsx";
			var mediaType = this.GetContentTypeForStream("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", mediaNameStr);

			// get the range and stream media type
			var range = this.Request.Headers.Range;
			HttpResponseMessage response;

			if (range == null)
			{
				// if the range header is present but null, then the header value must be invalid
				if (this.Request.Headers.Contains("Range"))
				{
					return this.Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "GenerateTemplate");
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
				DynamicLogger.Instance.WriteLoggerLogError("GenerateTemplate", exception);
				response.Dispose();
				return Request.CreateErrorResponse(exception);
			}

			// change status code if the entire stream was requested
			if (response.Content.Headers.ContentLength.Value == partialStream.Length)
				response.StatusCode = HttpStatusCode.OK;

			return response;
		}

		/// <summary>	Creates the stylesheet. </summary>
		///
		/// <returns>	The new stylesheet. </returns>
		private static Stylesheet CreateStylesheet()
		{
			Stylesheet stylesheet = new Stylesheet();

			Font font0 = new Font();         // Default font

			Font font1 = new Font();         // Bold font
			Bold bold = new Bold();
			font1.Append(bold);

			Fonts fonts = new Fonts();      // <APENDING Fonts>
			fonts.Append(font0);
			fonts.Append(font1);

			// <Fills>
			Fill fill0 = new Fill();        // Default fill

			Fills fills = new Fills();      // <APENDING Fills>
			fills.Append(fill0);

			// <Borders>
			Border border0 = new Border();     // Defualt border

			Borders borders = new Borders();    // <APENDING Borders>
			borders.Append(border0);

			CellFormat cellformat0 = new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 }; // Default style : Mandatory | Style ID =0

			CellFormat cellformat1 = new CellFormat() { FontId = 1 };
			CellFormats cellformats = new CellFormats();
			cellformats.Append(cellformat0);
			cellformats.Append(cellformat1);

			stylesheet.Append(fonts);
			stylesheet.Append(fills);
			stylesheet.Append(borders);
			stylesheet.Append(cellformats);

			return stylesheet;
		}
	}
}
