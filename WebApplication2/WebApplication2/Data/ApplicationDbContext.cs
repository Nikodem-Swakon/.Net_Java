using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    // Klasa reprezentująca kontekst bazy danych
    // Dziedziczy po DbContext (część Entity Framework)
    public class ApplicationDbContext : DbContext
    {
        // Konstruktor – przekazuje opcje konfiguracyjne do klasy bazowej
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        // Reprezentacja tabeli "Videos" w bazie danych
        public DbSet<Video> Videos { get; set; }
    }
}


