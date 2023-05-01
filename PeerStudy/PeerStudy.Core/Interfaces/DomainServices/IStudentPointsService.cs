using PeerStudy.Core.Models.StudentAssets;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudentPointsService
    {
        Task<int> GetNoPointsAsync(Guid studentId);

        Task AddAsync(SaveStudentPointsModel saveStudentPointsModel);

        Task SubtractPointsAsync(Guid studentId, int noPoints, bool saveChanges = true);
    }
}
