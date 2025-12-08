using AutoMapper;
using NSPC.Business.AutoMapper;
using NSPC.Business.Services;
using NSPC.Common;
using NSPC.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.ConstructionActitvityLog;    
using NSPC.Business;
using NSPC.Data;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.ExecutionTeams;
using NSPC.Business.Services.ConstructionWeekReport;

namespace SalesMngt.MSUnitTest.Construction
{
    [TestClass]
    public class ConstructionCreateUpdateUnitTest
    {
        SMDbContext _dbContext;
        IAttachmentHandler _attachmentHandler;
        IConstructionActivityLogHandler _constructionActivityLogHandler;
        IConstructionHandler _constructionHandler;
        IConstructionWeekReportHandler _constructionWeekReportHandler;

        public ConstructionCreateUpdateUnitTest()
        {
            _dbContext = new SMDbContext();
            var mapper = new Mapper(AutoMapperConfig.RegisterMappings());
            _attachmentHandler = new AttachmentHandler(_dbContext); 
            _constructionActivityLogHandler = new ConstructionActivityLogHandler(_dbContext, null, mapper, _attachmentHandler);
            _constructionHandler = new ConstructionHandler(_dbContext, null, mapper, _constructionActivityLogHandler, null);
            _constructionWeekReportHandler = new ConstructionWeekReportHandler(_dbContext, null, mapper, _attachmentHandler, _constructionActivityLogHandler);
        }
        
        #region Testcase Công trình
        
        #region Testcase Thêm mới công trình
        [TestMethod]
        public async Task CreateConstructionTestCase()
        {
            try
            {
                var currentUser = new Helper.RequestUser(Guid.Parse("e878abbf-6c79-4142-b850-459d7633100e"), "admin@gmail.com");
                var executionTeams =  new List<ExecutionTeamsCreateModel>();

                executionTeams.Add(new ExecutionTeamsCreateModel()
                {
                    EmployeeId = Guid.Parse("9500f199-e6bc-4c4b-955a-69e3323c0b04"),
                    UserType = "participants"
                });
                    
                executionTeams.Add(new ExecutionTeamsCreateModel()
                {
                    EmployeeId = Guid.Parse("5aa1616c-74d7-4164-9e0f-1894fc5ec320"),
                    UserType = "participants"
                });
                    
                executionTeams.Add(new ExecutionTeamsCreateModel()
                {
                    EmployeeId = Guid.Parse("5aa1616c-74d7-4164-9e0f-1894fc5ec320"),
                    UserType = "followers"
                });
                
        
                var constructionPayload = new ConstructionCreateUpdateModel()
                {
                    Code = null,
                    Name = "Công trình STARLAKE Hồ Tây UNITTEST",
                    VoltageTypeCode = "VT-0002",
                    OwnerTypeCode = "CĐT",
                    InvestorId = Guid.Parse("c5002d62-9511-4607-87d1-789b0d499e8e"),
                    ConstructionTemplateId = Guid.Parse("7d5efa8a-20e2-42c7-abb8-4550085e975f"),
                    DeliveryDate = DateTime.Now,
                    PriorityCode = "1",
                    CompletionByInvestor = String.Empty,
                    CompletionByCompany = String.Empty,
                    StatusCode = "IS_DESIGNING",
                    ExecutionTeams = executionTeams,
                    ExecutionStatusCode = "IS_DESIGNING",
                    DocumentStatusCode = "NOT_APPROVE",
                    Note = "Note",
                };
                
                var result = await _constructionHandler.Create(constructionPayload, currentUser);
        
                // nếu tạo thành công thì trả về thông báo "Thêm mới thành công" và IsSuccess = true
                if (result.IsSuccess == true)
                {
                    Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
                }
                else
                {
                    Assert.Fail("Thêm mới không thành công");
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion
        
        #region Testcase Cập nhật công trình
        [TestMethod]
        public async Task UpdateConstructionTestCase()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("e878abbf-6c79-4142-b850-459d7633100e"), "admin@gmail.com");
                var constructionEntity =  await _dbContext.sm_Construction
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();
                 
                var executionTeams =  new List<ExecutionTeamsCreateModel>();

                executionTeams.Add(new ExecutionTeamsCreateModel()
                {
                    EmployeeId = Guid.Parse("9500f199-e6bc-4c4b-955a-69e3323c0b04"),
                    UserType = "participants"
                });
                    
                executionTeams.Add(new ExecutionTeamsCreateModel()
                {
                    EmployeeId = Guid.Parse("5aa1616c-74d7-4164-9e0f-1894fc5ec320"),
                    UserType = "participants"
                });
                    
                executionTeams.Add(new ExecutionTeamsCreateModel()
                {
                    EmployeeId = Guid.Parse("5aa1616c-74d7-4164-9e0f-1894fc5ec320"),
                    UserType = "followers"
                });
        
                var constructionPayload = new ConstructionCreateUpdateModel()
                {
                    Code = constructionEntity != null ? constructionEntity.Code : null,
                    Name = "Công trình STARLAKE Hồ Tây UNITTEST",
                    VoltageTypeCode = "VT-0002",
                    OwnerTypeCode = "CĐT",
                    InvestorId = Guid.Parse("c5002d62-9511-4607-87d1-789b0d499e8e"),
                    ConstructionTemplateId = Guid.Parse("7d5efa8a-20e2-42c7-abb8-4550085e975f"),
                    DeliveryDate = DateTime.Now,
                    PriorityCode = "1",
                    CompletionByInvestor = String.Empty,
                    CompletionByCompany = String.Empty,
                    StatusCode = "IS_DESIGNING",
                    ExecutionTeams = executionTeams,
                    ExecutionStatusCode = "IS_DESIGNING",
                    DocumentStatusCode = "NOT_APPROVE",
                    Note = "Note",
                };
        
                if (constructionEntity != null)
                {
                    var result = await _constructionHandler.Update(constructionEntity != null 
                        ? constructionEntity.Id 
                        : Guid.NewGuid(), constructionPayload, currentUser);
        
                    // nếu tạo thành công thì trả về thông báo "Thêm mới thành công" và IsSuccess = true
                    if (result.IsSuccess == true)
                    {
                        Assert.IsTrue(result.IsSuccess, "Cập nhật thành công");
                    }
                    else
                    {
                        Assert.Fail("Cập nhật không thành công");
                    }
                }
        }
        #endregion
        
        #region Testcase Xoá công trình
        [TestMethod]
        public async Task DeleteConstructionTestCase()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("e878abbf-6c79-4142-b850-459d7633100e"), "admin@gmail.com");
               var constructionEntity =  await _dbContext.sm_Construction
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();
            
        
                if (constructionEntity != null)
                {
                    var result = await _constructionHandler.Delete(constructionEntity != null 
                        ? constructionEntity.Id 
                        : Guid.NewGuid(), currentUser);
        
                    // nếu tạo thành công thì trả về thông báo "Thêm mới thành công" và IsSuccess = true
                    if (result.IsSuccess == true)
                    {
                        Assert.IsTrue(result.IsSuccess, "Xoá thành công");
                    }
                    else
                    {
                        Assert.Fail("Xoá không thành công");
                    }
                }
        }
        #endregion
        
