using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IUnitOfWork unitOfWork;

        public EmailTemplateService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task<EmailTemplate> GetByTypeAsync(EmailType type)
        {
            var emailTemplate = unitOfWork.EmailTemplatesRepository.GetFirstOrDefaultAsync(
                x => x.Type == type) ?? throw new EntityNotFoundException($"Email template with type {type} was not found!");

            return emailTemplate;
        }
    }
}
