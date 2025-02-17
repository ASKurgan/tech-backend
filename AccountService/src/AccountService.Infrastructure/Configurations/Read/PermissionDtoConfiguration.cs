using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Infrastructure.Configurations.Read;

public class PermissionDtoConfiguration: IEntityTypeConfiguration<PermissionDto>
{
    public void Configure(EntityTypeBuilder<PermissionDto> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(u => u.Id);
    }
}