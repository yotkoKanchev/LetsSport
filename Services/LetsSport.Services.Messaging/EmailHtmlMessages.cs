namespace LetsSport.Services.Messaging
{
    public static class EmailHtmlMessages
    {
        public static string GetEmailConfirmationHtml(string username, string confirmationLink)
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

        public static string GetResetPasswordHtml(string username, string ressetPasswordLink)
        {
            return $"<div style=\"font-size:20px\">" +
                    $"<div>Hi {username},</div>" +
                    $"<br>" +
                    $"<div>looks like you having problems logging in.</div>" +
                    $"<div>Please reset your password by <a href='{ressetPasswordLink}'>clicking here</a>.</div>" +
                    $"<br>" +
                    $"<div>Sincerely Yours,</div>" +
                    $"<div>LetsSport Team</div>" +
                    $"</div>";
        }

        public static string GetEventCreationHtml(string username, string eventName, string sport, string date, string startingHour)
        {
            return $"<div style=\"font-size:20px\">" +
                   $"<div>Hi {username},</div>" +
                   $"<br>" +
                   $"<div>you just created new {sport} event: \"{eventName}\" on {date} at {startingHour}.</div>" +
                   $"<div>You can always edit, update or invite people to your event. Have fun!</div>" +
                   $"<br>" +
                   $"<div>Sincerely Yours,</div>" +
                   $"<div>LetsSport Team</div>" +
                   $"</div>";
        }
    }
}
