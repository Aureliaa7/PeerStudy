using PeerStudy.Core.Models.CourseUnits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseUnitService
    {
        Task<CourseUnitDetailsModel> CreateAsync(CourseUnitModel courseUnit);

        Task DeleteAsync(Guid id);

        Task RenameAsync(Guid id, string newName);

        Task<List<CourseUnitDetailsModel>> GetByCourseIdAsync(Guid courseId);
    }
}
