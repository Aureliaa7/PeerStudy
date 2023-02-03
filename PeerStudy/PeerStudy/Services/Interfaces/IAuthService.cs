using PeerStudy.Core.Models;
using System.Threading.Tasks;

namespace PeerStudy.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginModel loginModel);

        Task LogoutAsync();

        Task<string> GetCurrentUserIdAsync();

        Task<string> GetCurrentUserFullNameAsync();

        Task<string> GetCurrentUserRole();
    }
}
