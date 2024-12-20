using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectTemplate.Infrastructure.Configurations.Read;

public class RoleDtoConfiguration: IEntityTypeConfiguration<RoleDto>
{
    public void Configure(EntityTypeBuilder<RoleDto> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(u => u.Id);
    }
}