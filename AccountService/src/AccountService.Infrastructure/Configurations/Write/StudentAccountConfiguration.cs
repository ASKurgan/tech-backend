using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTemplate.Domain;

namespace ProjectTemplate.Infrastructure.Configurations.Write;

public class StudentAccountConfiguration : IEntityTypeConfiguration<StudentAccount>
{
    public void Configure(EntityTypeBuilder<StudentAccount> builder)
    {
        builder.ToTable("student_accounts");
    }
}