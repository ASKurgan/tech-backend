using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectTemplate.Infrastructure.Configurations.Read;

public class StudentAccountDtoConfiguration : IEntityTypeConfiguration<StudentAccountDto>
{
    public void Configure(EntityTypeBuilder<StudentAccountDto> builder)
    {
        builder.ToTable("student_accounts");

        builder.HasKey(s => s.Id);
    }
}