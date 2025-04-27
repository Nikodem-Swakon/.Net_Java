namespace WebApplication2.Models
{

    /// <summary>
    /// Model widoku profilu z informacjami o liczbie filmów.
    /// Używany do przekazywania danych do widoku zarządzania profilami.
    /// </summary>
    public class ProfileViewModel
    {
        public required Profile Profile { get; set; }
        public int VideoCount { get; set; }
    }
}
