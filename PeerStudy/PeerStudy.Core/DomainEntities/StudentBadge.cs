using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentBadge
    {
        public Guid StudentId { get; set; }

        public Guid BadgeId { get; set; }

        public DateTime RewardingDate { get; set; }


        public Student Student { get; set; }

        public Badge Badge { get; set; }
    }
}
