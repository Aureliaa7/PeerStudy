using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.ProgressModels;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAchievementService
    {
        Task<StudentProfileModel> GetProgressByStudentIdAsync(Guid studentId, CourseStatus courseStatus);

        /// <summary>
        /// Returns leaderboard data for each study group(we'll have a leaderboard for each study group)
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        Task<List<StudyGroupLeaderboardModel>> GetLeaderboardDataForStudyGroupsAsync(Guid courseId, Guid teacherId);

        /// <summary>
        /// Returns data to create a leaderboard per course
        /// </summary>
        /// <returns></returns>
        Task<List<StudentProgressModel>> GetLeaderboardDataByCourseAsync(Guid courseId, Guid teacherId);

        /// <summary>
        /// Returns data to create course units leaderboards
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        Task<List<CourseUnitLeaderboardModel>> GetCourseUnitsLeaderboardDataAsync(Guid courseId, Guid teacherId);

        /// <summary>
        /// Returns the progress of a student for a given course id
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="studentId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        Task<ExtendedStudentCourseProgressModel> GetProgressByCourseAndStudentAsync(Guid courseId, Guid studentId, Guid teacherId);

        Task<StudyGroupStatisticsDataModel> GetStatisticsDataByGroupAsync(Guid studyGroupId, Guid courseId);
    }
}
