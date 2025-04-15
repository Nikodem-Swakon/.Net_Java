using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;       // Rejestracja kontekstu bazy danych
using WebApplication2.Services;   // Rejestracja serwisu do pracy z YouTube API

var builder = WebApplication.CreateBuilder(args);

//  Konfiguracja połączenia z bazą danych SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Rejestrujemy serwis YouTubeApiService (wstrzykiwany w kontrolerach)
builder.Services.AddScoped<YouTubeApiService>();

//  Włączamy obsługę kontrolerów i widoków MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

//  Middleware obsługujący routing (mapowanie URL → kontroler/akcja)
app.UseRouting();

//  Middleware do autoryzacji (jeśli będzie używany np. Identity)
app.UseAuthorization();

//  Domyślna trasa: YouTubeController, Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=YouTube}/{action=Index}/{id?}");

app.Run();