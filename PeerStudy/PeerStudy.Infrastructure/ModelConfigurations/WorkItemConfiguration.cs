using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
    {
        public void Configure(EntityTypeBuilder<WorkItem> builder)
        {
            builder.HasOne(x => x.AssignedTo)
                .WithMany(x => x.WorkItems)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
