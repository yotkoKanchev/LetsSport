namespace LetsSport.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "LetsSport";
        public const string SystemEmail = "yokibul@gmail.com";
        //public const string SystemEmail = "admin@letssport.com";

        public const string OwnerName = "Yotko Kanchev";

        public const string AdministratorRoleName = "Administrator";
        public const string UserRoleName = "User";
        public const string ArenaAdminRoleName = "ArenaAdministrator";

        public const string AdministrationAreaName = "Administration";

        public const string DefaultDateFormat = "dd-MMM-yyyy";
        public const string DefaultTimeFormat = "HH:mm";
        public const string DefaultDateTimeFormat = "dd-MMM-yy HH:mm";
        public const string DefaultFilteringFieldDate = "yyyy-MM-dd";

        public const int ResultsPerPageCount = 8;
        public const int AdminLargeItemsPerPageCount = 10;
        public const int AdminItemsPerPageCount = 20;
        public const int ImageMaxSizeMB = 10;

        public const string TempDataMessage = "message";

        public const string CloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        public const string DefaultMainImagePath = "../../images/noArena.png";
        public const string NoAvatarImagePath = "../../images/noAvatar.png";

        // public const string ImageSizing = "w_480,h_288,c_scale,r_5,bo_1px_solid_silver/";
        public const string ImageSizing = "w_384,h_216,c_scale,r_10,bo_1px_solid_silver/";
        public const string CardImageSizing = "w_384,h_256,c_scale,r_10,bo_1px_solid_silver/";
        public const string MainImageSizing = "w_768,h_432,c_scale,r_10,bo_1px_solid_silver/";
        public const string AvatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_300/";

        public const string City = "city";
        public const string Country = "country";

        public const int MessageContentMinLength = 1;
        public const int MessageContentMaxLength = 1000;
    }
}
