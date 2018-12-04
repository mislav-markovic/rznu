using System.Collections.Generic;
using System.Net;
using lab1.Models.Artist;
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
			var id = CreateTestArtist();
			var req = new RestRequest("artist/{id}", Method.GET);
			req.AddUrlSegment("id", id.ToString());
			var resp = _client.RestClient.Execute<ViewArtist>(req);

			DeleteTestArtist(id);
			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.Equal("test", resp.Data.Name);
			Assert.Equal(10, resp.Data.YearOfBirth);
		}

		[Fact]
		public void PutExistingId()
		{
			var id = CreateTestArtist();
			var req = new RestRequest("artist/{id}", Method.PUT);
			req.AddUrlSegment("id", id.ToString());

			req.AddParameter("application/json",
				JsonConvert.SerializeObject(new CreateArtist {Name = "test2", YearOfBirth = 20}),
				ParameterType.RequestBody);
			var putResp = _client.RestClient.Execute(req);

			var getReq = new RestRequest("artist/{id}", Method.GET);
			getReq.AddUrlSegment("id", id.ToString());
			var resp = _client.RestClient.Execute<ViewArtist>(getReq, Method.GET);

			DeleteTestArtist(id);
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
			req.AddParameter("application/json", _serializedArtist, ParameterType.RequestBody);
			var putResp = _client.RestClient.Execute<ViewArtist>(req);

			DeleteTestArtist(putResp.Data.Id);
			Assert.Equal(HttpStatusCode.Created, putResp.StatusCode);
			Assert.Equal("test", putResp.Data.Name);
			Assert.Equal(10, putResp.Data.YearOfBirth);
		}

		public void GetArtistWorks()
		{
			var id
		}
	}
}