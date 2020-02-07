namespace LetsSport.Data.Models.ArenaModels
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;

    public class ArenaAdmin : IdentityUser
    {
        [MaxLength(100)]
        public string Occupation { get; set; }

        public int ArenaId { get; set; }

        [Required]
        public virtual Arena Arena { get; set; }
    }
}
