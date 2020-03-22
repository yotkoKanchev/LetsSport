namespace LetsSport.Web
{
    public static class EmailMessagingConstants
    {
        internal static string GetEmailConfirmation(string username, string confirmationLink)
        {
            return $"<div style=\"font-size:20px\">" +
                    $"<div>Hi {username},</div>" +
                    $"<br>" +
                    $"<div>thank you for becoming a member of our sporty comunity.</div>" +
                    $"<div>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</div>" +
                    $"<br>" +
                    $"<div>Sincerely Yours,</div>" +
                    $"<div>LetsSport Team</div>" +
                    $"</div>";
        }
    }
}
