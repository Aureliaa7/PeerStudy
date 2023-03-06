using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class StudentAssignmentConfiguration : IEntityTypeConfiguration<StudentAssignment>
    {
        public void Configure(EntityTypeBuilder<StudentAssignment> builder)
        {
            builder.Property(x => x.CompletedAt).IsRequired(false);
            builder.Property(x => x.Points).IsRequired(false);

            builder.HasOne(x => x.Student).WithMany(x => x.Assignments)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
