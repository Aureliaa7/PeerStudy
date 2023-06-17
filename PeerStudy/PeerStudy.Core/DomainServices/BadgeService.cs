using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class BadgeService : IBadgeService
    {
        private readonly IUnitOfWork unitOfWork;

        public BadgeService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Badge> GetByTypeAsync(BadgeType type)
        {
            var badge = await unitOfWork.BadgesRepository.GetFirstOrDefaultAsync(x => x.Type == type)
               ?? throw new EntityNotFoundException($"Badge {type.ToString()} was not found!");

            return badge;
        }
    }
}
