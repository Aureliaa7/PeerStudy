using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    class StudentStudyGroupConfiguration : IEntityTypeConfiguration<StudentStudyGroup>
    {
        public void Configure(EntityTypeBuilder<StudentStudyGroup> builder)
        {
            builder.HasOne(x => x.StudyGroup)
                .WithMany(x => x.StudentStudyGroups)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Student)
                .WithMany(x => x.StudentStudyGroups)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
