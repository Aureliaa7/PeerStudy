using PeerStudy.Core.Models.Users;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAchievementService
    {
        Task<StudentProfileModel> GetProgressByStudentIdAsync(Guid studentId);
    }
}
