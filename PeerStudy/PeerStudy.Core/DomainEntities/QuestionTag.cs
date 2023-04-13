using System;

namespace PeerStudy.Core.DomainEntities
{
    public class QuestionTag
    {
        public Guid Id { get; set; }

        public Guid QuestionId { get; set; }

        public Guid TagId { get; set; }

        public Question Question { get; set; }  

        public Tag Tag { get; set; }
    }
}
