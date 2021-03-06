﻿namespace LetsSport.Services.Data.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Users;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Reports;
    using LetsSport.Web.ViewModels.Reports;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;

    public class ReportsService : IReportsService
    {
        private readonly IUsersService usersService;
        private readonly IDeletableEntityRepository<Report> reportsRepository;

        public ReportsService(IUsersService usersService, IDeletableEntityRepository<Report> reportsRepository)
        {
            this.usersService = usersService;
            this.reportsRepository = reportsRepository;
        }

        public async Task<ReportInputModel> CreateAsync(string reportedUserId, string senderId)
        {
            var reportedUserUsername = await this.usersService.GetUserNameByUserIdAsync(reportedUserId);
            var senderUserName = await this.usersService.GetUserNameByUserIdAsync(senderId);
            var viewModel = new ReportInputModel
            {
                ReportedUserId = reportedUserId,
                ReportedUserUserName = reportedUserUsername,
                SenderUserId = senderId,
                SenderUserName = senderUserName,
            };

            return viewModel;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.reportsRepository
                .AllWithDeleted()
                .OrderBy(r => r.Abuse)
                .ThenByDescending(r => r.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id)
            => await this.reportsRepository
                .AllWithDeleted()
                .Where(r => r.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

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
            var report = await this.GetReportByIdAsync(id);
            this.reportsRepository.Delete(report);
            await this.reportsRepository.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
            => await this.reportsRepository
                .All()
                .CountAsync();

        public async Task<IndexViewModel> FilterAsync(int deletionStatus, int? take = null, int skip = 0)
        {
            var query = this.reportsRepository.AllWithDeleted();

            if (deletionStatus != 0)
            {
                if (deletionStatus == 1)
                {
                    query = query
                        .Where(c => c.IsDeleted == false);
                }
                else if (deletionStatus == 2)
                {
                    query = query
                        .Where(c => c.IsDeleted == true);
                }
            }

            var resultCount = await query.CountAsync();

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && resultCount > take)
            {
                query = query.Take(take.Value);
            }

            var viewModel = new IndexViewModel
            {
                ResultCount = resultCount,
                Reports = await query
                    .OrderBy(c => c.Abuse)
                    .ThenByDescending(c => c.CreatedOn)
                    .To<ReportInfoInputModel>().ToListAsync(),
                Filter = new SimpleModelsFilterBarViewModel
                {
                    DeletionStatus = deletionStatus,
                },
            };

            return viewModel;
        }

        private async Task<Report> GetReportByIdAsync(int id)
        {
            var report = await this.reportsRepository
                .AllWithDeleted()
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                throw new ArgumentException(string.Format(ReportInvalidIdErrorMessage, id));
            }

            return report;
        }
    }
}
