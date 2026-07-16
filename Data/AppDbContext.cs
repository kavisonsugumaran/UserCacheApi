using Microsoft.EntityFrameworkCore;
using UserCacheApi.Models;

namespace UserCacheApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {       
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(user => user.Id);

                entity.Property(user => user.Id)
                    .ValueGeneratedNever();

                entity.Property(user => user.Name).HasMaxLength(200).IsRequired();
                entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
                entity.Property(user => user.Email).HasMaxLength(200).IsRequired();
                entity.Property(user => user.Phone).HasMaxLength(100);
                entity.Property(user => user.Website).HasMaxLength(200);
                entity.Property(user => user.CompanyName).HasMaxLength(200);
                entity.Property(user => user.City).HasMaxLength(200);
            });
        }
    }
}
