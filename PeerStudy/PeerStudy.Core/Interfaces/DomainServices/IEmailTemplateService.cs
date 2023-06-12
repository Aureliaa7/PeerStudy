using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplate> GetByTypeAsync(EmailType type);
    }
}
