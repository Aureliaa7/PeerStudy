using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    class CourseResourceConfiguration : IEntityTypeConfiguration<CourseResource>
    {
        public void Configure(EntityTypeBuilder<CourseResource> builder)
        {
            builder.HasOne(x => x.Owner)
                .WithMany(x => x.CourseResources)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
