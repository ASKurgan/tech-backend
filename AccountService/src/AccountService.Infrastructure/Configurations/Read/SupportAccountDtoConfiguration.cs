using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectTemplate.Infrastructure.Configurations.Read;

public class SupportAccountDtoConfiguration : IEntityTypeConfiguration<SupportAccountDto>
{
    public void Configure(EntityTypeBuilder<SupportAccountDto> builder)
    {
        builder.ToTable("support_accounts");

        builder.HasKey(s => s.Id);
    }
}