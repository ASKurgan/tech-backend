using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Infrastructure.Configurations.Read;

public class RolePermissionDtoConfiguration: IEntityTypeConfiguration<RolePermissionDto>
{
    public void Configure(EntityTypeBuilder<RolePermissionDto> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }
}