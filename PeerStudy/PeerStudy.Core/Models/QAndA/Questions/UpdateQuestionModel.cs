using System;

namespace PeerStudy.Core.Models.QAndA.Questions
{
    public class UpdateQuestionModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
