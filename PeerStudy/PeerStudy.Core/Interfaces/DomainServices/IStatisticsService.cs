using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStatisticsService
    {
        Task<StudyGroupStatisticsDataModel> GetStatisticsDataByGroupAsync(Guid studyGroupId, Guid courseId);
    }
}
