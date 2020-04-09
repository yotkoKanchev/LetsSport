namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Admin.Reports;
    using LetsSport.Web.ViewModels.Reports;

    public interface IReportsService
    {
        Task<ReportInputModel> CreateAsync(string reportedUserId, string id, string userName);

        Task AddAsync(string senderId, int abuse, string content, string reportedUserId);

        // Admin
        Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0);

        Task<T> GetByIdAsync<T>(int id);

        Task ArchiveAsync(int id);

        Task<int> GetCountAsync();

        Task<IndexViewModel> FilterAsync(int deletionStatus, int? take = null, int skip = 0);
    }
}
