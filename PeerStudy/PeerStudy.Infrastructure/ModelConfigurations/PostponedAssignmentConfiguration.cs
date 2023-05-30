using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class PostponedAssignmentConfiguration : IEntityTypeConfiguration<PostponedAssignment>
    {
        public void Configure(EntityTypeBuilder<PostponedAssignment> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.PostponedAssignments)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Assignment).WithMany(x => x.PostponedAssignments)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
