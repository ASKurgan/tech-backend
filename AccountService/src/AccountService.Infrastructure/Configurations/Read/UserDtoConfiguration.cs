using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTemplate.Application.DataModels;
using SachkovTech.Core.Database;

namespace ProjectTemplate.Infrastructure.Configurations.Read;

public class UserDtoConfiguration : IEntityTypeConfiguration<UserDataModel>
{
    public void Configure(EntityTypeBuilder<UserDataModel> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<UserRolesDataModel>(
                e => e.HasOne<UserDataModel>()
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey("UserId")
            );

        builder.Property(s => s.SocialNetworks)
            .ValueObjectsCollectionJsonConversion(
                input => input,
                output => output)
            .HasColumnName("social_networks");

        builder.HasOne(u => u.StudentAccount)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey<StudentAccountDto>(s => s.UserId);

        builder.HasOne(u => u.SupportAccount)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey<SupportAccountDto>(s => s.UserId);

        builder.HasOne(u => u.AdminAccount)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey<AdminAccountDto>(s => s.UserId);
    }
}