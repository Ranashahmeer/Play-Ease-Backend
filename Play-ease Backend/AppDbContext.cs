using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder;
using static Play_ease_Backend.NewFolder.LoginModels; // 👈 import User model

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }   // ✅ correct

    public DbSet<PlayerRequest> PlayerRequests { get; set; }

    public DbSet<MatchApplicant> MatchApplicants { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }

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

        modelBuilder.Entity<MatchApplicant>()
            .Property(a => a.AcceptedAt)
            .HasColumnName("accepted_at");

        // ChatMessage configuration
        modelBuilder.Entity<ChatMessage>()
            .ToTable("chat_messages");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.Id)
            .HasColumnName("id");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.MatchId)
            .HasColumnName("match_id");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.SenderId)
            .HasColumnName("sender_id");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.SenderName)
            .HasColumnName("sender_name")
            .HasColumnType("nvarchar(255)");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.ReceiverId)
            .HasColumnName("receiver_id");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.ReceiverName)
            .HasColumnName("receiver_name")
            .HasColumnType("nvarchar(255)");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.Message)
            .HasColumnName("message")
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.Timestamp)
            .HasColumnName("timestamp")
            .HasColumnType("datetime2");

        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2");
    }

}
