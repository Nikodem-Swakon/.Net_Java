using System;
using WebApplication2.Models; // żeby mógł widzieć klasę Profile
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Video
    { //[Required] dodane jako walidacja danych 
        [Required] public int Id { get; set; }
        [Required] public string VideoId { get; set; } = string.Empty;
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string ChannelId { get; set; } = string.Empty;
        [Required] public DateTime PublishedAt { get; set; }
        [Required] public bool IsFavorite { get; set; }

        [Required] public int ProfileId { get; set; } // FK
        public Profile? Profile { get; set; } // Nawigacja
    }
}
