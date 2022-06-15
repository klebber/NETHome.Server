using System.Threading.Tasks;
using NetHome.Common;

namespace NetHome.Core.Services
{
    public interface IUserService
    {
        Task<LoginResponse> Login(LoginRequest loginUser);
        Task Register(RegisterRequest registerUser);
        Task<UserModel> Validate(string userId);
        Task<UserModel> UpdateUser(UserModel userModel, string senderId);
        Task DeleteUser(string userId);
        Task GiveUserDeviceAccess(DeviceAccessPayload deviceAccess);
        Task RemoveUserDeviceAccess(DeviceAccessPayload deviceAccess);
        Task ChangePassword(PasswordChangePayload payload, string senderId);
        Task LockUser(UserModel userModel);
        Task UnlockUser(UserModel userModel);
    }
}
