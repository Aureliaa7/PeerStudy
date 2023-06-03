using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudyGroupCourseUnitStatusModel
    {
        public Guid CourseUnitId { get; set; }

        public string CourseUnitTitle { get; set; }

        public List<StudyGroupAssignmentsModel> StudentAssignmentStatus { get; set;}
        
    }
}
