namespace LetsSport.Data.Models.ArenaModels
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    public class ArenaAdmin : IdentityUser
    {
        [MaxLength(100)]
        public string Occupation { get; set; }

        [Required]
        public int ArenaId { get; set; }

        public virtual Arena Arena { get; set; }
    }
}
