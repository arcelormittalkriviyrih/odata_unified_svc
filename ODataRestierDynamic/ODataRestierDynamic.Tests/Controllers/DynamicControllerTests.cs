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
namespace ODataRestierDynamic.Controllers.Tests
{
	[TestClass()]
	public class DynamicControllerTests
	{
		private HttpClient client;

		public DynamicControllerTests()
		{
			var configuration = new HttpConfiguration();
			configuration.MapRestierRoute<DynamicApi>("DynamicApi", "DynamicApi").Wait();
			client = new HttpClient(new HttpServer(configuration));
		}

		[TestMethod()]
		public async Task GetUserMetadataTest()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/$metadata");
			request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/xml"));
			var response = await client.SendAsync(request);
			var result = await response.Content.ReadAsStringAsync();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.IsTrue(result.Contains("v_TestView"));
		}

		[TestMethod()]
		public async Task GetTest()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/TestParent");
			request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
			HttpResponseMessage response = await client.SendAsync(request);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod()]
		public async Task GetViewTest()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/v_TestView");
			request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
			HttpResponseMessage response = await client.SendAsync(request);
			var result = await response.Content.ReadAsStringAsync();
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod()]
		public async Task GetNonExistingEntityTest()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/TestParent(-1)");
			request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
			HttpResponseMessage response = await client.SendAsync(request);
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		}

		[TestMethod()]
		public async Task PostDeleteTest()
		{
			var anonymousTestParent = new { ID = 0, f_nvarchar = "test_f_nvarchar", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_bit = true, f_date = "2016-05-30", f_real = 1.3 };
			string payload = JsonConvert.SerializeObject(anonymousTestParent);
			var requestPost = new HttpRequestMessage(HttpMethod.Post, "http://host/DynamicApi/TestParent")
			{
				Content = new StringContent(payload, Encoding.UTF8, "application/json")
			};

			HttpResponseMessage responsePost = await client.SendAsync(requestPost);
			var resultPost = await responsePost.Content.ReadAsStringAsync();
			anonymousTestParent = JsonConvert.DeserializeAnonymousType(resultPost, anonymousTestParent);
			Assert.AreEqual(HttpStatusCode.Created, responsePost.StatusCode);

			var requestDelete = new HttpRequestMessage(HttpMethod.Delete, "http://host/DynamicApi/TestParent(" + anonymousTestParent.ID + ")");
			requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
			HttpResponseMessage responseDelete = await client.SendAsync(requestDelete);
			Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
		}

		[TestMethod()]
		public async Task PostGetNavigationPropertyDeleteTest()
		{
			//Post Parent
			var anonymousTestParent = new { ID = 0, f_nvarchar = "test_f_nvarchar", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_bit = true, f_date = "2016-05-30", f_real = 1.3 };
			{
				string payload = JsonConvert.SerializeObject(anonymousTestParent);
				var requestPost = new HttpRequestMessage(HttpMethod.Post, "http://host/DynamicApi/TestParent")
				{
					Content = new StringContent(payload, Encoding.UTF8, "application/json")
				};
				HttpResponseMessage responsePost = await client.SendAsync(requestPost);
				var resultPost = await responsePost.Content.ReadAsStringAsync();
				anonymousTestParent = JsonConvert.DeserializeAnonymousType(resultPost, anonymousTestParent);
				Assert.AreEqual(HttpStatusCode.Created, responsePost.StatusCode);
			}

			//Post Child
			var anonymousTestChild = new { ID = 0, f_nvarchar = "test_f_nvarchar", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_bit = true, f_date = "2016-05-30", f_real = 1.3, TestParentID = anonymousTestParent.ID };
			{
				string payload = JsonConvert.SerializeObject(anonymousTestChild);
				var requestPost = new HttpRequestMessage(HttpMethod.Post, "http://host/DynamicApi/TestChild")
				{
					Content = new StringContent(payload, Encoding.UTF8, "application/json")
				};
				HttpResponseMessage responsePost = await client.SendAsync(requestPost);
				var resultPost = await responsePost.Content.ReadAsStringAsync();
				anonymousTestChild = JsonConvert.DeserializeAnonymousType(resultPost, anonymousTestChild);
				Assert.AreEqual(HttpStatusCode.Created, responsePost.StatusCode);
			}

			//Get Parent
			{
				var requestGet = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/TestParent(" + anonymousTestParent.ID + ")");
				requestGet.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
				HttpResponseMessage response = await client.SendAsync(requestGet);
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			}

			//Get Child
			{
				var requestGet = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/TestChild(" + anonymousTestChild.ID + ")");
				requestGet.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;odata.metadata=full"));
				HttpResponseMessage response = await client.SendAsync(requestGet);
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			}

			//Get Navigation Property
			{
				var requestGet = new HttpRequestMessage(HttpMethod.Get, "http://host/DynamicApi/TestChild(" + anonymousTestChild.ID + ")/TestParent");
				requestGet.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
				HttpResponseMessage responseGet = await client.SendAsync(requestGet);
				Assert.AreEqual(HttpStatusCode.OK, responseGet.StatusCode);
			}

			//Delete Child
			{
				var requestDelete = new HttpRequestMessage(HttpMethod.Delete, "http://host/DynamicApi/TestChild(" + anonymousTestChild.ID + ")");
				requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
				HttpResponseMessage responseDelete = await client.SendAsync(requestDelete);
				Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
			}

			//Delete Parent
			{
				var requestDelete = new HttpRequestMessage(HttpMethod.Delete, "http://host/DynamicApi/TestParent(" + anonymousTestParent.ID + ")");
				requestDelete.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
				HttpResponseMessage responseDelete = await client.SendAsync(requestDelete);
				Assert.AreEqual(HttpStatusCode.NoContent, responseDelete.StatusCode);
			}
		}

		[TestMethod()]
		public async Task PostActionImportShouldReturnNotFound()
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "http://host/DynamicApi/KLHKLJHFKLJKLEJK");
			request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
			HttpResponseMessage response = await client.PostAsync("http://host/DynamicApi/KLHKLJHFKLJKLEJK", request.Content);
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		}

		[TestMethod()]
		public async Task PostActionImportShouldReturnOK()
		{
			var anonymousTestParent = new { f_nvarchar = "PostActionImportShouldReturnOK", f_datetimeoffset = "1970-01-01T02:00:00.000Z", f_date = "2016-05-30", f_real = 1.3, f_bit = true };
			string payload = JsonConvert.SerializeObject(anonymousTestParent);
			HttpResponseMessage response = await client.PostAsync("http://host/DynamicApi/ins_TestParent", new StringContent(payload, Encoding.UTF8, "application/json"));
			Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
		}
	}
}
