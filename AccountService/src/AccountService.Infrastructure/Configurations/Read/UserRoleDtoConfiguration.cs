using AccountService.Application.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Infrastructure.Configurations.Read;

public class UserRoleDtoConfiguration : IEntityTypeConfiguration<UserRolesDataModel>
{
    public void Configure(EntityTypeBuilder<UserRolesDataModel> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(u => new { u.UserId, u.RoleId });
    }
}