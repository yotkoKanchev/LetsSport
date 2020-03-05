namespace LetsSport.Web.ViewModels.UsersProfile
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class UserProfileCreateInputModel
    {
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; }

        public string FavoriteSport { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        [Range(0, 150)]
        public int? Age { get; set; }

        public string Gender { get; set; }

        [MaxLength(16)]
        public string PhoneNumber { get; set; }

        public IFormFile AvatarUrl { get; set; }

        public string Status { get; set; }

        //[RegularExpression(@"(?https://)?(?:www.)?facebook.com/")]
        public string FaceBookAccount { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }
    }
}
