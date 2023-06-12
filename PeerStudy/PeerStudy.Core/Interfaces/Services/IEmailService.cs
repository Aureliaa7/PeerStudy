using PeerStudy.Core.Models.Emails;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailBaseModel emailModel);
    }
}
