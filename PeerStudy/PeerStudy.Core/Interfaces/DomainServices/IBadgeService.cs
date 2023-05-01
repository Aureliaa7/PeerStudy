using PeerStudy.Core.Enums;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IBadgeService
    {
        Task AddAsync(Guid studentId, BadgeType badgeType);
    }
}
