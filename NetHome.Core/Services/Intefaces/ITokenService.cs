using NetHome.Data.Entities;

namespace NetHome.Core.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
