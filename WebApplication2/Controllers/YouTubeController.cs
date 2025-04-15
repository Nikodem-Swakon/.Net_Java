using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 


namespace WebApplication2.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly YouTubeApiService _youTubeApiService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<YouTubeController> _logger; // Logger do rejestrowania informacji

     public YouTubeController(YouTubeApiService youTubeApiService, ApplicationDbContext context, ILogger<YouTubeController> logger)
        {
            _youTubeApiService = youTubeApiService;
            _context = context;
            _logger = logger;  // Assigning the injected logger
        }




        public IActionResult AddToFavoritesForm() //trochę shit ale potrzebne jako przekazanie danych przed kolejnymi funkcjami
    {
        // Pobieramy dostępne profile z bazy
        var profiles = _context.Profiles.ToList();

        // Przekazujemy je do widoku
        ViewData["Profiles"] = profiles;

        return View();
    }


public IActionResult ManageProfiles()
{
    var profiles = _context.Profiles
        .Select(p => new WebApplication2.Models.ProfileViewModel
        {
            Profile = p,
            VideoCount = _context.Videos.Count(v => v.ProfileId == p.Id)
        })
        .ToList();

        _logger.LogInformation("Profile count: {ProfileCount}", profiles.Count);

    ViewData["Profiles"] = profiles;  // Przypisanie listy profili do ViewData
    return View();
}


[HttpPost]
public async Task<IActionResult> DeleteProfile(int profileId)
{
    var profile = await _context.Profiles.FindAsync(profileId);
    if (profile == null)
        return NotFound();

    // Usuń filmy powiązane z profilem
    var videos = _context.Videos.Where(v => v.ProfileId == profileId);
    _context.Videos.RemoveRange(videos);

    _context.Profiles.Remove(profile);
    await _context.SaveChangesAsync();

    return RedirectToAction("CreateProfile"); // Zamiast RedirectToAction("Index", "YouTube"), przekieruj do ManageProfiles
}



        // Akcja do utworzenia nowego profilu
[HttpPost]
public async Task<IActionResult> CreateProfile(string profileName)
{
    if (string.IsNullOrWhiteSpace(profileName))
    {
        ModelState.AddModelError("", "Nazwa profilu nie może być pusta.");
        return View(); // Jeśli nazwa profilu jest pusta, zwróć widok z błędami
    }

    if (_context.Profiles.Any(p => p.Name == profileName))
    {
        ModelState.AddModelError("", "Profil o tej nazwie już istnieje.");
        return View(); // Jeśli profil o tej nazwie już istnieje, zwróć widok z błędami
    }

    var newProfile = new Profile
    {
        Name = profileName
    };

    _context.Profiles.Add(newProfile);
    await _context.SaveChangesAsync();

    return RedirectToAction("CreateProfile"); // Zamiast RedirectToAction("Index", "YouTube"), przekieruj do ManageProfiles
}





 
        // Widok startowy / formularz wyszukiwania
        public IActionResult Index()
        {
            return View();
        }

public IActionResult CreateProfile()
{
    var profiles = _context.Profiles
        .Select(p => new ProfileViewModel
        {
            Profile = p,
            VideoCount = _context.Videos.Count(v => v.ProfileId == p.Id)
        })
        .ToList();

    //ViewData["Profiles"] = profiles;
    return View(profiles);
}



        // Wyszukiwanie filmów z YouTube na podstawie zapytania
        public async Task<IActionResult> Search(string query)
        {
            var videos = await _youTubeApiService.SearchVideosAsync(query); // pobranie filmów z API
            var favoriteVideos = _context.Videos.Select(v => v.VideoId).ToList(); // ID ulubionych filmów z bazy
            var profiles = await _context.Profiles.ToListAsync(); // Pobieramy dostępne profile


            ViewData["Favorites"] = favoriteVideos; // przekazujemy info do widoku, które filmy są ulubione
            ViewData["Profiles"] = profiles; // przekazujemy dostępne profile do widoku

            return View("Results", videos); // pokazujemy widok wyników
        }

        // Dodawanie filmu do ulubionych (zapis do bazy)
[HttpPost]
public async Task<IActionResult> AddToFavorites(string videoId, string title, string channelId, DateTime publishedAt, int profileId)
{
    if (string.IsNullOrWhiteSpace(videoId) || videoId.Length > 20)  //walidacja danych
        ModelState.AddModelError("videoId", "Nieprawidłowy identyfikator filmu.");

    if (string.IsNullOrWhiteSpace(title) || title.Length > 100)
        ModelState.AddModelError("title", "Tytuł jest wymagany i może mieć maks. 100 znaków.");

    if (string.IsNullOrWhiteSpace(channelId) || channelId.Length > 50)
        ModelState.AddModelError("channelId", "Nieprawidłowy identyfikator kanału.");

    if (publishedAt > DateTime.Now)
        ModelState.AddModelError("publishedAt", "Data publikacji nie może być w przyszłości.");


    // Sprawdzamy, czy wybrany profil istnieje w bazie
    var selectedProfile = await _context.Profiles.FindAsync(profileId);

    if (selectedProfile == null)
    {
        ModelState.AddModelError("", "Nie znaleziono wybranego profilu.");
        return View();  // Możesz przekazać błędy do widoku, jeśli nie ma takiego profilu
    }

    // Jeśli tego filmu nie ma jeszcze w bazie, dodajemy go
    if (!_context.Videos.Any(v => v.VideoId == videoId))
    {
        var favoriteVideo = new Video
        {
            VideoId = videoId,
            Title = title,
            ChannelId = channelId,
            PublishedAt = publishedAt,
            IsFavorite = true,
            ProfileId = selectedProfile.Id // Przypisujemy wybrany profil do filmu
        };

        _context.Videos.Add(favoriteVideo);
        await _context.SaveChangesAsync();
    }

    // Zwracamy JSON z id filmu, który właśnie został dodany
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
       public IActionResult Favorites(int? profileId, string searchQuery)
{
    var videos = _context.Videos
        .Where(v => v.IsFavorite)
        .AsQueryable();

    if (profileId.HasValue)
    {
        videos = videos.Where(v => v.ProfileId == profileId.Value);
    }

    if (!string.IsNullOrEmpty(searchQuery))
    {
        videos = videos.Where(v => v.Title.Contains(searchQuery));
    }

    var resultList = videos.ToList();
    var profiles = _context.Profiles.ToList();

    ViewData["Profiles"] = profiles;
    ViewData["SelectedProfileId"] = profileId;
    ViewData["SearchQuery"] = searchQuery;

    return View(resultList);
}


/*public IActionResult Search(string query, int? profileId)
{
    var videosQuery = _context.Videos.AsQueryable();

    if (!string.IsNullOrEmpty(query))
    {
        videosQuery = videosQuery.Where(v => v.Title.Contains(query));
    }

    if (profileId.HasValue)
    {
        videosQuery = videosQuery.Where(v => v.ProfileId == profileId.Value);
    }

    var results = videosQuery.ToList();

    var favorites = _context.Videos
        .Where(v => v.IsFavorite)
        .Select(v => v.VideoId)
        .ToList();

    var profiles = _context.Profiles.ToList();

    ViewData["Favorites"] = favorites;
    ViewData["Profiles"] = profiles;
    ViewData["SelectedProfileId"] = profileId;
    ViewData["SearchQuery"] = query;

    return View("Results", results);
}
*/
        
    }
}





