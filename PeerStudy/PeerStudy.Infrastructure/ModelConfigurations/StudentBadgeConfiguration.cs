using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class StudentBadgeConfiguration : IEntityTypeConfiguration<StudentBadge>
    {
        public void Configure(EntityTypeBuilder<StudentBadge> builder)
        {
            builder.HasOne(x => x.Student)
                .WithMany(x => x.StudentBadges)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Badge)
                .WithMany(x => x.StudentBadges)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
