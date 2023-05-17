using System.Collections.Generic;

namespace PeerStudy.Core.Models.Assignments
{
    public class StudentCourseUnitAssignmentsModel
    {
        public string CourseUnitTitle { get; set; }

        public List<StudentAssignmentDetailsModel> StudentAssignments { get; set;}
    }
}
