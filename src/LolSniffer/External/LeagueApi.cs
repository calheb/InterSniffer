using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LolSniffer
{
    public class LeagueApi
    {
        private readonly string _authToken;

        private readonly int _authPort;

        private HttpClient _client;

        private readonly string _baseUrl;

        public LeagueApi(string authToken, int authPort)
        {
            _authToken = authToken;
            _authPort = authPort;
            _client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (HttpRequestMessage _, X509Certificate2 _, X509Chain _, SslPolicyErrors _) => true
            });
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + authToken)));
            _client.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _baseUrl = $"https://127.0.0.1:{_authPort}";
        }

        public async Task<string?> SendAsync(HttpMethod method, string endpoint, HttpContent? body = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, _baseUrl + endpoint);
            if (body != null)
            {
                httpRequestMessage.Content = body;
            }

            return await (await _client.SendAsync(httpRequestMessage)).Content.ReadAsStringAsync();
        }
    }
}