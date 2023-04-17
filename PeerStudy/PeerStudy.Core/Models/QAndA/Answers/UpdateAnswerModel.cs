using System;

namespace PeerStudy.Core.Models.QAndA.Answers
{
    public class UpdateAnswerModel
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public string Content { get; set; }
    }
}
