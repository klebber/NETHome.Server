using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IHttpRequestHandler
    {
        HttpResponseMessage Get(Uri uri);
        Task<HttpResponseMessage> GetAsync(Uri uri);
    }
}
