using NetHomeServer.Common.Models;
using NetHomeServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Core.Services
{
    public interface IUserService
    {
        public Task<LoginResponseModel> Login(LoginModel loginUser);
        public Task Register(RegisterModel registerUser);
        public Task<UserModel> Validate(string id);
    }
}
