using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.Property(x => x.Description).IsRequired(false);
            builder.Property(x => x.Deadline).IsRequired(false);

            builder.HasOne(x => x.StudyGroup).WithMany(x => x.Assignments)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
