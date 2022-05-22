using System.Threading.Tasks;
using NetHome.Common;
using NetHome.Common.Models;

namespace NetHome.Core.Services
{
    public interface IUserService
    {
        Task<LoginResponse> Login(LoginRequest loginUser);
        Task Register(RegisterRequest registerUser);
        Task<UserModel> Validate(string id);
    }
}
