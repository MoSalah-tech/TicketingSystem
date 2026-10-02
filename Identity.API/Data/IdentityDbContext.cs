using Microsoft.EntityFrameworkCore;
using Identity.API.Domain;

namespace Identity.API.Data;

public class IdentityDbContext : DbContext

{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>()
        
            .HasIndex(u => u.Email)
            .IsUnique();
        
    }
    

}
