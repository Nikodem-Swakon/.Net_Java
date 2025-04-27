using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebApplication2.Models
{
    /// <summary>
    /// Model reprezentujący profil użytkownika.
    /// </summary>
    public class Profile
    { //[Required] dodane jako walidacja danych 
        [Required] public int Id { get; set; }

        // Name or label for the profile (e.g., "Home", "Work")
        [Required] public string Name { get; set; } = string.Empty;

        // Navigation property - list of videos linked to this profile
        public List<Video> Videos { get; set; } = new();
    }
}
