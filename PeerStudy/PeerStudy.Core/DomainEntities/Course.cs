using PeerStudy.Core.Enums;
using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Course
    {
        public Course()
        {
            CourseEnrollments = new HashSet<StudentCourse>();
            CourseUnits = new HashSet<CourseUnit>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public int NoStudents { get; set; }

        public Guid TeacherId { get; set; }

        public CourseStatus Status { get; set; }

        public string DriveRootFolderId { get; set; }

        public string ResourcesDriveFolderId { get; set; }

        public string AssignmentsDriveFolderId { get; set; }

        public string StudyGroupsDriveFolderId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool HasStudyGroups { get; set; }

        public Teacher Teacher { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }
        public ICollection<CourseUnit> CourseUnits { get; set; }
    }
}
