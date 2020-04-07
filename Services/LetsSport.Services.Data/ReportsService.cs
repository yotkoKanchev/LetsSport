namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Reports;
    using Microsoft.EntityFrameworkCore;

    public class ReportsService : IReportsService
    {
        private readonly IUsersService usersService;
        private readonly IRepository<Report> reportsRepository;

        public ReportsService(IUsersService usersService, IDeletableEntityRepository<Report> reportsRepository)
        {
            this.usersService = usersService;
            this.reportsRepository = reportsRepository;
        }

        public async Task<ReportInputModel> CreateAsync(string reportedUserId, string senderId, string senderUserName)
        {
            var reportedUserUsername = await this.usersService.GetUserNameByUserIdAsync(reportedUserId);

            var viewModel = new ReportInputModel
            {
                SenderUserId = senderId,
                SenderUserName = senderUserName,
                ReportedUserId = reportedUserId,
                ReportedUserUserName = reportedUserUsername,
            };

            return viewModel;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.reportsRepository
                .All()
                .Where(r => r.IsDeleted == false)
                .OrderBy(r => r.Abuse)
                .ThenByDescending(r => r.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id)
        {
            return await this.reportsRepository
                .All()
                .Where(r => r.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(string senderId, int abuse, string content, string reportedUserId)
        {
            var report = new Report
            {
                SenderId = senderId,
                Content = content,
                ReportedUserId = reportedUserId,
                Abuse = (AbuseType)abuse,
            };

            await this.reportsRepository.AddAsync(report);
            await this.reportsRepository.SaveChangesAsync();
        }

        public async Task ArchiveAsync(int id)
        {
            var report = await this.GetReportById(id);
            report.IsDeleted = true;
            this.reportsRepository.Update(report);
            await this.reportsRepository.SaveChangesAsync();
        }

        public int GetCount()
        {
            return this.reportsRepository.All().Count();
        }

        private async Task<Report> GetReportById(int id)
        {
            var report = await this.reportsRepository
                .All()
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                throw new ArgumentException($"Report with ID: {id} does not exists!");
            }

            return report;
        }
    }
}
