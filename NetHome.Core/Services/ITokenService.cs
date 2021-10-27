using NetHome.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}
