using Microsoft.OData.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace ODataRestierDynamic.Models
{
	/// <summary>
	/// Wrapper for Interface for synchronous OData request messages.
	/// </summary>
	internal class ODataMessageWrapper : IODataRequestMessage
	{
		/// <summary>
		/// Stream of params provided by a client in a POST request.
		/// </summary>
		private Stream _stream;
		private Dictionary<string, string> _headers;
		private IDictionary<string, string> _contentIdMapping;

		public ODataMessageWrapper()
			: this(stream: null, headers: null)
		{
		}

		public ODataMessageWrapper(Stream stream)
			: this(stream: stream, headers: null)
		{
		}

		public ODataMessageWrapper(Stream stream, HttpHeaders headers)
			: this(stream: stream, headers: headers, contentIdMapping: null)
		{
		}

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

		public IEnumerable<KeyValuePair<string, string>> Headers
		{
			get
			{
				return _headers;
			}
		}

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

		public string GetHeader(string headerName)
		{
			string value;
			if (_headers.TryGetValue(headerName, out value))
			{
				return value;
			}

			return null;
		}

		public Stream GetStream()
		{
			return _stream;
		}

		public void SetHeader(string headerName, string headerValue)
		{
			_headers[headerName] = headerValue;
		}
	}
}