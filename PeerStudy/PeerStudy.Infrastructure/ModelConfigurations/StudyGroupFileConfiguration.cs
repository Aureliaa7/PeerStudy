using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    class StudyGroupFileConfiguration : IEntityTypeConfiguration<StudyGroupFile>
    {
        public void Configure(EntityTypeBuilder<StudyGroupFile> builder)
        {
            builder.HasOne(x => x.Owner)
                .WithMany(x => x.StudyGroupFiles)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
