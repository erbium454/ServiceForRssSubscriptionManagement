using Microsoft.EntityFrameworkCore;

namespace ServiceForRssSubscriptionManagement.Models.DataAccess.Feeds
{
    class FeedContext : DbContext
    {
        public DbSet<ItemEntity> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=App_Data/feeds.db");
        }
    }
}
