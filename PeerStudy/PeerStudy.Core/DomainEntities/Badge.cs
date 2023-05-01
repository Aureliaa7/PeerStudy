using PeerStudy.Core.Enums;
using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Badge
    {
        public Badge()
        {
            StudentBadges = new HashSet<StudentBadge>();
        }

        public Guid Id { get; set; }

        public BadgeType Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Base64Content { get; set; }

        public int Points { get; set; }

        public ICollection<StudentBadge> StudentBadges { get; set; }
    }
}
