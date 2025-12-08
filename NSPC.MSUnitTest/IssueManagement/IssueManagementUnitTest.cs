using NSPC.Business.Services;
using NSPC.Business;
using NSPC.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NSPC.Business.AutoMapper;
using NSPC.Business.Services.ConstructionActitvityLog;
using NSPC.Common;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace SalesMngt.MSUnitTest.IssueManagement
{
    [TestClass]
    public class IssueManagementUnitTest
    {
        SMDbContext _dbContext;
        IAttachmentHandler _attachmentHandler;
        IIssueActivityLogHandler _issueActivityLogHandler;
        IIssueManagementHandler _issueManagementHandler;

        public IssueManagementUnitTest()
        {
            _dbContext = new SMDbContext();
            var mapper = new Mapper(AutoMapperConfig.RegisterMappings());
            _attachmentHandler = new AttachmentHandler(_dbContext);
            _issueActivityLogHandler = new IssueActivityLogHandler(_dbContext, null, mapper, _attachmentHandler);
            _issueManagementHandler = new IssueManagementHandler(_dbContext, null, mapper,_attachmentHandler, _issueActivityLogHandler, null, null); 
        }
        #region Testcase thêm mới vướng mắc
        [TestMethod]
        public async Task CreateIssueTestCase()
        { 
            try
            {
                var currentUser = new Helper.RequestUser(Guid.Parse("e878abbf-6c79-4142-b850-459d7633100e"), "admin@gmail.com");
                var issuePayload = new IssueManagementCreateUpdateModel()
                {
                    Code = null,
                    Id = Guid.NewGuid(),
                    PriorityLevel = "MEDIUM",
                    ConstructionId = Guid.Parse("dbc526b3-6216-42ec-a5a3-f0ef510c9ff1"),
                    ExpiryDate = DateTime.Now,
                    Status = StatusIssue.WAIT_PROCESSING,
                    UserId = Guid.Parse("624b7648-934c-487c-a37d-44b1acc7378a"),
                    Content = "Nội dung chính",
                    Description = "Mô tả"
                };
                var result = await _issueManagementHandler.Create(issuePayload, currentUser);
                if (result.IsSuccess == true)
                {
                    Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
                }
                else
                {
                    Assert.Fail("Thêm mới thất bại");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Testcase cập nhật vướng mắc
        [TestMethod]
        public async Task UpdateIssueTestCase()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("e878abbf-6c79-4142-b850-459d7633100e"), "admin@gmail.com");
            var isssuePayload = new IssueManagementCreateUpdateModel()
            {
                Code = null,
                Id = Guid.NewGuid(),
                PriorityLevel = "MEDIUM",
                ConstructionId = Guid.Parse("dbc526b3-6216-42ec-a5a3-f0ef510c9ff1"),
                ExpiryDate = DateTime.Now,
                Status = StatusIssue.WAIT_PROCESSING,
                UserId = Guid.Parse("624b7648-934c-487c-a37d-44b1acc7378a"),
                Content = "Nội dung chính",
                Description = "Mô tả"
            };
            var result = await _issueManagementHandler.Update(Guid.Parse("7a441267-fdb2-4fc0-acdf-6863100e3ab8"), isssuePayload, currentUser);
            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess, "Cập nhật thành công");
            }
            else
            {
                Assert.Fail("Cập nhật thất bại");
            }
        }
        #endregion

        #region Testcase huỷ vướng mắc
        [TestMethod]
        public async Task DeactiveIssue() 
        {
            var entity =  _dbContext.sm_IssueManagement.Where(x => x.Id == Guid.Parse("7a441267-fdb2-4fc0-acdf-6863100e3ab8")).FirstOrDefault();
            var result = await _issueManagementHandler.DeactiveIssueAsync(Guid.Parse("7a441267-fdb2-4fc0-acdf-6863100e3ab8"), "HUY");
            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess, "Huỷ vướng mắc thành công");
            }
            else
            {
                Assert.Fail("Huỷ vướng mắc thất bại");
            }
        }


        #endregion

        #region Testcase giải quyết vướng mắc
        [TestMethod]
        public async Task ResolveIssue()
        {
            var resolvePayload = new ResolveModel()
            {
                ContentResolve = "Nội dung đã giải quyết"
            };
            var result = await _issueManagementHandler.ResolveIssueAsync(Guid.Parse("7a441267-fdb2-4fc0-acdf-6863100e3ab8"), resolvePayload);
            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess, "Xử lý vướng mắc thành công");
            }
            else
            {
                Assert.Fail("Xử lý vướng mắc thất bại");
            }
        }


        #endregion

        #region Testcase mở lại vướng mắc
        [TestMethod]
        public async Task ReopenIssue()
        {
           
            var result = await _issueManagementHandler.ReopenIssueAsync(Guid.Parse("7a441267-fdb2-4fc0-acdf-6863100e3ab8"), "Mở lại vướng mắc");
            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess, "Mở lại vướng mắc thành công");
            }
            else
            {
                Assert.Fail("Mở lại vướng mắc thất bại");
            }
        }


        #endregion
    }
}
