using NSPC.Business;
using NSPC.Data;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using NSPC.Data.Data;
using static NSPC.Common.Helper;
using NSPC.Business.AutoMapper;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SalesMngt.MSUnitTest.TaskManagement
{
    [TestClass]
    public class TaskManagementHandlerTests
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly TaskManagementHandler _handler;
        private readonly RequestUser _currentUser;
        private readonly IAttachmentHandler _attachmentHandler;


        public TaskManagementHandlerTests()
        {
            _mapper = AutoMapperConfig.RegisterMappings().CreateMapper();
            _dbContext = new SMDbContext();
            _httpContextAccessor = new HttpContextAccessor();
            _handler = new TaskManagementHandler(_dbContext, _httpContextAccessor, _mapper, _attachmentHandler);
            _currentUser = new RequestUser { UserId = new Guid("e878abbf-6c79-4142-b850-459d7633100e"), UserName = "admin@gmail.com", TenantId = null };
        }

        #region Test cases Công việc (TaskManagement)       

        [TestMethod]
        public async Task CreateSuccessTaskAsync()
        {
            var model = new TaskManagementCreateUpdateModel { Title = "New Task", DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

            var result = await _handler.CreateTask(model, _currentUser);

            Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
        }

        [TestMethod]
        public async Task GetByIdTask_ShouldReturnTask_WhenFound()
        {
            var taskId = Guid.NewGuid();
            var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

            _dbContext.sm_TaskManagement.Add(taskEntity);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.GetByIdTask(taskId, _currentUser);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Test Task", result.Data.Title);
        }

        [TestMethod]
        public async Task UpdateTask_ShouldReturnSuccess_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();
            var model = new TaskManagementCreateUpdateModel { Title = "Updated Task", DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };
            var existingTask = new sm_TaskManagement { Id = taskId, Title = "Old Task", DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

            _dbContext.sm_TaskManagement.Add(existingTask);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.UpdateTask(taskId, model, _currentUser);

            Assert.IsTrue(result.IsSuccess, "Cập nhật thành công");
        }

        [TestMethod]
        public async Task DeleteTask_ShouldReturnSuccess_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();
            var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Old Task", DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

            _dbContext.sm_TaskManagement.Add(taskEntity);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.DeleteTask(taskId, _currentUser);

            Assert.IsTrue(result.IsSuccess, "Xóa công việc thành công");
        }

        #endregion

        #region Test cases Giao việc (TaskManagementAssignee)

        [TestMethod]
        public async Task CreateTaskAssignee_ShouldReturnSuccess()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var model = new TaskManagementAssigneeCreateUpdateModel { UserId = _currentUser.UserId, TaskManagementId = taskId };

                var result = await _handler.CreateTaskAssignee(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }
            else
            {
                var model = new TaskManagementAssigneeCreateUpdateModel { UserId = _currentUser.UserId, TaskManagementId = task.Id };

                var result = await _handler.CreateTaskAssignee(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }
        }

        [TestMethod]
        public async Task GetByIdTaskAssignee_ShouldReturnTaskAssignee_WhenFound()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var assigneeId = Guid.NewGuid();
                var assigneeEntity = new sm_TaskManagementAssignee { Id = assigneeId, TaskManagementId = taskId, TenantId = _currentUser.TenantId };

                _dbContext.sm_TaskManagementAssignee.Add(assigneeEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.GetByIdTaskAssignee(assigneeId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
            }
            else
            {
                var assigneeId = Guid.NewGuid();
                var assigneeEntity = new sm_TaskManagementAssignee { Id = assigneeId, TaskManagementId = task.Id, TenantId = _currentUser.TenantId };

                _dbContext.sm_TaskManagementAssignee.Add(assigneeEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.GetByIdTaskAssignee(assigneeId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
            }

        }

        [TestMethod]
        public async Task DeleteTaskAssignee_ShouldReturnSuccess_WhenTaskAssigneeExists()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var assigneeId = Guid.NewGuid();
                var assigneeEntity = new sm_TaskManagementAssignee { Id = assigneeId, TaskManagementId = taskId, TenantId = _currentUser.TenantId, UserId = _currentUser.UserId };

                _dbContext.sm_TaskManagementAssignee.Add(assigneeEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.DeleteTaskAssignee(assigneeId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
            }
            else
            {
                var assigneeId = Guid.NewGuid();
                var assigneeEntity = new sm_TaskManagementAssignee { Id = assigneeId, TaskManagementId = task.Id, TenantId = _currentUser.TenantId, UserId = _currentUser.UserId };

                _dbContext.sm_TaskManagementAssignee.Add(assigneeEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.DeleteTaskAssignee(assigneeId, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Xóa giao việc thành công");
            }

        }

        #endregion

        #region Test cases Lịch sử công việc (TaskManagementHistory)

        [TestMethod]
        public async Task CreateTaskHistory_ShouldReturnSuccess()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var model = new TaskManagementHistoryCreateUpdateModel { TaskManagementId = taskId, Action = "New History Entry" };

                var result = await _handler.CreateTaskHistory(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }
            else
            {
                var model = new TaskManagementHistoryCreateUpdateModel { TaskManagementId = task.Id, Action = "New History Entry" };

                var result = await _handler.CreateTaskHistory(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }

        }

        [TestMethod]
        public async Task GetByIdTaskHistory_ShouldReturnHistory_WhenFound()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var historyId = Guid.NewGuid();
                var historyEntity = new sm_TaskManagementHistory { Id = historyId, TaskManagementId = taskId, Action = "Test History", TenantId = _currentUser.TenantId };

                _dbContext.sm_TaskManagementHistory.Add(historyEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.GetByIdTaskHistory(historyId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Test History", result.Data.Action);
            }
            else
            {
                var historyId = Guid.NewGuid();
                var historyEntity = new sm_TaskManagementHistory { Id = historyId, TaskManagementId = task.Id, Action = "Test History", TenantId = _currentUser.TenantId };

                _dbContext.sm_TaskManagementHistory.Add(historyEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.GetByIdTaskHistory(historyId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Test History", result.Data.Action);
            }

        }

        [TestMethod]
        public async Task GetPageTaskHistory_ShouldReturnPagedResult()
        {
            var query = new TaskManagementHistoryQueryModel();

            var result = await _handler.GetPageTaskHistory(query, _currentUser);

            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Test cases Mốc công việc (TaskManagementMileStone)

        [TestMethod]
        public async Task CreateTaskMileStone_ShouldReturnSuccess()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var model = new TaskManagementMileStoneCreateUpdateModel { TaskManagementId = taskId, Title = "New Milestone", StartDate = DateTime.Now, DueDate = DateTime.Now.AddDays(5) };

                var result = await _handler.CreateTaskMileStone(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }
            else
            {
                var model = new TaskManagementMileStoneCreateUpdateModel { TaskManagementId = task.Id, Title = "New Milestone", StartDate = DateTime.Now, DueDate = DateTime.Now.AddDays(5) };

                var result = await _handler.CreateTaskMileStone(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }

        }

        [TestMethod]
        public async Task GetByIdTaskMileStone_ShouldReturnMileStone_WhenFound()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();
               

                var mileStoneEntity = new TaskManagementMileStoneCreateUpdateModel {TaskManagementId = taskId, Title = "Test Milestone", DueDate = DateTime.Now.AddDays(2), StartDate = DateTime.Now };

                var milestone = await _handler.CreateTaskMileStone(mileStoneEntity, _currentUser);

                var result = await _handler.GetByIdTaskMileStone(milestone.Data.Id, _currentUser);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Test Milestone", result.Data.Title);
            }
            else
            {
                var mileStoneEntity = new TaskManagementMileStoneCreateUpdateModel { TaskManagementId = task.Id, Title = "Test Milestone", DueDate = DateTime.Now.AddDays(2), StartDate = DateTime.Now };

                var milestone = await _handler.CreateTaskMileStone(mileStoneEntity, _currentUser);

                var result = await _handler.GetByIdTaskMileStone(milestone.Data.Id, _currentUser);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Test Milestone", result.Data.Title);
            }

        }

        [TestMethod]
        public async Task DeleteTaskMileStone_ShouldReturnSuccess_WhenMileStoneExists()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var mileStoneId = Guid.NewGuid();

                var mileStoneEntity = new sm_TaskManagementMileStone { Id = mileStoneId, TaskManagementId = taskId };

                _dbContext.sm_TaskManagementMileStone.Add(mileStoneEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.DeleteTaskMileStone(mileStoneId, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Xóa milestone thành công");
            }
            else
            {
                var mileStoneId = Guid.NewGuid();

                var mileStoneEntity = new sm_TaskManagementMileStone { Id = mileStoneId, TaskManagementId = task.Id };

                _dbContext.sm_TaskManagementMileStone.Add(mileStoneEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.DeleteTaskMileStone(mileStoneId, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Xóa milestone thành công");
            }

        }

        #endregion

        #region Test cases Bình luận công việc (TaskManagementComment)

        [TestMethod]
        public async Task CreateTaskComment_ShouldReturnSuccess()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var model = new TaskManagementCommentCreateUpdateModel { TaskManagementId = Guid.NewGuid(), Content = "New Comment" };

                var result = await _handler.CreateTaskComment(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }
            else
            {
                var model = new TaskManagementCommentCreateUpdateModel { TaskManagementId = task.Id, Content = "New Comment" };

                var result = await _handler.CreateTaskComment(model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Thêm mới thành công");
            }

        }

        [TestMethod]
        public async Task GetByIdTaskComment_ShouldReturnComment_WhenFound()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var commentId = Guid.NewGuid();

                var commentEntity = new sm_TaskManagementComment { Id = commentId, TaskManagementId = taskId, Content = "Test Comment", TenantId = _currentUser.TenantId };

                _dbContext.sm_TaskManagementComment.Add(commentEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.GetByIdTaskComment(commentId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Test Comment", result.Data.Content);
            }
            else
            {
                var commentId = Guid.NewGuid();

                var commentEntity = new sm_TaskManagementComment { Id = commentId, TaskManagementId = task.Id, Content = "Test Comment", TenantId = _currentUser.TenantId };

                _dbContext.sm_TaskManagementComment.Add(commentEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.GetByIdTaskComment(commentId, _currentUser);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Test Comment", result.Data.Content);
            }

        }

        [TestMethod]
        public async Task UpdateTaskComment_ShouldReturnSuccess_WhenCommentExists()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var commentId = Guid.NewGuid();
                var model = new TaskManagementCommentCreateUpdateModel { TaskManagementId = taskId, Content = "Updated Comment" };

                var existingComment = new sm_TaskManagementComment { Id = commentId, Content = "Old Comment", TaskManagementId = taskId };

                _dbContext.sm_TaskManagementComment.Add(existingComment);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.UpdateTaskComment(commentId, model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Cập nhật thành công");
            }
            else
            {

                var commentId = Guid.NewGuid();
                var model = new TaskManagementCommentCreateUpdateModel { TaskManagementId = task.Id, Content = "Updated Comment" };

                var existingComment = new sm_TaskManagementComment { Id = commentId, Content = "Old Comment", TaskManagementId = task.Id };

                _dbContext.sm_TaskManagementComment.Add(existingComment);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.UpdateTaskComment(commentId, model, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Cập nhật thành công");
            }

        }

        [TestMethod]
        public async Task DeleteTaskComment_ShouldReturnSuccess_WhenCommentExists()
        {
            var task = _dbContext.sm_TaskManagement.AsQueryable().FirstOrDefault();
            if (task == null)
            {
                var taskId = Guid.NewGuid();
                var taskEntity = new sm_TaskManagement { Id = taskId, Title = "Test Task", TenantId = _currentUser.TenantId, DueDate = DateTime.Now.AddDays(7), StartDate = DateTime.Now };

                _dbContext.sm_TaskManagement.Add(taskEntity);
                await _dbContext.SaveChangesAsync();

                var commentId = Guid.NewGuid();
                var commentEntity = new sm_TaskManagementComment { Id = commentId, TaskManagementId = taskId };

                _dbContext.sm_TaskManagementComment.Add(commentEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.DeleteTaskComment(commentId, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Xóa bình luận thành công");
            }
            else
            {
                var commentId = Guid.NewGuid();
                var commentEntity = new sm_TaskManagementComment { Id = commentId, TaskManagementId = task.Id };

                _dbContext.sm_TaskManagementComment.Add(commentEntity);
                await _dbContext.SaveChangesAsync();

                var result = await _handler.DeleteTaskComment(commentId, _currentUser);

                Assert.IsTrue(result.IsSuccess, "Xóa bình luận thành công");
            }

        }

        #endregion
    }
}
