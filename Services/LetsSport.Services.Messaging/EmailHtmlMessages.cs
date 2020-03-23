namespace LetsSport.Services.Messaging
{
    using LetsSport.Common;
    using LetsSport.Services.Messaging.Models;

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

        public static string GetContactFormContentHtml(string name, string title, string content)
        {
            return $"<div style=\"font-size:20px\">" +
                  $"<div>{name} sent message trough Contact Form!,</div>" +
                  $"<br>" +
                  $"<div><strong>{title}</strong></div>" +
                  $"<br>" +
                  $"<div>{content}</div>" +
                  $"</div>";
        }

        public static string GetArenaCreationHtml(string username, string name, string sportName)
        {
            return $"<div style=\"font-size:20px\">" +
                   $"<div>Hi {username},</div>" +
                   $"<br>" +
                   $"<div>you just created new {sportName} arena: \"{name}\".</div>" +
                   $"<div>You can always edit, update or delete your arena.</div>" +
                   $"<br>" +
                   $"<div>Sincerely Yours,</div>" +
                   $"<div>LetsSport Team</div>" +
                   $"</div>";
        }

        public static string GetUpdateProfileHtml(string username)
        {
            return $"<div style=\"font-size:20px\">" +
                   $"<div>Hi {username},</div>" +
                   $"<br>" +
                   $"<div>your Profile has been updated successfully.</div>" +
                   $"<br>" +
                   $"<div>Sincerely Yours,</div>" +
                   $"<div>LetsSport Team</div>" +
                   $"</div>";
        }

        public static string GetJoinEventHtml(string username, EventDetailsModel eventObject)
        {
            return $"<div style=\"font-size:20px\">" +
                 $"<div>Hi {username},</div>" +
                 $"<br>" +
                 $"<div>you joined {eventObject.Sport} \"{eventObject.Name}\" event orginized by " +
                 $"{eventObject.Orginizer} successfully.</div>" +
                 $"<div>It will be held on {eventObject.Date.ToString(GlobalConstants.DefaultDateFormat)} at " +
                 $"{eventObject.Time.ToString(GlobalConstants.DefaultTimeFormat)} in {eventObject.Name}.</div>" +
                 $"<div>Have funn!</div>" +
                 $"<br>" +
                 $"<div>Sincerely Yours,</div>" +
                 $"<div>LetsSport Team</div>" +
                 $"</div>";
        }

        public static string GetLeaveEventHtml(string username, EventDetailsModel eventObject)
        {
            return $"<div style=\"font-size:20px\">" +
                 $"<div>Hi {username},</div>" +
                 $"<br>" +
                 $"<div>we are sorry to hear you left {eventObject.Sport} \"{eventObject.Name}\" event orginized by " +
                 $"{eventObject.Orginizer}.</div>" +
                 $"<div>The event will be held on {eventObject.Date.ToString(GlobalConstants.DefaultDateFormat)} at " +
                 $"{eventObject.Time.ToString(GlobalConstants.DefaultTimeFormat)} in {eventObject.Name}.</div>" +
                 $"<div>You can always come back and join the it again!</div>" +
                 $"<br>" +
                 $"<div>Sincerely Yours,</div>" +
                 $"<div>LetsSport Team</div>" +
                 $"</div>";
        }
    }
}
