using PeerStudy.Core.Models.GoogleDriveModels;
using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class StudyGroupAssignmentFileModel : DriveFileDetailsModel
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; }
    }
}
