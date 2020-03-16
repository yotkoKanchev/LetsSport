namespace LetsSport.Web.ViewModels.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class UserUpdateInputModel : IMapTo<ApplicationUser>
    {
        [MinLength(2)]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Range(0, 150)]
        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(16)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile AvatarImage { get; set; }

        public UserStatus Status { get; set; }

        // [RegularExpression(@"(?https://)?(?:www.)?facebook.com/")]       
        [Display(Name = "FaceBook Account")]
        public string FaceBookAccount { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }

        [Display(Name ="Sport")]
        public int SportId { get; set; }

        [Display(Name = "City")]
        public int CityId { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }
    }
}
