namespace LetsSport.Services.Messaging
{
    using System;

    using LetsSport.Common;
    using LetsSport.Services.Models;

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
                 $"<div>you joined successfully {eventObject.Sport} \"{eventObject.Name}\" event orginized by " +
                 $"{eventObject.Orginizer}.</div>" +
                 $"<div>It will be held on {eventObject.Date.ToString(GlobalConstants.DefaultDateFormat)} at " +
                 $"{eventObject.Time.ToString(GlobalConstants.DefaultTimeFormat)} in \"{eventObject.Arena}\" Arena.</div>" +
                 $"<div>Have fun!</div>" +
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
                 $"{eventObject.Time.ToString(GlobalConstants.DefaultTimeFormat)} in \"{eventObject.Arena}\" Arena.</div>" +
                 $"<div>You can always come back and join it again!</div>" +
                 $"<br>" +
                 $"<div>Sincerely Yours,</div>" +
                 $"<div>LetsSport Team</div>" +
                 $"</div>";
        }

        public static string GetCancelEventHtml(string username, string sport, string name, DateTime date)
        {
            return $"<div style=\"font-size:20px\">" +
                 $"<div>Hi {username},</div>" +
                 $"<br>" +
                 $"<div>we are sorry to hear you canceled {sport} event: \"{name}\" on {date.ToString(GlobalConstants.DefaultDateFormat)}." +
                 $"<div>You can always come back and recreate it again!</div>" +
                 $"<br>" +
                 $"<div>Sincerely Yours,</div>" +
                 $"<div>LetsSport Team</div>" +
                 $"</div>";
        }

        public static string GetEventCanceledHtml(string userName, string eventAdmin, string sport, string name, DateTime date)
        {
            return $"<div style=\"font-size:20px\">" +
                $"<div>Hi {userName},</div>" +
                $"<br>" +
                $"<div>we are sorry to say {eventAdmin} decided to cancel {sport} \"{name}\" event for {date.ToString(GlobalConstants.DefaultDateFormat)}." +
                $"<div>You can taka a look on other posted events and join a new one you like!</div>" +
                $"<br>" +
                $"<div>Sincerely Yours,</div>" +
                $"<div>LetsSport Team</div>" +
                $"</div>";
        }

        public static string GetChangedStatusHtml(string userName, string sport, string name, DateTime date, string status)
        {
            return $"<div style=\"font-size:20px\">" +
                $"<div>Hi {userName},</div>" +
                $"<br>" +
                $"<div>the status of {sport} {name} event which will be held on {date.ToString(GlobalConstants.DefaultDateFormat)} has changed to {status}." +
                $"<br>" +
                $"<div>Sincerely Yours,</div>" +
                $"<div>LetsSport Team</div>" +
                $"</div>";
        }

        public static string GetUserLeftHtml(string userName, string sport, string name, DateTime date, string userLeft)
        {
            return $"<div style=\"font-size:20px\">" +
                $"<div>Hi {userName},</div>" +
                $"<br>" +
                $"<div>we are sorry to say {userLeft} decided to leave {sport} \"{name}\" event for {date.ToString(GlobalConstants.DefaultDateFormat)}." +
                $"<div>You can always invite more users to join the event!</div>" +
                $"<br>" +
                $"<div>Sincerely Yours,</div>" +
                $"<div>LetsSport Team</div>" +
                $"</div>";
        }

        public static string GetUserInvitationHtml(InviteUsersMessagingModel serviceModel, string username, string eventLink)
        {
            return $"<div style=\"font-size:20px\">" +
               $"<div>Hi {username},</div>" +
               $"<br>" +
               $"<div>you have been ivited to join {serviceModel.Sport} {serviceModel.EventName} event orginized by {serviceModel.Username}.</div>" +
               $"<div>It will be held on {serviceModel.Date.ToString(GlobalConstants.DefaultDateFormat)} at " +
               $"{serviceModel.StartingTime.ToString(GlobalConstants.DefaultDateTimeFormat)}</div>" +
               $"Hosting Arena is {serviceModel.ArenaName}</div>" +
               $"<br>" +
               $"You can find more information or join the event here : {eventLink}</div>" +
               $"<br>" +
               $"<div>Sincerely Yours,</div>" +
               $"<div>LetsSport Team</div>" +
               $"</div>";
        }

        public static string GetStatusChangedHtml(string userName, string sport, string name, DateTime date, string status)
        {
            return $"<div style=\"font-size:20px\">" +
               $"<div>Hi {userName},</div>" +
               $"<br>" +
               $"<div>the status of {sport} - {name} event which will be held on {date.ToString(GlobalConstants.DefaultDateFormat)},</div>" +
               $"<div>has been changed to: {status}.</div>" +
               $"<div>Sincerely Yours,</div>" +
               $"<div>LetsSport Team</div>" +
               $"</div>";
        }
    }
}
