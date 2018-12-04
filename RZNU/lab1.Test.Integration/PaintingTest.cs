using System.Collections.Generic;
using System.Net;
using lab1.Models.Artist;
using lab1.Models.Painting;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace lab1.Test.Integration
{
	public class PaintingTest : IClassFixture<Client>
	{
		public PaintingTest(Client client)
		{
			_client = client;
			_client.Login();
		}

		private readonly Client _client;

		[Fact]
		public void GetAll()
		{
			var req = new RestRequest("painting", Method.GET);
			var resp = _client.RestClient.Execute<List<ViewPainting>>(req);

			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.NotNull(resp.Content);
			Assert.NotEmpty(resp.Content);
		}

		[Fact]
		public void PutExistingId()
		{
			var authorId = Helper.CreateTestArtist(_client);
			var paintId = Helper.AddPaintingToArtist(authorId, _client);
			var newPaint = new CreatePainting {AuthorId = authorId, Name = "changed", YearMade = 200};
			var req = new RestRequest("painting/{id}", Method.PUT);
			req.AddUrlSegment("id", paintId.ToString());
			req.AddParameter("application/json", JsonConvert.SerializeObject(newPaint), ParameterType.RequestBody);

			var putResp = _client.RestClient.Execute(req);
			var getReq = new RestRequest("painting/{id}");
			getReq.AddUrlSegment("id", paintId);
			var getResp = _client.RestClient.Execute<ViewPainting>(getReq);

			Helper.DeleteTestArtist(authorId, _client);
			Assert.Equal(HttpStatusCode.NoContent, putResp.StatusCode);
			Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
			Assert.Equal("changed", getResp.Data.Name);
			Assert.Equal(200, getResp.Data.YearMade);
		}

		[Fact]
		public void PutNewId()
		{
			var authorId = Helper.CreateTestArtist(_client);
			var newPaint = new CreatePainting {AuthorId = authorId, Name = "changed", YearMade = 200};
			var req = new RestRequest("painting/{id}", Method.PUT);
			req.AddUrlSegment("id", "-15");
			req.AddParameter("application/json", JsonConvert.SerializeObject(newPaint), ParameterType.RequestBody);

			var putResp = _client.RestClient.Execute<ViewPainting>(req);

			Helper.DeleteTestArtist(authorId, _client);
			Assert.Equal(HttpStatusCode.Created, putResp.StatusCode);
			Assert.Equal("changed", putResp.Data.Name);
			Assert.Equal(200, putResp.Data.YearMade);
			Assert.Equal(Helper._testArtist.Name, putResp.Data.AuthorName);
		}

		[Fact]
		public void GetAuthor()
		{
			var authorId = Helper.CreateTestArtist(_client);
			var paintId = Helper.AddPaintingToArtist(authorId, _client);
			var req = new RestRequest("painting/{id}/author", Method.GET);
			req.AddUrlSegment("id", paintId.ToString());
			var resp = _client.RestClient.Execute<ViewArtist>(req);

			Helper.DeleteTestArtist(authorId, _client);
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.Equal(Helper._testArtist.Name, resp.Data.Name);
			Assert.Equal(Helper._testArtist.YearOfBirth, resp.Data.YearOfBirth);
		}

		[Fact]
		public void GetById()
		{
			var authorId = Helper.CreateTestArtist(_client);
			var paintId = Helper.AddPaintingToArtist(authorId, _client);
			var req = new RestRequest("painting/{id}", Method.GET);
			req.AddUrlSegment("id", paintId.ToString());
			var resp = _client.RestClient.Execute<ViewPainting>(req);

			Helper.DeleteTestArtist(authorId, _client);
			Assert.NotNull(resp.Data);
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.Equal(Helper._testPainting.Name, resp.Data.Name);
			Assert.Equal(Helper._testPainting.YearMade, resp.Data.YearMade);
		}

		[Fact]
		public void DeletePainting()
		{
			var authorId = Helper.CreateTestArtist(_client);
			var paintId = Helper.AddPaintingToArtist(authorId, _client);

			var deleteReq = new RestRequest("painting/{id}", Method.DELETE);
			deleteReq.AddUrlSegment("id", paintId.ToString());
			var deleteResp = _client.RestClient.Execute(deleteReq);

			var getReq = new RestRequest("painting/{id}", Method.GET);
			getReq.AddUrlSegment("id", paintId.ToString());
			var getResp = _client.RestClient.Execute<ViewPainting>(getReq);

			Helper.DeleteTestArtist(authorId, _client);

			Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
			Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
			Assert.Null(getResp.Data);
		}
	}
}