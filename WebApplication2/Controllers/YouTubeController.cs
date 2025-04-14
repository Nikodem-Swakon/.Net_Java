using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly YouTubeApiService _youTubeApiService;
        private readonly ApplicationDbContext _context;



        // Akcja do utworzenia nowego profilu
    [HttpPost]
    public async Task<IActionResult> CreateProfile(string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName))
        {
            // Jeśli nazwa jest pusta, zwrócimy błąd
            ModelState.AddModelError("", "Nazwa profilu nie może być pusta.");
            return View();
        }

        // Sprawdzamy, czy profil o tej nazwie już istnieje
        if (_context.Profiles.Any(p => p.Name == profileName))
        {
            ModelState.AddModelError("", "Profil o tej nazwie już istnieje.");
            return View();
        }

        // Tworzymy nowy profil
        var newProfile = new Profile
        {
            Name = profileName
        };

        // Dodajemy profil do bazy danych
        _context.Profiles.Add(newProfile);
        await _context.SaveChangesAsync();

        // Możesz przekierować użytkownika na stronę profili lub inne miejsce
        return RedirectToAction("Index", "YouTube");
    }




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

        public IActionResult CreateProfile()
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
    // Załóżmy, że domyślny profil ma ID 1. Możesz dostosować ten kod do swojej logiki.
    var defaultProfile = await _context.Profiles.FirstOrDefaultAsync(); // Pobieramy domyślny profil
    
    if (defaultProfile == null)
    {
        // Jeśli nie ma profilu, tworzymy nowy (przykładowy)
        defaultProfile = new Profile
        {
            Name = "Default Profile" // Lub jakiekolwiek dane chcesz ustawić
        };
        _context.Profiles.Add(defaultProfile);
        await _context.SaveChangesAsync(); // Zapisz profil w bazie
    }

    // Jeśli tego filmu nie ma jeszcze w bazie, dodajemy
    if (!_context.Videos.Any(v => v.VideoId == videoId))
    {
        var favoriteVideo = new Video
        {
            VideoId = videoId,
            Title = title,
            ChannelId = channelId,
            PublishedAt = publishedAt,
            IsFavorite = true,
            ProfileId = defaultProfile.Id // Przypisujemy domyślny profil do filmu
        };

        _context.Videos.Add(favoriteVideo);
        await _context.SaveChangesAsync();
    }

    // Zwracamy JSON z id filmu, który właśnie został dodany (żeby przycisk działał od razu)
    return Json(new { VideoId = videoId, IsFavorite = true });
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





