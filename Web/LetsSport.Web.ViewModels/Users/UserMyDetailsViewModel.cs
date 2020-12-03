namespace LetsSport.Web.ViewModels.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using AutoMapper;
    using LetsSport.Data.Common;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.Users;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class UserMyDetailsViewModel : IValidatableObject, IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        private readonly string[] validImageExtensions = { ".ai", ".gif", ".webp", ".bmp", ".djvu", ".ps", ".ept", ".eps", ".eps3", ".fbx", ".flif", ".gif", ".gltf", ".heif", ".heic", ".ico", ".indd", ".jpg", ".jpe", ".jpeg", ".jp24", ".wdp", ".jxr", ".hdp", ".pdf", ".png", ".psd", ".arw", ".cr2", ".svg", ".tga", ".tif", ".tiff", ".webp", };

        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        [Display(Name = "Favorite sport")]
        public string SportName { get; set; }

        public string Location { get; set; }

        public int? Age { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string AvatarId { get; set; }

        public string AvatarUrl { get; set; }

        [AllowedExtensions]
        [MaxFileSize]
        public IFormFile NewAvatarImage { get; set; }

        public string Status { get; set; }

        [Display(Name = "Orinized Events:")]
        public int OrginizedEventsCount { get; set; }

        [Display(Name = "Facebook")]
        public string FaceBookAccount { get; set; }

        public string Occupation { get; set; }

        public string UserScore { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserMyDetailsViewModel>()
               .ForMember(vm => vm.FullName, opt => opt.MapFrom(u => u.FirstName + " " + u.LastName))
               .ForMember(vm => vm.Location, opt => opt.MapFrom(u => u.City.Name + ", " + u.City.Country.Name))
               .ForMember(vm => vm.OrginizedEventsCount, opt => opt.MapFrom(u => u.Events.Count))
               .ForMember(vm => vm.Status, opt => opt.MapFrom(u => u.Status.GetDisplayName()))
               .ForMember(vm => vm.UserScore, opt => opt.MapFrom(e => $"{e.Events.Count}/{e.AdministratingEvents.Count}"));
        }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (!this.validImageExtensions.Any(e => this.NewAvatarImage.FileName.EndsWith(e)))
            {
                yield return new ValidationResult("File format not supported!");
            }
        }
    }
}
