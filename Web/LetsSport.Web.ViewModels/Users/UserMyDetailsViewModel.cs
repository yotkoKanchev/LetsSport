namespace LetsSport.Web.ViewModels.Users
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using LetsSport.Data.Common;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class UserMyDetailsViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        [Display(Name = "Favorite sport")]
        public string SportName { get; set; }

        public string Location { get; set; }

        public int? Age { get; set; }

        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string AvatarId { get; set; }

        public string AvatarUrl { get; set; }

        public IFormFile NewAvatarImage { get; set; }

        public string Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public string FaceBookAccount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserMyDetailsViewModel>()
               .ForMember(vm => vm.FullName, opt => opt.MapFrom(u => u.FirstName + " " + u.LastName))
               .ForMember(vm => vm.Location, opt => opt.MapFrom(u => u.City.Name + ", " + u.City.Country.Name))
               .ForMember(vm => vm.OrginizedEventsCount, opt => opt.MapFrom(u => u.Events.Count))
               .ForMember(vm => vm.Status, opt => opt.MapFrom(u => u.Status.GetDisplayName()));
        }
    }
}