        #endregion
        
        #region Testcase Báo cáo tuần trong công trình
        #region Thêm mới báo cáo tuần
        public async Task CreateWeekReportTestCase()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("e878abbf-6c79-4142-b850-459d7633100e"), "admin@gmail.com");
            var constructionEntity =  await _dbContext.sm_Construction
                .OrderByDescending(x => x.CreatedOnDate)
                .FirstOrDefaultAsync();

            var listFileAttachments = new List<AttachmentViewModel>();

            var payload = new ConstructionWeekReportCreateModel()
            {
                Title = $"Báo cáo công việc tuần tạo ngày {DateTime.Now} ",
                LastWeekPlan = "<p>Làm văn bản gửi BA1 về hồ sơ thỏa thuận các vị trí trong hành lang bảo vệ hồ chưa thủy điện Lai Châu.<br>Trao đổi với BA1 làm văn bản gửi Công ty Thủy điện Sơn La.<br></p>",
                ProcessResult = "\"<p>Làm văn bản gửi BA1 về hồ sơ thỏa thuận các vị trí trong hành lang bảo vệ hồ chưa thủy điện Lai Châu.<br>Trao đổi với BA1 làm văn bản gửi Công ty Thủy điện Sơn La.<br></p>",
                NextWeekPlan = "\"<p>Làm văn bản gửi BA1 về hồ sơ thỏa thuận các vị trí trong hành lang bảo vệ hồ chưa thủy điện Lai Châu.<br>Trao đổi với BA1 làm văn bản gửi Công ty Thủy điện Sơn La.<br></p>",
                ConstructionId =  constructionEntity != null ? constructionEntity.Id : Guid.NewGuid(),
                FileAttachments =  null,
                StatusCode = "RIGHT_ON_PLAN"
            };
            
            var result = await _constructionWeekReportHandler.Create(payload, currentUser);
        
            // nếu tạo thành công thì trả về thông báo "Thêm mới thành công" và IsSuccess = true
            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess, "Tạo mới thành công");
            }
            else
            {
                Assert.Fail("Tạo mới thành công");
            }
        }
        #endregion
        #endregion
        
    }
}

