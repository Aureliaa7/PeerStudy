using System;

namespace PeerStudy.Core.Models.Emails
{
    public class NewAssignmentEmailModel : AssignmentBaseModel
    {
        public DateTime Deadline { get; set; }
    }
}
