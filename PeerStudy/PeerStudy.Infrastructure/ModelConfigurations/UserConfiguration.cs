using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(b => b.FirstName)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(b => b.LastName)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(b => b.Email)
                .IsRequired()
                .HasMaxLength(30);

            builder.Ignore(b => b.Password);
        }
    }
}
