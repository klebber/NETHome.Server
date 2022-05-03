using NetHome.Common;
using NetHome.Common.Models;
using NetHome.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IUserService
    {
        public Task<LoginResponse> Login(LoginRequest loginUser);
        public Task Register(RegisterRequest registerUser);
        public Task<UserModel> Validate(string id);
    }
}
