using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTemplate.Domain;

namespace ProjectTemplate.Infrastructure.Configurations.Write;

public class ParticipantAccountConfiguration : IEntityTypeConfiguration<ParticipantAccount>
{
    public void Configure(EntityTypeBuilder<ParticipantAccount> builder)
    {
        builder.ToTable("participant_accounts");
        
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<ParticipantAccount>(p => p.UserId);
    }
}