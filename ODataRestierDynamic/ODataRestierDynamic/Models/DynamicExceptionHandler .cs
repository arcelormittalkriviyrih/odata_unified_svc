using ODataRestierDynamic.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace ODataRestierDynamic.Models
{
    /// <summary>	A dynamic exception handler. </summary>
    public class DynamicExceptionHandler : ExceptionHandler
    {
        /// <summary>	The inner invoker. </summary>
        private readonly IExceptionHandler _innerInvoker;

        /// <summary>   Flag for Exception Logging state  </summary>
        private bool _loggingException = false;

        /// <summary>	Constructor. </summary>
        ///
        /// <param name="innerInvoker">	The inner invoker. </param>
        public DynamicExceptionHandler(IExceptionHandler innerInvoker)
        {
            _innerInvoker = innerInvoker;
        }

        /// <summary>	Handles the given context. </summary>
        ///
        /// <param name="context">	The exception handler context. </param>
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is System.Data.SqlClient.SqlException)
            {
                var sqlException = ((System.Data.SqlClient.SqlException)context.Exception);
                if (sqlException.Number >= 50000)
                {
                    var errorObj = new { message = sqlException.Message };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(errorObj);
                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.Conflict)
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
                        ReasonPhrase = sqlException.Number.ToString()
                    };
                    context.Result = new ErrorMessageResult(context.Request, response);
                    return;
                }
            }

            if (_innerInvoker != null)
            {
                _innerInvoker.HandleAsync(context, CancellationToken.None);
            }

            if (context != null && context.Exception != null && !_loggingException)
            {
                try
                {
                    _loggingException = true;
                    DynamicLogger.Instance.WriteLoggerLogError("HandleExceptions", context.Exception);
                }
                finally
                {
                    _loggingException = false;
                }
            }
        }

        /// <summary>	Encapsulates the result of an error message. </summary>
        public class ErrorMessageResult : IHttpActionResult
        {
            private HttpRequestMessage _request;
            private HttpResponseMessage _httpResponseMessage;

            public ErrorMessageResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
            {
                _request = request;
                _httpResponseMessage = httpResponseMessage;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_httpResponseMessage);
            }
        }
    }
}