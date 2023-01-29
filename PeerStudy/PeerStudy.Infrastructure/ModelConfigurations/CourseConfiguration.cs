﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(60);

            builder.Property(x => x.NoStudents).HasDefaultValue(default(int));
        }
    }
}
