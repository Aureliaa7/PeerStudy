using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Tag
    {
        public Tag()
        {
            QuestionTags = new HashSet<QuestionTag>();
        }

        public Guid Id { get; set; }

        public string Content { get; set; }

        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
