using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using WebApplication2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication2.Services
{
    // Serwis obsługujący zapytania do API YouTube
    public class YouTubeApiService
    {
        private readonly string _apiKey;

        // Konstruktor: pobiera klucz API z pliku konfiguracyjnego (appsettings.json)
        public YouTubeApiService(IConfiguration configuration)
        {
            _apiKey = configuration["YouTubeApiKey"] ?? throw new ArgumentNullException("YouTube API Key is missing!");
        }

        //  Metoda asynchroniczna do wyszukiwania filmów po zapytaniu tekstowym

        /// <summary>
        /// Wyszukiwanie filmów na YouTube na podstawie zapytania tekstowego.
        /// Metoda asynchroniczna, która zwraca listę obiektów Video.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>lista obiektów video</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<Video>> SearchVideosAsync(string query)
        {
            // Inicjalizacja klienta YouTube API
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = "MyYouTubeApp"
            });

            // Tworzymy zapytanie typu "search" z parametrami
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = query;              // zapytanie wpisane przez użytkownika
            searchRequest.MaxResults = 5;         // liczba wyników 

            // Wysyłamy zapytanie do API
            var searchResponse = await searchRequest.ExecuteAsync();
            var videos = new List<Video>();

            //sprawdzamy, czy odpowiedź z API jest poprawna
            if (searchResponse == null || searchResponse.Items == null || !searchResponse.Items.Any())
            {
                throw new Exception("Brak wyników wyszukiwania lub błąd odpowiedzi z YouTube API.");
            }

            // Przetwarzamy odpowiedź z API
            foreach (var result in searchResponse.Items)
            {
                if (result.Id.VideoId != null)
                {
                    videos.Add(new Video
                    {
                        VideoId = result.Id.VideoId,
                        Title = result.Snippet.Title,
                        ChannelId = result.Snippet.ChannelId,
                        PublishedAt = result.Snippet.PublishedAtDateTimeOffset?.UtcDateTime ?? DateTime.UtcNow
                    });
                }
            }

            return videos; // Zwracamy listę znalezionych filmów
        }
    }
}
