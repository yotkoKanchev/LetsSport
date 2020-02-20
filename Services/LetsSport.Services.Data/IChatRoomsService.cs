namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    public interface IChatRoomsService
    {
        Task<string> Create();
    }
}
