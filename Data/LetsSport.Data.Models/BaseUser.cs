namespace LetsSport.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    using static LetsSport.Common.GlobalConstants;

    public abstract class BaseUser : BaseDeletableModel<string>
    {
        [Required]
        [MaxLength(MaximumUserNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(MaximumUserNameLength)]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MaxLength(100)]
        [Required]
        public string Username { get; set; }

        [MaxLength(100)]
        [Required]
        public string PasswordHash { get; set; }
    }
}
