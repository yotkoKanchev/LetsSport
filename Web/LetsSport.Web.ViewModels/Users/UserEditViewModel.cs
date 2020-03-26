﻿namespace LetsSport.Web.ViewModels.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class UserEditViewModel : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        public string Id { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Sport")]
        public int SportId { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(16)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // [RegularExpression(@"(?https://)?(?:www.)?facebook.com/")]
        [Display(Name = "FaceBook Account")]
        public string FaceBookAccount { get; set; }

        [Display(Name = "Country")]
        public string CityCountryName { get; set; }

        [Display(Name = "City")]
        public int CityId { get; set; }

        public UserStatus? Status { get; set; }

        public string Occupation { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
