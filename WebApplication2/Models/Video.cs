using System;
using WebApplication2.Models; // żeby mógł widzieć klasę Profile

namespace WebApplication2.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string VideoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public bool IsFavorite { get; set; }

        public int ProfileId { get; set; } // FK
        public Profile? Profile { get; set; } // Nawigacja
    }
}
