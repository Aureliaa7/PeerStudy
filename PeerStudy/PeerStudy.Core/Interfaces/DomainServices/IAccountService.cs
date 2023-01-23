using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Models;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAccountService
    {
        Task<string> LoginAsync(LoginModel loginModel);

        Task<User> RegisterAsync(RegisterModel registerModel);
    }
}
