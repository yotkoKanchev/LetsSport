namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Reports;

    public interface IReportsService
    {
        ReportInputModel CreateReport(string reportedUserId, string id, string userName);

        Task ReportAsync(string senderId, int abuse, string content, string reportedUserId);

        // Admin
        Task<IEnumerable<T>> GetAllAsync<T>();

        Task<T> GetReportByIdAsync<T>(int id);

        Task ArchiveReportAsync(int id);
    }
}
