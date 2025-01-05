using CommentService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CommentService.Infrastructure;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    private const string DATABASE = "Database";

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(DATABASE));
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>()
            .ToTable("comments");

        modelBuilder.Entity<Comment>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Comment>()
            .Property(c => c.Id)
            .HasColumnName("id");

        modelBuilder.Entity<Comment>()
            .Property(c => c.RelationId)
            .HasColumnName("relation_id")
            .IsRequired();

        modelBuilder.Entity<Comment>()
            .Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        modelBuilder.Entity<Comment>()
            .Property(c => c.RepliedId)
            .HasColumnName("replied_id")
            .IsRequired(false);

        modelBuilder.Entity<Comment>()
            .Property(c => c.Text)
            .HasColumnName("text")
            .HasMaxLength(Comment.TEXT_MAX_LENGTH)
            .IsRequired();

        modelBuilder.Entity<Comment>()
            .Property(c => c.Rating)
            .HasColumnName("rating")
            .IsRequired();

        modelBuilder.Entity<Comment>()
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}