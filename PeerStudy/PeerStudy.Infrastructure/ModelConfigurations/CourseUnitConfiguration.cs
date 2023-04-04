using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class CourseUnitConfiguration : IEntityTypeConfiguration<CourseUnit>
    {
        public void Configure(EntityTypeBuilder<CourseUnit> builder)
        {
            builder.HasOne(x => x.Course)
                .WithMany(x => x.CourseUnits)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
