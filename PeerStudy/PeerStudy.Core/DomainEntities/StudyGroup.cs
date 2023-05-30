using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class StudyGroup
    {
        public StudyGroup()
        {
            StudentStudyGroups = new HashSet<StudentStudyGroup>();
            WorkItems = new HashSet<WorkItem>();
            StudyGroupFiles = new HashSet<StudyGroupFile>();
            Assignments = new HashSet<Assignment>();
            PostponedAssignments = new HashSet<PostponedAssignment>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        public string DriveFolderId { get; set; }

        public ICollection<StudentStudyGroup> StudentStudyGroups { get; set; }

        public ICollection<WorkItem> WorkItems { get; set; }

        public ICollection<StudyGroupFile> StudyGroupFiles { get; set; }

        public ICollection<Assignment> Assignments { get; set; }

        public ICollection<PostponedAssignment> PostponedAssignments { get; set; }
    }
}
