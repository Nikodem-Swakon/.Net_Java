namespace WebApplication2.Models
{
    // Model reprezentujący film z YouTube
    public class Video
    {
        // Unikalny identyfikator w bazie danych (klucz główny)
        public int Id { get; set; }

        // Id filmu z YouTube (np. "dQw4w9WgXcQ")
        public string VideoId { get; set; } = string.Empty;

        // Tytuł filmu
        public string Title { get; set; } = string.Empty;

        // Identyfikator kanału YouTube, który opublikował film
        public string ChannelId { get; set; } = string.Empty;

        // Data publikacji filmu
        public DateTime PublishedAt { get; set; }

        // Czy film jest oznaczony jako "ulubiony"
        public bool IsFavorite { get; set; } = false;
    }
}