using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTemplate.Application;
using ProjectTemplate.Domain;

namespace ProjectTemplate.Infrastructure.Configurations.Write;

public class SupportAccountConfiguration : IEntityTypeConfiguration<SupportAccount>
{
    public void Configure(EntityTypeBuilder<SupportAccount> builder)
    {
        builder.ToTable("support_accounts");

        builder.Property(s => s.AboutSelf)
            .HasMaxLength(Constants.Default.MAX_HIGH_TEXT_LENGTH)
            .IsRequired();
    }
}