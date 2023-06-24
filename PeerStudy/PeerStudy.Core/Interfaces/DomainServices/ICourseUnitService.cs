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

        Task<List<CourseUnitDetailsModel>> GetByCourseAndStudentIdAsync(Guid courseId, Guid studentId);

        Task UnlockAsync(Guid courseUnitId, Guid studentId);

        /// <summary>
        /// Checks if the previous course units are available
        /// </summary>
        /// <param name="courseUnitOrder">The index of the course unit to be unlocked</param>
        /// <param name="studentId">The student id</param>
        /// <param name="courseId">The course id</param>
        /// <returns>A bool indicating whether or not all the previous course units are available</returns>
        Task<bool> CheckPreviousCourseUnitsAvailabilityAsync(int courseUnitOrder, Guid studentId, Guid courseId);
    }
}
