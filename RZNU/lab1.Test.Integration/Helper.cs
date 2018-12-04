using System.Net;
using lab1.Models.Artist;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace lab1.Test.Integration
{
	internal class Helper
	{
		internal static readonly CreateArtist _testArtist = new CreateArtist {Name = "test", YearOfBirth = 10};
		internal static readonly string _serializedArtist = JsonConvert.SerializeObject(_testArtist);

		internal static int CreateTestArtist(Client _client)
		{
			var req = new RestRequest("artist", Method.POST);
			req.AddParameter("application/json", _serializedArtist, ParameterType.RequestBody);
			var resp = _client.RestClient.Execute<ViewArtist>(req);

			Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
			return resp.Data.Id;
		}

		internal static void DeleteTestArtist(int id, Client _client)
		{
			var req = new RestRequest("artist/{id}", Method.DELETE);
			req.AddUrlSegment("id", id.ToString());
			var resp = _client.RestClient.Execute(req);

			Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
		}

		internal static void AddPaintingToArtist(int id, Client _client)
		{
			var req = new RestRequest("painting", Method.POST);
			req.AddParameter();
		}
	}
}