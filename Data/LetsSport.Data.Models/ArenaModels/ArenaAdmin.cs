namespace LetsSport.Data.Models.ArenaModels
{
    using System.ComponentModel.DataAnnotations;

    public class ArenaAdmin : BaseUser
    {
        [MaxLength(100)]
        public string Occupation { get; set; }

        [MaxLength(100)]
        public string PhoneNumber { get; set; }
    }
}
