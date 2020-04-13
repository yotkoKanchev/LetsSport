namespace LetsSport.Web.ViewModels.Users
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;

    public class UserDetailsViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
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

        public string AvatarUrl { get; set; }

        public UserStatus Status { get; set; }

        [Display(Name = "Orginized Events")]
        public int OrginizedEventsCount { get; set; }

        [Display(Name = "Facebook")]
        public string FaceBookAccount { get; set; }

        public string Ocupation { get; set; }

        public string UserScore { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserDetailsViewModel>()
               .ForMember(vm => vm.FullName, opt => opt.MapFrom(u => u.FirstName + " " + u.LastName))
               .ForMember(vm => vm.Location, opt => opt.MapFrom(u => u.City.Name + ", " + u.City.Country.Name))
               .ForMember(vm => vm.OrginizedEventsCount, opt => opt.MapFrom(u => u.Events.Count))
                .ForMember(vm => vm.UserScore, opt => opt.MapFrom(e => $"{e.Events.Count}/{e.AdministratingEvents.Count}"));
        }
    }
}
