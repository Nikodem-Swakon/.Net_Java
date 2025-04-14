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
        public DbSet<Profile> Profiles { get; set; }

        // Konfiguracja relacji w bazie danych
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Video>()
        .HasOne(v => v.Profile)  // Video ma jeden Profile
        .WithMany(p => p.Videos)  // Profile może mieć wiele Video (zmieniono na Videos)
        .HasForeignKey(v => v.ProfileId);  // Klucz obcy w Video
}

    }
}
