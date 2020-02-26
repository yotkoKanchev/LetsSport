namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Messages;

    public interface IMessageService
    {
        Task Create(MessageCreateInputModel inputModel, string userId, string chatRoomId);
    }
}
