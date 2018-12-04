using System.Collections.Generic;
using System.Net;
using lab1.Models.Artist;
using lab1.Models.Painting;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace lab1.Test.Integration
{
	public class ArtistTest : IClassFixture<Client>
	{
		public ArtistTest(Client client)
		{
			_client = client;
			_client.Login();
		}

		private readonly Client _client;



		//get all
		[Fact]
		public void GetAllTest()
		{
			var req = new RestRequest("artist", Method.GET);
			var resp = _client.RestClient.Execute<List<ViewArtist>>(req);

			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.NotNull(resp.Content);
			Assert.NotEmpty(resp.Content);
		}

		//get id, create new, delete
		[Fact]
		public void GetById()
		{
			var id = Helper.CreateTestArtist(_client);
			var req = new RestRequest("artist/{id}", Method.GET);
			req.AddUrlSegment("id", id.ToString());
			var resp = _client.RestClient.Execute<ViewArtist>(req);

			Helper.DeleteTestArtist(id, _client);
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.Equal("test", resp.Data.Name);
			Assert.Equal(10, resp.Data.YearOfBirth);
		}

		[Fact]
		public void PutExistingId()
		{
			var id = Helper.CreateTestArtist(_client);
			var req = new RestRequest("artist/{id}", Method.PUT);
			req.AddUrlSegment("id", id.ToString());

			req.AddParameter("application/json",
				JsonConvert.SerializeObject(new CreateArtist {Name = "test2", YearOfBirth = 20}),
				ParameterType.RequestBody);
			var putResp = _client.RestClient.Execute(req);

			var getReq = new RestRequest("artist/{id}", Method.GET);
			getReq.AddUrlSegment("id", id.ToString());
			var resp = _client.RestClient.Execute<ViewArtist>(getReq, Method.GET);

			Helper.DeleteTestArtist(id,_client);
			Assert.Equal(HttpStatusCode.NoContent, putResp.StatusCode);
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.Equal("test2", resp.Data.Name);
			Assert.Equal(20, resp.Data.YearOfBirth);
		}

		[Fact]
		public void PutNewId()
		{
			var req = new RestRequest("artist/{id}", Method.PUT);
			req.AddUrlSegment("id", "-15");
			req.AddParameter("application/json", Helper._serializedArtist, ParameterType.RequestBody);
			var putResp = _client.RestClient.Execute<ViewArtist>(req);

			Helper.DeleteTestArtist(putResp.Data.Id, _client);
			Assert.Equal(HttpStatusCode.Created, putResp.StatusCode);
			Assert.Equal("test", putResp.Data.Name);
			Assert.Equal(10, putResp.Data.YearOfBirth);
		}

		[Fact]
		public void GetArtistWorks()
		{
			var id = Helper.CreateTestArtist(_client);
			Helper.AddPaintingToArtist(id, _client);

			var req = new RestRequest("artist/{id}/paintings");
			req.AddUrlSegment("id", id.ToString());
			var resp = _client.RestClient.Execute<List<ViewPainting>>(req);

			Helper.DeleteTestArtist(id, _client);
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.NotNull(resp.Data);
			Assert.NotEmpty(resp.Data);
		}

		[Fact]
		public void DeletingArtistDeletesWorks()
		{
			var authId = Helper.CreateTestArtist(_client);
			var paintId = Helper.AddPaintingToArtist(authId, _client);

			var worksGetReq = new RestRequest("artist/{id}/paintings");
			worksGetReq.AddUrlSegment("id", authId.ToString());
			var resp = _client.RestClient.Execute<List<ViewPainting>>(worksGetReq);

			Helper.DeleteTestArtist(authId, _client);

			var paintGetReq = new RestRequest("painting/{id}");
			paintGetReq.AddUrlSegment("id", paintId.ToString());
			var paintGetResp = _client.RestClient.Execute<ViewPainting>(paintGetReq);

			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.NotNull(resp.Data);
			Assert.NotEmpty(resp.Data);
			Assert.Equal(HttpStatusCode.NotFound, paintGetResp.StatusCode);
		}
	}
}