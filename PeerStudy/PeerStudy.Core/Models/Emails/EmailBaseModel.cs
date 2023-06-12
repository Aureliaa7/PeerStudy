using PeerStudy.Core.Enums;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.Emails
{
    public class EmailBaseModel
    {
        public string RecipientName { get; set; }

        public List<string> To { get; set; }

        public EmailType EmailType { get; set; }
    }
}
