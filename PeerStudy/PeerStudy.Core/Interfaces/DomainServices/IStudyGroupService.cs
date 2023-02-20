using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudyGroupService
    {
        Task CreateStudyGroupsAsync(Guid teacherId, Guid courseId, int noStudentsPerGroup);
    }
}
