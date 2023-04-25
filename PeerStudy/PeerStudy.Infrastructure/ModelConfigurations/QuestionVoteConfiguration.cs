using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class QuestionVoteConfiguration : IEntityTypeConfiguration<QuestionVote>
    {
        public void Configure(EntityTypeBuilder<QuestionVote> builder)
        {
            builder.HasOne(x => x.Author)
               .WithMany(x => x.QuestionVotes)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
