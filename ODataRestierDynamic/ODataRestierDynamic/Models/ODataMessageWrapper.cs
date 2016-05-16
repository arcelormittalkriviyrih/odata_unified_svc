using Microsoft.OData.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Runtime;

namespace ODataRestierDynamic.Models
{
	/// <summary>	Wrapper for Interface for synchronous OData request messages. </summary>
	internal class ODataMessageWrapper : IODataRequestMessage
	{
		/// <summary>	Stream of params provided by a client in a POST request. </summary>
		private Stream _stream;
		/// <summary>	The headers. </summary>
		private Dictionary<string, string> _headers;
		/// <summary>	The content identifier mapping. </summary>
		private IDictionary<string, string> _contentIdMapping;

		/// <summary>	Default constructor. </summary>
		public ODataMessageWrapper()
			: this(stream: null, headers: null)
		{
		}

		/// <summary>	Constructor. </summary>
		///
		/// <param name="stream">	Stream of params provided by a client in a POST request. </param>
		public ODataMessageWrapper(Stream stream)
			: this(stream: stream, headers: null)
		{
		}

		/// <summary>	Constructor. </summary>
		///
		/// <param name="stream"> 	Stream of params provided by a client in a POST request. </param>
		/// <param name="headers">	An enumerable over all the headers for this message. </param>
		public ODataMessageWrapper(Stream stream, HttpHeaders headers)
			: this(stream: stream, headers: headers, contentIdMapping: null)
		{
		}

		/// <summary>	Constructor. </summary>
		///
		/// <param name="stream">		   	Stream of params provided by a client in a POST request. </param>
		/// <param name="headers">		   	An enumerable over all the headers for this message. </param>
		/// <param name="contentIdMapping">	The content identifier mapping. </param>
		public ODataMessageWrapper(Stream stream, HttpHeaders headers, IDictionary<string, string> contentIdMapping)
		{
			_stream = stream;
			if (headers != null)
			{
				_headers = headers.ToDictionary(kvp => kvp.Key, kvp => String.Join(";", kvp.Value));
			}
			else
			{
				_headers = new Dictionary<string, string>();
			}
			_contentIdMapping = contentIdMapping ?? new Dictionary<string, string>();
		}

		/// <summary>	Gets an enumerable over all the headers for this message. </summary>
		///
		/// <value>	An enumerable over all the headers for this message. </value>
		public IEnumerable<KeyValuePair<string, string>> Headers
		{
			get
			{
				return _headers;
			}
		}

		/// <summary>	Gets or sets the HTTP method used for this request message. </summary>
		///
		/// <value>	The HTTP method used for this request message. </value>
		public string Method
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>	Gets or sets the request URL for this request message. </summary>
		///
		/// <value>	The request URL for this request message. </value>
		public Uri Url
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>	Returns a value of an HTTP header. </summary>
		///
		/// <param name="headerName">	The name of the header to get. </param>
		///
		/// <returns>
		/// The value of the HTTP header, or null if no such header was present on the message.
		/// </returns>
		public string GetHeader(string headerName)
		{
			string value;
			if (_headers.TryGetValue(headerName, out value))
			{
				return value;
			}

			return null;
		}

		/// <summary>	Gets the stream backing for this message. </summary>
		///
		/// <returns>	The stream backing for this message. </returns>
		public Stream GetStream()
		{
			return _stream;
		}

		/// <summary>	Sets the value of an HTTP header. </summary>
		///
		/// <param name="headerName"> 	The name of the header to set. </param>
		/// <param name="headerValue">	The value of the HTTP header or 'null' if the header should be
		/// 							removed. </param>
		public void SetHeader(string headerName, string headerValue)
		{
			_headers[headerName] = headerValue;
		}
	}
}