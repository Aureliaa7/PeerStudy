using System.Collections.Generic;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudyGroupLeaderboardModel
    {
        public string StudyGroupName { get; set; }

        public List<StudentProgressModel> StudentsProgress { get; set; }
    }
}
