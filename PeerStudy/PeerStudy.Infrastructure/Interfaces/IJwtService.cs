using PeerStudy.Core.Models.Accounts;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(LoginModel loginModel);
    }
}
