using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Testing2.Domain;

namespace Testing2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Discussion>().HasMany(x => x.Tags).WithMany(x => x.Discussions);
            //builder.Entity<Tag>().HasMany(x => x.Discussions).WithMany(x => x.Tags);

            base.OnModelCreating(builder);
        }
    }
}