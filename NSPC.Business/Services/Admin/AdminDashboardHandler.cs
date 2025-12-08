using AutoMapper;
using NSPC.Common;
using NSPC.Data;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using NSPC.Data.Data;

namespace NSPC.Business
{
    public class AdminDashboardHandler : IAdminDashboardHandler
    {
        private readonly IConfiguration _config;
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;

        public AdminDashboardHandler(IConfiguration config, SMDbContext dbContext, IMapper mapper)
        {
            _config = config;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Response<AdminDashboardViewModel>> GetAdminInfo()
        {
            try
            {
                var result = new AdminDashboardViewModel();

                /*// Fetch total amount
                result.TotalCommissionAmount = await _dbContext.subm_Partner_Commission
                    .SumAsync(x => x.CommissionAmount);

                result.TotalIncomeAmount = (decimal) await _dbContext.subm_Partner_Commission
                    .SumAsync(x => x.TotalAmount);

                // Fetch total offers
                result.TotalOffer = await _dbContext.subm_Profile_Offer
                    .CountAsync();

                // Fetch total proposal
                result.TotalProposal = await _dbContext.subm_Profile_Investor_Link
                    .CountAsync();

                // Fetch total profile
                 result.TotalProfile = await _dbContext.subm_Profile
                    .Where(x => x.StatusCode == ProfileStatus.Approved)
                    .CountAsync();

                // Fetch total profile purchased
                result.TotalPurchasedProfile = await _dbContext.subm_Profile
                    .Where(x => x.StatusCode == ProfileStatus.Approved && x.IsPurchased == true)
                    .CountAsync();
*/
                // Fetch active user
                int totalInactiveUser = _dbContext.IdmUser
                    .Where(x => x.IsActive == false || x.IsEmailVerified == false)
                    .Count();

                result.TotalUser = _dbContext.IdmUser
                    .Where(x => x.Level == 1)
                    .Count();

                result.TotalEmailVerifiedUser = _dbContext.IdmUser
                    .Where(x => x.IsEmailVerified == true)
                    .Count();

                result.TotalActiveUser = result.TotalUser - totalInactiveUser;

                return new Response<AdminDashboardViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new Response<AdminDashboardViewModel>(HttpStatusCode.InternalServerError, null, Utils.CreateExceptionResponseError(ex).Message);
            }
        }

       /* public async Task<Response<List<MonthlyStatisticModel>>> GetMonthlyStatistic()
        {
            int numberOfMonth = 12;
            var result = new List<MonthlyStatisticModel>();
            int startMonth = DateTime.Now.Month;

            for (int i = 0; i < numberOfMonth; i++)
            {
                DateTime StartOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1 * i);
                DateTime EndOfMonth = StartOfMonth.AddMonths(1);

                var model = new MonthlyStatisticModel();

                model.Year = StartOfMonth.Year;
                model.Month = StartOfMonth.Month;

                *//*var query = _dbContext.ub_Profile
                        .Include(x => x.IdmUser)
                        .Where(x => x.IsPurchased == true && x.PackageId.HasValue);

                // Fetch total amount
                model.CommissionAmount = await _dbContext.ub_Partner_Commission
                    .Where(x => x.CreatedOnDate >= StartOfMonth && x.CreatedOnDate < EndOfMonth)
                    .SumAsync(x => x.TotalAmount);

                // Fetch total offers
                model.OfferCount = await _dbContext.ub_Profile_Offer
                    .Include(x => x.CreatedByUser)
                    .Where(x => x.CreatedOnDate >= StartOfMonth && x.CreatedOnDate < EndOfMonth)
                    .CountAsync();

                // Fetch total proposal
                model.ProposalCount = await _dbContext.ub_Profile_Investor_Link
                    .Include(x => x.CreatedByUser)
                    .Where(x => x.CreatedOnDate >= StartOfMonth && x.CreatedOnDate < EndOfMonth)
                    .CountAsync();

                // Fetch total proposal
                model.ProfileCount = await _dbContext.ub_Profile
                    .Include(x => x.IdmUser)
                    .Where(x => x.CreatedOnDate >= StartOfMonth && x.CreatedOnDate < EndOfMonth)
                    .CountAsync();*//*

                result.Add(model);
            }

            return new Response<List<MonthlyStatisticModel>>(result);
        }*/

        /*public async Task<Response<List<PackageStatisticModel>>> GetPackageStatistic()
        {
            // Fetch total proposal
            var query = _dbContext.ub_Profile
                .Include(x => x.IdmUser)
                .Where(x => x.IsPurchased == true && x.PackageId.HasValue);

            var allPurchasedProfiles = await query.Select(x => new { x.PackageId, x.Id })
                .ToListAsync();

            var result = new List<PackageStatisticModel>();

            var allPackages = await _dbContext.ub_Package
                .Select(x => new { x.Id, x.Type }).ToListAsync();

            var level1Packages = allPackages.Where(x => x.Type == 1).Select(x => x.Id).ToList();
            var level2Packages = allPackages.Where(x => x.Type == 2).Select(x => x.Id).ToList();
            var level3Packages = allPackages.Where(x => x.Type == 3).Select(x => x.Id).ToList();
            var level4Packages = allPackages.Where(x => x.Type == 4).Select(x => x.Id).ToList();

            result.Add(new PackageStatisticModel
            {
                Type = 1,
                Name = "Basic",
                BackgroundColor = "#fff",
                Count = allPurchasedProfiles.Where(x => level1Packages.Contains(x.PackageId.Value)).Count()
            });

            result.Add(new PackageStatisticModel
            {
                Type = 2,
                Name = "Silver",
                BackgroundColor = "#f2f2f2",
                Count = allPurchasedProfiles.Where(x => level2Packages.Contains(x.PackageId.Value)).Count()
            });

            result.Add(new PackageStatisticModel
            {
                Type = 3,
                Name = "Gold",
                BackgroundColor = "#d6b002",
                Count = allPurchasedProfiles.Where(x => level3Packages.Contains(x.PackageId.Value)).Count()
            });

            result.Add(new PackageStatisticModel
            {
                Type = 4,
                Name = "Platinum",
                BackgroundColor = "#11a6a1",
                Count = allPurchasedProfiles.Where(x => level4Packages.Contains(x.PackageId.Value)).Count()
            });
            return new Response<List<PackageStatisticModel>>(result);

        }*/

        
    }
}