using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using ODataRestierDynamic.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Restier.WebApi;
using ODataRestierDynamic.Models;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;

namespace ODataRestierDynamic.Controllers.Tests
{
    [TestClass()]
    public class DynamicControllerTests
    {
        /// <summary>	The database command timeout in seconds. </summary>
        internal static readonly string cDefaultBaseURL = "http://host/DynamicApi/";

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

        public DynamicControllerTests()
        {
            if (ConfigurationManager.AppSettings["TestServiceURL"] != null)
            {
                testServiceURL = ConfigurationManager.AppSettings["TestServiceURL"];
            }

            if (string.IsNullOrEmpty(testServiceURL))
            {
                testServiceURL = cDefaultBaseURL;
                var configuration = new HttpConfiguration();
                configuration.MapRestierRoute<DynamicApi>("DynamicApi", "DynamicApi").Wait();
                client = new HttpClient(new HttpServer(configuration));
            }
            else
            {
                var handler = new HttpClientHandler { UseDefaultCredentials = true };
                client = new HttpClient(handler);
            }

            if (client != null)
                client.Timeout = TimeSpan.FromMinutes(10);

            if (!testServiceURL.EndsWith("/"))
            {
                testServiceURL += "/";
            }
        }

        [TestMethod()]
        public async Task GetUserMetadataTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "$metadata");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/xml"));
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(result.Contains("v_TestView"));
        }

        [TestMethod()]
        public async Task GetTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "TestParent");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod()]
        public async Task GetViewTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "v_TestView");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
            HttpResponseMessage response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod()]
        public async Task GetNonExistingEntityTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "TestParent(-1)");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod()]
        public void PostDeleteTest()
        {
            var anonymousTestParent = new { ID = 0, f_nvarchar = "test_f_nvarchar", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_bit = true, f_date = "2016-05-30", f_real = 1.3 };

            var url = new Uri(testServiceURL + "TestParent");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string payload = JsonConvert.SerializeObject(anonymousTestParent);
            byte[] body = Encoding.UTF8.GetBytes(payload);
            request.Method = "POST";
            request.ContentLength = body.Length;
            request.ContentType = "application/json";
            request.UseDefaultCredentials = true;

            using (System.IO.Stream stream = request.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
                stream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    anonymousTestParent = JsonConvert.DeserializeAnonymousType(responseText, anonymousTestParent);
                }

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                response.Close();
            }

            var requestDelete = new HttpRequestMessage(HttpMethod.Delete, testServiceURL + "TestParent(" + anonymousTestParent.ID + ")");
            requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            HttpResponseMessage responseDelete = client.SendAsync(requestDelete).Result;
            Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
        }

        [TestMethod()]
        public async Task PostGetNavigationPropertyDeleteTest()
        {
            //Post Parent
            var anonymousTestParent = new { ID = 0, f_nvarchar = "test_f_nvarchar", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_bit = true, f_date = "2016-05-30", f_real = 1.3 };
            {
                var url = new Uri(testServiceURL + "TestParent");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                string payload = JsonConvert.SerializeObject(anonymousTestParent);
                byte[] body = Encoding.UTF8.GetBytes(payload);
                request.Method = "POST";
                request.ContentLength = body.Length;
                request.ContentType = "application/json";
                request.UseDefaultCredentials = true;

                using (System.IO.Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = ASCIIEncoding.ASCII;
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                    {
                        string responseText = reader.ReadToEnd();
                        anonymousTestParent = JsonConvert.DeserializeAnonymousType(responseText, anonymousTestParent);
                    }

                    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                    response.Close();
                }
            }

            //Post Child
            var anonymousTestChild = new { ID = 0, f_nvarchar = "test_f_nvarchar", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_bit = true, f_date = "2016-05-30", f_real = 1.3, TestParentID = anonymousTestParent.ID };
            {
                var url = new Uri(testServiceURL + "TestChild");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                string payload = JsonConvert.SerializeObject(anonymousTestChild);
                byte[] body = Encoding.UTF8.GetBytes(payload);
                request.Method = "POST";
                request.ContentLength = body.Length;
                request.ContentType = "application/json";
                request.UseDefaultCredentials = true;

                using (System.IO.Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = ASCIIEncoding.ASCII;
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                    {
                        string responseText = reader.ReadToEnd();
                        anonymousTestChild = JsonConvert.DeserializeAnonymousType(responseText, anonymousTestChild);
                    }

                    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                    response.Close();
                }
            }

            //Get Parent
            {
                var requestGet = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "TestParent(" + anonymousTestParent.ID + ")");
                requestGet.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
                HttpResponseMessage response = await client.SendAsync(requestGet);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            //Get Child
            {
                var requestGet = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "TestChild(" + anonymousTestChild.ID + ")");
                requestGet.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
                HttpResponseMessage response = await client.SendAsync(requestGet);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            //Get Navigation Property
            {
                var requestGet = new HttpRequestMessage(HttpMethod.Get, testServiceURL + "TestChild(" + anonymousTestChild.ID + ")/TestParent");
                requestGet.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage responseGet = await client.SendAsync(requestGet);
                Assert.AreEqual(HttpStatusCode.OK, responseGet.StatusCode);
            }

            //Delete Child
            {
                var requestDelete = new HttpRequestMessage(HttpMethod.Delete, testServiceURL + "TestChild(" + anonymousTestChild.ID + ")");
                requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage responseDelete = await client.SendAsync(requestDelete);
                Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
            }

            //Delete Parent
            {
                var requestDelete = new HttpRequestMessage(HttpMethod.Delete, testServiceURL + "TestParent(" + anonymousTestParent.ID + ")");
                requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage responseDelete = await client.SendAsync(requestDelete);
                Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
            }
        }

        [TestMethod()]
        public async Task PostActionImportShouldReturnNotFound()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, testServiceURL + "KLHKLJHFKLJKLEJK");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            HttpResponseMessage response = await client.PostAsync(testServiceURL + "KLHKLJHFKLJKLEJK", request.Content);
            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestMethod()]
        public void PostActionImportShouldReturnOK()
        {
            var url = new Uri(testServiceURL + "ins_TestParent");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var anonymousTestParent = new { f_nvarchar = "PostActionImportShouldReturnOK", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_date = "2016-05-30", f_real = 1.3, f_bit = true };
            string payload = JsonConvert.SerializeObject(anonymousTestParent);
            byte[] body = Encoding.UTF8.GetBytes(payload);
            request.Method = "POST";
            request.ContentLength = body.Length;
            request.ContentType = "application/json";
            request.UseDefaultCredentials = true;

            using (System.IO.Stream stream = request.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
                stream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                }

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                response.Close();
            }
        }
    }
}
