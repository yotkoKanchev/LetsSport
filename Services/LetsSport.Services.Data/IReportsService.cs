namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Reports;

    public interface IReportsService
    {
        ReportInputModel Create(string reportedUserId, string id, string userName);

        Task AddAsync(string senderId, int abuse, string content, string reportedUserId);

        // Admin
        Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0);

        Task<T> GetByIdAsync<T>(int id);

        Task ArchiveAsync(int id);

        int GetCount();
    }
}
