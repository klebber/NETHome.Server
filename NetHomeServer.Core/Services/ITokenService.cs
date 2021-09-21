using NetHomeServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Core.Services
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}
