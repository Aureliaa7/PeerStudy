using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.CourseUnits;
using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.Courses
{
    public class StudentCourseProgressModel
    {
        public Guid CourseId { get; set; }

        public string CourseTitle { get; set; }

        public string TeacherName { get; set; }

        public List<UnlockedCourseUnitModel> UnlockedCourseUnits { get; set; }  
        
        public List<StudentCourseUnitAssignmentsModel> CourseUnitsAssignmentsProgress { get; set; }
    }
}
