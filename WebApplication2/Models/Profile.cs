using System.Collections.Generic;

namespace WebApplication2.Models
{
    public class Profile
    {
        public int Id { get; set; }

        // Name or label for the profile (e.g., "Home", "Work")
        public string Name { get; set; } = string.Empty;

        // Navigation property - list of videos linked to this profile
        public List<Video> Videos { get; set; } = new();
    }
}
