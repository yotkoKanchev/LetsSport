namespace LetsSport.Data.Models.ArenaModels
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    using static LetsSport.Common.GlobalConstants;

    public class ArenaAdmin : BaseDeletableModel<string>
    {
        [Required]
        [MaxLength(MaximumUserNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(MaximumUserNameLength)]
        public string LasstName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MaxLength(100)]
        [Required]
        public string Username { get; set; }

        [MaxLength(100)]
        [Required]
        public string Password { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }

        [MaxLength(100)]
        public string PhoneNumber { get; set; }
    }
}
