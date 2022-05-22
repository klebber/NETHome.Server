using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IHttpRequestHandler
    {
        HttpResponseMessage Get(Uri uri);
        Task<HttpResponseMessage> GetAsync(Uri uri);
    }
}
