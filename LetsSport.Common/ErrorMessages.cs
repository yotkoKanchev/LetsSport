namespace LetsSport.Common
{
    public static class ErrorMessages
    {
        public const string ArenaInvalidIdErrorMessage = "Arena with ID: {0} does not exist.";

        public const string CityInvalidNameErrorMesage = "City with name: {0} in country with ID: {1} does not exists.";
        public const string CityExistsMessage = "City with name: {0} already exists in country with ID: {1}";

        public const string CountryInvalidNameErrorMessage = "Country with name: {0} does not exists!";
        public const string CountryExistsMessage = "Country with name: {0} already exists.";

        public const string EventIvanlidIdwithUserIdErrorMessage = "User with that ID: {0} does not participate in event with ID: {1}";

        public const string FileFormatErrorMessage = "File format not supported!";
        public const string InvalidImageIdErrorMessage = "Image with ID: {0} does not exists.";

        public const string MessageInvalidIdErrorMessage = "Message with ID: {0} does not exist.";

        public const string RentalRequestInvalidIdErrorMessage = "Request with ID: {0} can not be found!";

        public const string ReportInvalidIdErrorMessage = "Report with ID: {0} does not exists!";

        public const string SportExistsMessage = "Sport with name: {0} already exists.";

        public const string UserWithoutArenaErrorMessage = "User with ID: {0} does not have arena!";
        public const string UserInvalidIdErrorMessage = "User with ID: {0} does not exists.";

        public const string ImageMaximSizeErrorMessage = "Maximum allowed file size is {0}MB.";
    }
}
