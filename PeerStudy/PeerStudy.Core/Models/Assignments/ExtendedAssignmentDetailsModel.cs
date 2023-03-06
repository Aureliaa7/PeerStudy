using System.Collections.Generic;

namespace PeerStudy.Core.Models.Assignments
{
    public class ExtendedAssignmentDetailsModel : AssignmentDetailsModel
    {
        public List<GradeAssignmentModel> Students { get; set; }
    }
}
