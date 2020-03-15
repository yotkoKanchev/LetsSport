namespace LetsSport.Web.ViewModels.UsersProfile
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class UserProfileDetailsViewModel : IMapTo<UserProfile>, IMapFrom<UserProfile>, IHaveCustomMappings
    {
        public string ApplicationUserId { get; set; }

        public string UserProfileId { get; set; }

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

        public UserStatus? Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public string FaceBookAccount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserProfile, UserProfileDetailsViewModel>()
               .ForMember(vm => vm.FullName, opt => opt.MapFrom(u => u.FirstName + " " + u.LastName))
               .ForMember(vm => vm.Location, opt => opt.MapFrom(u => u.City.Name + ", " + u.City.Country.Name))
               .ForMember(vm => vm.OrginizedEventsCount, opt => opt.MapFrom(u => u.ApplicationUser.Events.Count))
               .ForMember(vm => vm.UserProfileId, opt => opt.MapFrom(u => u.Id));
            //.ForMember(vm => vm.AvatarUrl, opt => opt.MapFrom(u => u.Avatar.Url));
        }
    }
}
