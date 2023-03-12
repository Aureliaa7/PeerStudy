using PeerStudy.Core.Models;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAccountService
    {
        Task<string> LoginAsync(LoginModel loginModel);

        Task RegisterAsync(RegisterModel registerModel);

        Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel);
    }
}
