using System.Net;
using lab1.Models;
using RestSharp;
using Xunit;

namespace lab1.Test.Integration
{
	public class LoginTest : IClassFixture<Client>
	{
		public LoginTest(Client client)
		{
			_client = client;
		}

		private readonly Client _client;

		[Fact]
		public void BadLogin()
		{
			var req = new RestRequest("login", Method.POST);
			req.AddParameter("application/json-patch+json", "{\"username\":\"test\",\"password\":\"wrong\"}",
				ParameterType.RequestBody);
			var resp = _client.RestClient.Execute<User>(req);

			Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
		}
	}
}