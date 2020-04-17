namespace LetsSport.Common
{
    public static class ErrorMessages
    {
        public const string ArenaInvalidIdErrorMessage = "Arena with ID: {0} does not exist.";
        public const string UserWithoutArenaErrorMessage = "User with ID: {0} does not have arena!";

        public const string CityInvalidIdErrorMessage = "City with ID: {0} does not exists!";
        public const string CityInvalidNameErrorMesage = "City with name: {0} in country with ID: {1} does not exists.";
        public const string CityExistsMessage = "City with name: {0} already exists in country with ID: {1}";

        public const string CountryInvalidNameErrorMessage = "Country with name: {0} does not exists!";
        public const string CountryInvalidIdErrorMessage = "Country with ID: {0} does not exists!";
        public const string CountryExistsMessage = "Country with name: {0} already exists.";

        public const string EventInvalidIdErrorMessage = "Event with ID: {0} does not exist.";
        public const string EventIvanlidIdwithUserIdErrorMessage = "User with that ID: {0} does not participate in event with ID: {1}";
        public const string EventLeavingMessage = "Sorry, I have to leave the event!";
        public const string EventJoiningMessage = "Hi, I just joined the event!";
        public const string EventCreationMessage = "Hi, I just created the event!";

        public const string FileFormatErrorMessage = "File format not supported!";
        public const string InvalidImageIdErrorMessage = "Image with ID: {0} does not exists.";

        public const string MessageInvalidIdErrorMessage = "Message with ID: {0} does not exist.";

        public const string RentalRequestInvalidIdErrorMessage = "Request with ID: {0} can not be found!";

        public const string ReportInvalidIdErrorMessage = "Report with ID: {0} does not exists!";

        public const string SportInvalidIdErrorMessage = "Sport with ID: {0} does not exists.";
        public const string SportExistsMessage = "Sport with name: {0} already exists.";
    }
}
