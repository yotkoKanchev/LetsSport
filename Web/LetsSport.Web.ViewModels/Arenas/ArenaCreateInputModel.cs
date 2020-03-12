namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenaCreateInputModel : IMapFrom<Arena>, IMapTo<Arena>
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

        [Required]
        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Web-site")]
        public string WebUrl { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int Country { get; set; }

        [Required]
        public int City { get; set; }

        [MinLength(5)]
        [MaxLength(200)]
        public string StreetAddress { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public ICollection<IFormFile> Pictures { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }

        //public void CreateMappings(IProfileExpression configuration)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
