using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class ActiveDirectorySettings
    {
        [Required]
        public string Domain { get; set; } = string.Empty;

        [Required]
        public string LdapPath { get; set; } = string.Empty;

        [Required]
        public ServiceAccountSettings ServiceAccount { get; set; } = new ServiceAccountSettings();

        public bool UseServerBinding { get; set; } = true;

        [Range(5, 300)]
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class ServiceAccountSettings
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthenticationSettings
    {
        public int MaxFailedAttempts { get; set; } = 5;
        
        public int LockoutMinutes { get; set; } = 15;
        
        public bool EnableAccountLockout { get; set; } = true;
    }
}
