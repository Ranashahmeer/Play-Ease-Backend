using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder1;
using System.ComponentModel.DataAnnotations;

using static Play_ease_Backend.NewFolder.LoginModels; // 👈 import User model

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }   // ✅ correct
    public DbSet<MatchRequest> MatchRequests { get; set; }
    public DbSet<Applicant> Applicants { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision for Price
        modelBuilder.Entity<MatchRequest>()
            .Property(m => m.Price)
            .HasColumnType("decimal(18,2)");

        // Configure unique constraint for applicants
        modelBuilder.Entity<Applicant>()
            .HasIndex(a => new { a.MatchId, a.UserId })
            .IsUnique();

        // Remove the relationship configuration entirely
    }
}


public class User
{
    [Key]
    public int UserID { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public int RoleID { get; set; }

    public int? Age { get; set; }

    public string? Cnic { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
}
