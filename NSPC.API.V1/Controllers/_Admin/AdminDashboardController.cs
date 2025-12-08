using NSPC.Business;
using NSPC.Common;
using NSPC.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NSPC.Api.Controllers
{
    /// <summary>
    /// Admin Dashboard API
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/admin")]
    [ApiExplorerSettings(GroupName = "Dashboard - Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminDashboardHandler _adminDashboardHandler;
        private readonly IAttachmentHandler _attachmentHandler;
        public AdminDashboardController(IConfiguration configuration,
            IAdminDashboardHandler adminDashboardHandler,
            IAttachmentHandler attachmentHandler)
        {
            _configuration = configuration;
            _adminDashboardHandler = adminDashboardHandler;
            _attachmentHandler = attachmentHandler;
        }

        /// <summary>
        /// Get admin dashboard info
        /// </summary>
        /// <returns></returns>
        [HttpGet, Allow(RoleConstants.AdminRoleCode), Authorize, Route("info")]
        [ProducesResponseType(typeof(Response<AdminDashboardViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardInfo()
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            var partnerInfo = await _adminDashboardHandler.GetAdminInfo();

            return Helper.TransformData(partnerInfo);
        }

        /// <summary>
        /// xóa attachment 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, Allow(RoleConstants.AdminRoleCode), HttpDelete("old-attachment")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOldAttachment()
        {
            return Helper.TransformData(await _attachmentHandler.RemoveOldAttachment());
        }

        /*/// <summary>
        /// Get admin's monthly statistics
        /// </summary>
        /// <returns></returns>
        [HttpGet, Allow(RoleConstants.AdminRoleCode), Authorize, Route("monthly-statistic")]
        [ProducesResponseType(typeof(Response<List<MonthlyStatisticModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthlyStatistic()
        {
            var result = await _adminDashboardHandler.GetMonthlyStatistic();

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Get admin's package statistics
        /// </summary>
        /// <returns></returns>
        [HttpGet, Allow(RoleConstants.AdminRoleCode), Authorize, Route("package-statistic")]
        [ProducesResponseType(typeof(List<PackageStatisticModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPackageStatistics()
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _adminDashboardHandler.GetPackageStatistic();

            return Helper.TransformData(result);
        }*/
    }
}