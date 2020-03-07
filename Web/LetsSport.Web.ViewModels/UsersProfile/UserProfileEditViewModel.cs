namespace LetsSport.Web.ViewModels.UsersProfile
{
    public class UserProfileEditViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FavoriteSport { get; set; }

        public int? Age { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string FacebookAccount { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Status { get; set; }

        // if ArenaAdminOnly
        public string Ocupation { get; set; }
    }
}
