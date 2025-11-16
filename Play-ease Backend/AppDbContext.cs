using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder;
using static Play_ease_Backend.NewFolder.LoginModels; // 👈 import User model

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }   // ✅ correct

    public DbSet<PlayerRequest> PlayerRequests { get; set; }

    public DbSet<MatchApplicant> MatchApplicants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MatchApplicant>()
            .ToTable("applicants"); // <-- FIXED TABLE NAME

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.Id)
            .HasColumnName("id");

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.MatchId)
            .HasColumnName("match_id");

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.UserId)
            .HasColumnName("user_id");

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.UserName)
            .HasColumnName("user_name");

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.Role)
            .HasColumnName("role");

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.Status)
            .HasColumnName("status");

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.AppliedAt)
            .HasColumnName("applied_at");
    }

}
