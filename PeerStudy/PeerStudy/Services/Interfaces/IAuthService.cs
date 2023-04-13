using PeerStudy.Core.Models.Accounts;
using System.Threading.Tasks;

namespace PeerStudy.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginModel loginModel);

        Task LogoutAsync();

        Task<string> GetCurrentUserIdAsync();

        Task<string> GetCurrentUserRole();

        Task<string> GetCurrentUserEmailAsync();

    }
}
