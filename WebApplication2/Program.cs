using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;       // Rejestracja kontekstu bazy danych
using WebApplication2.Services;   // Rejestracja serwisu do pracy z YouTube API

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Rejestrujemy serwis YouTubeApiService (wstrzykiwany w kontrolerach)
builder.Services.AddScoped<YouTubeApiService>();

//  Włączamy obsługę kontrolerów i widoków MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja nasłuchiwania na wszystkich interfejsach sieciowych (IPv4 i IPv6)
app.Urls.Add("http://0.0.0.0:80");  // Nasłuchujemy na IPv4 i IPv6, port 8080

//  Middleware obsługujący routing (mapowanie URL → kontroler/akcja)
app.UseRouting();

//  Middleware do autoryzacji (jeśli będzie używany np. Identity)
app.UseAuthorization();

//  Domyślna trasa: YouTubeController, Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=YouTube}/{action=Index}/{id?}");

app.Run();