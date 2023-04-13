using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.QAndA
{
    public class QuestionModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public Guid AuthorId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
