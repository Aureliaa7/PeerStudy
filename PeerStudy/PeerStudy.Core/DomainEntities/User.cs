using PeerStudy.Core.Enums;
using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class User
    {
        public User()
        {
            Answers = new HashSet<Answer>();
            AnswerVotes = new HashSet<AnswerVote>();
            Questions = new HashSet<Question>();
            QuestionVotes = new HashSet<QuestionVote>();
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ProfilePhotoName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Password { get; set; }

        public Role Role { get; set; }


        public ICollection<Answer> Answers { get; set; }

        public ICollection<AnswerVote> AnswerVotes { get; set; }

        public ICollection<Question> Questions { get; set; }

        public ICollection<QuestionVote> QuestionVotes { get; set; }
    }
}
