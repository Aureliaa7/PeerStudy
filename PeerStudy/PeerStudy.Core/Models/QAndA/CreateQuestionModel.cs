using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.QAndA
{
    public class CreateQuestionModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid AuthorId { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
