using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public class HttpRequestHandler : IHttpRequestHandler
    {
        private readonly HttpClient _httpClient;

        public HttpRequestHandler()
        {
            _httpClient = new HttpClient();
        }

        public HttpResponseMessage Get(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return _httpClient.SendAsync(request).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await _httpClient.SendAsync(request);
        }
    }
}
