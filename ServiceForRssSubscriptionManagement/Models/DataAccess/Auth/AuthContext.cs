using Microsoft.EntityFrameworkCore;
using ServiceForRssSubscriptionManagement.Models.DataModels;
using System.Reflection.Emit;

namespace ServiceForRssSubscriptionManagement.Models.DataAccess.Auth
{
    public class AuthContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=App_Data/users.db");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
            builder.Entity<UserEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
