using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class AnswerVoteConfiguration : IEntityTypeConfiguration<AnswerVote>
    {
        public void Configure(EntityTypeBuilder<AnswerVote> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.AnswerVotes)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
