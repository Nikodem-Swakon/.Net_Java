using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly YouTubeApiService _youTubeApiService;
        private readonly ApplicationDbContext _context;

        // Konstruktor: wstrzykiwanie serwisów (API YouTube i baza danych)
        public YouTubeController(YouTubeApiService youTubeApiService, ApplicationDbContext context)
        {
            _youTubeApiService = youTubeApiService;
            _context = context;
        }

        // Widok startowy / formularz wyszukiwania
        public IActionResult Index()
        {
            return View();
        }

        // Wyszukiwanie filmów z YouTube na podstawie zapytania
        public async Task<IActionResult> Search(string query)
        {
            var videos = await _youTubeApiService.SearchVideosAsync(query); // pobranie filmów z API
            var favoriteVideos = _context.Videos.Select(v => v.VideoId).ToList(); // ID ulubionych filmów z bazy

            ViewData["Favorites"] = favoriteVideos; // przekazujemy info do widoku, które filmy są ulubione
            return View("Results", videos); // pokazujemy widok wyników
        }

        // Dodawanie filmu do ulubionych (zapis do bazy)
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(string videoId, string title, string channelId, DateTime publishedAt)
        {
            // Jeśli tego filmu nie ma jeszcze w bazie, dodajemy
            if (!_context.Videos.Any(v => v.VideoId == videoId))
            {
                var favoriteVideo = new Video
                {
                    VideoId = videoId,
                    Title = title,
                    ChannelId = channelId,
                    PublishedAt = publishedAt,
                    IsFavorite = true
                };

                _context.Videos.Add(favoriteVideo);
                await _context.SaveChangesAsync();
            }

            // ⚠️ Uwaga: ten fragment powoduje załadowanie tylko części wyników (dla jednego tytułu)
            // Można tu zamiast tego zrobić RedirectToAction z query
            var videos = await _youTubeApiService.SearchVideosAsync(title);
            return View("Results", videos);
        }

        // Usuwanie filmu z ulubionych
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(string videoId)
        {
            var video = _context.Videos.FirstOrDefault(v => v.VideoId == videoId);
            if (video != null)
            {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Favorites"); // Po usunięciu wracamy na widok ulubionych
        }

        // Widok ulubionych filmów
        public IActionResult Favorites()
        {
            var favoriteVideos = _context.Videos.Where(v => v.IsFavorite).ToList(); // tylko te, które są oznaczone jako ulubione
            return View(favoriteVideos);
        }

        
    }
}





