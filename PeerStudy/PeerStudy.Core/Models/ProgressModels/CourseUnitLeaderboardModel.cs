using System.Collections.Generic;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class CourseUnitLeaderboardModel
    {
        public string CourseUnitTitle { get; set; }

        public List<StudentProgressModel> StudentProgressModels { get; set; }
    }
}
