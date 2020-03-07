namespace LetsSport.Web.ViewModels.UsersProfile
{
    using Microsoft.AspNetCore.Http;

    public class UserProfileDetailsViewModel
    {
        public string UserProfileId { get; set; }

        public string FullName { get; set; }

        public string FavoriteSport { get; set; }

        public string Location { get; set; }

        public int? Age { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string AvatarImageId { get; set; }

        public string AvatarImageUrl { get; set; }

        public IFormFile NewAvatarImage { get; set; }

        public string Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public string FaceBookAccount { get; set; }
    }
}
