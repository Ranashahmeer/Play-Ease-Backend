using Microsoft.EntityFrameworkCore;
using static Play_ease_Backend.NewFolder.LoginModels; // 👈 import User model

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }   // ✅ correct
}
