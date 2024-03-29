﻿using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentBadge
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid BadgeId { get; set; }

        public DateTime EarnedAt { get; set; }

        public StudentBadgeType Type { get; set; }

        public Guid? CourseId { get; set; }

        public Course? Course { get; set; }

        public Student Student { get; set; }

        public Badge Badge { get; set; }
    }
}
