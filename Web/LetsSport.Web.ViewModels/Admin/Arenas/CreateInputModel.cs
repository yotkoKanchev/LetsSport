﻿namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateInputModel : IMapTo<Arena>
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sport Type")]
        public int SportId { get; set; }

        [Display(Name = "Price per hour")]
        [Range(0, 10000000)]
        public double PricePerHour { get; set; }

        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Web-site")]
        public string WebUrl { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ArenaStatus Status { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }

        public string CountryName { get; set; }

        [Required]
        [Display(Name = "City")]
        public int CityId { get; set; }

        public string CityName { get; set; }

        [MinLength(5)]
        [MaxLength(200)]
        [Display(Name = "Street Address")]
        public string Address { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile MainImageFile { get; set; }

        public ICollection<IFormFile> ImageFiles { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
