namespace LetsSport.Data.Models.UserModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class ArenaAdmin : BaseDeletableModel<string>
    {
        public ArenaAdmin()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }
    }
}
