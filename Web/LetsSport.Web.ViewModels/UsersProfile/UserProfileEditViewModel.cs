namespace LetsSport.Web.ViewModels.UsersProfile
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.UserModels;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class UserProfileEditViewModel
    {
        public string Id { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; }

        public int Sport { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(16)]
        public string PhoneNumber { get; set; }

        // [RegularExpression(@"(?https://)?(?:www.)?facebook.com/")]
        public string FaceBookAccount { get; set; }

        public int Country { get; set; }

        public int City { get; set; }

        public UserStatus? Status { get; set; }

        public string Occupation { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
