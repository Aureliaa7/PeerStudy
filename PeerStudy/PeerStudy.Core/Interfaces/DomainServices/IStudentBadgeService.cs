using PeerStudy.Core.Enums;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudentBadgeService
    {
        Task AddAsync(Guid studentId, BadgeType badgeType, StudentBadgeType studentBadgeType, Guid? courseId = null);
    }
}
