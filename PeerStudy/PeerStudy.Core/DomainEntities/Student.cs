﻿using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Student : User
    {
        public Student()
        {
            CourseEnrollments = new HashSet<StudentCourse>();
            StudentStudyGroups = new HashSet<StudentStudyGroup>();
            Assignments = new HashSet<StudentAssignment>();
            WorkItems = new HashSet<WorkItem>();
            StudyGroupFiles = new HashSet<StudyGroupFile>();
            UnlockedCourseUnits = new HashSet<UnlockedCourseUnit>();
            StudentBadges = new HashSet<StudentBadge>();
            PostponedAssignments = new HashSet<PostponedAssignment>();
        }

        public int NoTotalPoints { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }

        public ICollection<StudentStudyGroup> StudentStudyGroups { get; set; }

        public ICollection<StudentAssignment> Assignments { get; set; }

        public ICollection<WorkItem> WorkItems { get; set; }

        public ICollection<StudyGroupFile> StudyGroupFiles { get; set; }

        public ICollection<UnlockedCourseUnit> UnlockedCourseUnits { get; set; }

        public ICollection<StudentBadge> StudentBadges { get; set; }

        public ICollection<PostponedAssignment> PostponedAssignments { get; set; }
    }
}
