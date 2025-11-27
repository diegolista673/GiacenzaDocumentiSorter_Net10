using System.ComponentModel.DataAnnotations;

namespace GiacenzaSorterRm.Models
{
    public class ActiveDirectorySettings
    {
        [Required]
        public string Domain { get; set; }

        [Required]
        public string LdapPath { get; set; }

        [Required]
        public ServiceAccountSettings ServiceAccount { get; set; }

        public bool UseServerBinding { get; set; } = true;

        [Range(5, 300)]
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class ServiceAccountSettings
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthenticationSettings
    {
        public int MaxFailedAttempts { get; set; } = 5;
        public int LockoutMinutes { get; set; } = 15;
        public bool EnableAccountLockout { get; set; } = true;
    }
}
