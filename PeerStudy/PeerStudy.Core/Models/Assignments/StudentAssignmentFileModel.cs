using PeerStudy.Core.Models.GoogleDriveModels;
using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class StudentAssignmentFileModel : DriveFileDetailsModel
    {
        public Guid Id { get; set; }
    }
}
