using System.Collections.Generic;

namespace PeerStudy.Core.Models.Assignments
{
    public class AssignmentFilesModel : AssignmentDetailsModel
    {
        public List<StudyGroupAssignmentFileModel> StudyGroupAssignmentFiles { get; set; }
    }
}
