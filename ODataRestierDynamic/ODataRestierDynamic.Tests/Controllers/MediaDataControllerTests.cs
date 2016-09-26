using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODataRestierDynamic.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Restier.WebApi;
using ODataRestierDynamic.Models;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Configuration;
using System.Collections.Specialized;

namespace ODataRestierDynamic.Controllers.Tests
{
    [TestClass()]
    public class MediaDataControllerTests
    {
        private HttpClient client;

        private string testServiceURL = null;

        public void FixEfProviderServicesProblem()
        {
            //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
            //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
            //Make sure the provider assembly is available to the running application. 
            //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.

            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public MediaDataControllerTests()
        {
            if (ConfigurationManager.AppSettings["TestServiceURL"] != null)
            {
                testServiceURL = ConfigurationManager.AppSettings["TestServiceURL"];
            }

            if (string.IsNullOrEmpty(testServiceURL))
            {
                testServiceURL = DynamicControllerTests.cDefaultBaseURL;
                var configuration = new HttpConfiguration();
                Task<System.Web.OData.Routing.ODataRoute> odataRoute = configuration.MapRestierRoute<DynamicApi>("DynamicApi", "DynamicApi");
                odataRoute.Wait();
                odataRoute.Result.PathRouteConstraint.RoutingConventions.Add(new ODataRestierDynamic.DynamicFactory.MediadataRoutingConvention());

                // Register an Action selector that can include template parameters in the name
                IHttpActionSelector actionSelectorService = configuration.Services.GetActionSelector();
                configuration.Services.Replace(typeof(IHttpActionSelector), new DynamicODataActionSelector(actionSelectorService));
                // Register an Action invoker that can include template parameters in the name
                IHttpActionInvoker actionInvokerService = configuration.Services.GetActionInvoker();
                configuration.Services.Replace(typeof(IHttpActionInvoker), new DynamicODataActionInvoker(actionInvokerService));

                client = new HttpClient(new HttpServer(configuration));
            }
            else
            {
                var handler = new HttpClientHandler { UseDefaultCredentials = true };
                client = new HttpClient(handler);
                
            }

            if (!testServiceURL.EndsWith("/"))
            {
                testServiceURL += "/";
            }
        }

        [TestMethod()]
        public void PostGetDeleteMediaResourceTest()
        {
            int id = -1;
            var fileByteArray = ODataRestierDynamic.Tests.Properties.Resources.test;
            //Post Media Resource
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("FileName", "test.xls");
                nvc.Add("Encoding", "");
                nvc.Add("FileType", "Test Excel");
                nvc.Add("Status", "Test");
                nvc.Add("Name", Guid.NewGuid().ToString());
                var result = UploadFilesToRemoteUrl(testServiceURL + "Files(" + id + ")/$value", fileByteArray, nvc);
                id = int.Parse(result);
            }

            //Get Media Resource
            {
                var request = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "Files(" + id + ")/$value");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
                HttpResponseMessage response = client.SendAsync(request).Result;
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var result = response.Content.ReadAsByteArrayAsync().Result;
                Assert.IsTrue(result.SequenceEqual(fileByteArray));
            }

            //Delete Media Resource
            {
                var requestDelete = new HttpRequestMessage(HttpMethod.Delete, testServiceURL + "Files(" + id + ")");
                requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage responseDelete = client.SendAsync(requestDelete).Result;
                Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
            }
        }

        public static string UploadFilesToRemoteUrl(string url, byte[] file, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.ContentType = "multipart/form-data; boundary=" +
                                    boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                    boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "--");


            string formdataTemplate = "\r\n--" + boundary +
                                        "\r\nContent-Type: text/plain; charset=utf-8" +
                                        "\r\nContent-Disposition: form-data; name={0}\r\n\r\n{1}";

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            string headerTemplate =
                "Content-Disposition: form-data; name={0}; filename={1}\r\n\r\n";

            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            var header = string.Format(headerTemplate, "Data", "test.xls");
            var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

            memStream.Write(headerbytes, 0, headerbytes.Length);
            memStream.Write(file, 0, file.Length);

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
                var result = response.Headers.GetValues("ID").First();
                return result;
            }
        }
    }
}
