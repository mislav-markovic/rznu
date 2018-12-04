using System.Linq;
using System.Net;
using lab1.Models;
using RestSharp;
using Xunit;

namespace lab1.Test.Integration
{
	public class Client
	{
		public Client()
		{
			CookieContainer _cookieJar = new CookieContainer();
			RestClient = new RestClient("https://localhost:5001/api");
			RestClient.CookieContainer = _cookieJar;
			ServicePointManager.ServerCertificateValidationCallback +=
				(sender, certificate, chain, sslPolicyErrors) => true;
		}

		public RestClient RestClient { get; set; }

		public void Login()
		{
			var req = new RestRequest("login", Method.POST);
			req.AddParameter("application/json-patch+json", "{\"username\":\"test\",\"password\":\"test\"}", ParameterType.RequestBody);
			var resp = RestClient.Execute<User>(req);

			Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
			Assert.Equal("test", resp.Data.Username);
			Assert.Null(resp.Data.Password);
		}
	}
}