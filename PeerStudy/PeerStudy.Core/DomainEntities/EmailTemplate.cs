using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class EmailTemplate
    {
        public Guid Id { get; set; }

        public EmailType Type { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
