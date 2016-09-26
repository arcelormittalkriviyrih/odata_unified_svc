﻿using System;
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
				MultipartFormDataContent form = new MultipartFormDataContent();
				form.Add(new StringContent("test.xls"), "FileName");
				form.Add(new StringContent(""), "Encoding");
				form.Add(new StringContent("Test Excel"), "FileType");
				form.Add(new StringContent("Test"), "Status");
				form.Add(new StringContent(Guid.NewGuid().ToString()), "Name");
				form.Add(new ByteArrayContent(fileByteArray, 0, fileByteArray.Length), "Data", "test.xls");
				HttpResponseMessage response = client.PostAsync(testServiceURL + "Files(" + id + ")/$value", form).Result;
				Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
				var result = response.Headers.GetValues("ID").First();
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
	}
}
