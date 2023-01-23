using System.ComponentModel.DataAnnotations;

namespace ServiceForRssSubscriptionManagement.Models.DataAccess.Feeds
{
    class ItemEntity
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Category { get; set; }
        public string? Comments { get; set; }
        public string? EnclosureUrl { get; set; }
        public long? EnclosureLength { get; set; }
        public string? EnclosureType { get; set; }
        public DateTime? PubDate { get; set; }
        public string? SourceName { get; set; }
        public string? SourceUrl { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;
    }
}
