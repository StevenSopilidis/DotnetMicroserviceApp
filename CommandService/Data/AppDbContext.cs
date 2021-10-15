using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts)
        {}
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(c => c.Platform)
                .HasForeignKey(c => c.PlatformId);
            
            builder.Entity<Command>()
                .HasOne(c => c.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(c => c.PlatformId);
        }
    }
}