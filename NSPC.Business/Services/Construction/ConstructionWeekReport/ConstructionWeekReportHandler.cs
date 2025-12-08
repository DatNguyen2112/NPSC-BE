using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using NSPC.Data.Entity;
using static NSPC.Common.Helper;
using System.Linq.Expressions;
using NSPC.Business.Services.ConstructionActitvityLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text.RegularExpressions;
using System.Net;

namespace NSPC.Business.Services.ConstructionWeekReport
{
    public class ConstructionWeekReportHandler : IConstructionWeekReportHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IAttachmentHandler _attachmentHandler;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;

        public ConstructionWeekReportHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IAttachmentHandler attachmentHandler,
            IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _attachmentHandler = attachmentHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }

        public async Task<Response<ConstructionWeekReportViewModel>> Create(ConstructionWeekReportCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId = currentUser.UserId;
                var userName = currentUser.FullName;

                var entity = _mapper.Map<sm_ConstructionWeekReport>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                entity.StatusName = ConstructionConstants.FetchStatus(model.StatusCode).Name;
                entity.Code = await GetNewCode(ConstructionConstants.PrefixCode.WeekReportCode);
                entity.FileAttachments = model.FileAttachments;
                _dbContext.sm_ConstructionWeekReport.Add(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động thêm mới báo cáo tuần vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã thêm mới báo cáo tuần",
                        CodeLinkDescription = $"{entity.Code} - {entity.Title}",
                        OrderId = entity.Id,
                        ConstructionId = entity.ConstructionId,
                        ActionType = ConstructionConstants.ActionType.WEEK_REPORT
                    }, currentUser);

                    var constructionEntity =  await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == entity.ConstructionId);

                    if (constructionEntity != null)
                    {
                        constructionEntity.LastModifiedOnDate = DateTime.Now;
                        _dbContext.sm_Construction.Update(constructionEntity);
                        await _dbContext.SaveChangesAsync();
                    }

                    #endregion
                }

                // if (model.FileAttachments != null)
                // {
                //     /* Attachment Process */
                //     await processAttachment(entity.Id, model.FileAttachments);
                // }

                Log.Information(
                    "Thêm mới báo cáo tuần thành công, UserId: {@userId}, UserName: {@userName}, Model: {@model}",
                    model, userId, userName);
                return Helper.CreateSuccessResponse(_mapper.Map<ConstructionWeekReportViewModel>(entity),
                    "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ConstructionWeekReportViewModel>(ex);
            }
        }

        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_ConstructionWeekReport.AsNoTracking().Where(x => x.Code.Contains(code))
                    .OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.Code.Substring(result.Code.Length - 3, 3);
                    var currentNumInt = int.Parse(currentNum) + 1;
                    var stringResult = "";
                    if (currentNumInt < 10)
                    {
                        stringResult = "00" + currentNumInt;
                    }
                    else if (currentNumInt >= 10 && currentNumInt < 100)
                    {
                        stringResult = "0" + currentNumInt;
                    }
                    else
                    {
                        stringResult = currentNumInt.ToString();
                    }

                    return code + stringResult;
                }
                else
                {
                    return code + "001";
                }
            }
            catch (Exception ex)
            {
                Log.Error("123", ex);
                return string.Empty;
            }
        }

        public async Task<Response<Pagination<ConstructionWeekReportViewModel>>> GetPage(
            ConstructionWeekReportQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_ConstructionWeekReport.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ConstructionWeekReportViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<ConstructionWeekReportViewModel>>(ex);
            }
        }

        public async Task<Response<ConstructionWeekReportViewModel>> Update(Guid Id,
            ConstructionWeekReportCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_ConstructionWeekReport
                    .FirstOrDefaultAsync(x => x.Id == Id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<ConstructionWeekReportViewModel>("Không tìm thấy bản ghi");
                }

                entity.LastModifiedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;

                entity.Title = model.Title;
                entity.FileAttachments = model.FileAttachments;
                entity.LastWeekPlan = model.LastWeekPlan;
                entity.StartDate = model.StartDate;
                entity.EndDate = model.EndDate;
                entity.NextWeekPlan = model.NextWeekPlan;
                entity.ProcessResult = model.ProcessResult;
                entity.StatusCode = model.StatusCode;
                entity.StatusName = ConstructionConstants.FetchStatus(model.StatusCode).Name;

                _dbContext.sm_ConstructionWeekReport.Update(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động chỉnh sửa báo cáo tuần vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã chỉnh sửa báo cáo tuần",
                        CodeLinkDescription = $"{entity.Code} - {entity.Title}",
                        OrderId = entity.Id,
                        ConstructionId = entity.ConstructionId,
                        ActionType = ConstructionConstants.ActionType.WEEK_REPORT
                    }, currentUser);

                    #endregion
                    
                    var constructionEntity =  await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == entity.ConstructionId);

                    if (constructionEntity != null)
                    {
                        constructionEntity.LastModifiedOnDate = DateTime.Now;
                        _dbContext.sm_Construction.Update(constructionEntity);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                return Helper.CreateSuccessResponse<ConstructionWeekReportViewModel>
                    (_mapper.Map<ConstructionWeekReportViewModel>(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<ConstructionWeekReportViewModel>(ex);
            }
        }

        public async Task<Response<ConstructionWeekReportViewModel>> GetById(Guid Id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_ConstructionWeekReport.FirstOrDefaultAsync(x => x.Id == Id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<ConstructionWeekReportViewModel>("Không tìm thấy bản ghi");
                }

                var result = _mapper.Map<ConstructionWeekReportViewModel>(entity);

                return Helper.CreateSuccessResponse<ConstructionWeekReportViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<ConstructionWeekReportViewModel>(ex);
            }
        }

        public async Task<Response<ConstructionWeekReportViewModel>> Delete(Guid Id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_ConstructionWeekReport.FirstOrDefaultAsync(x => x.Id == Id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<ConstructionWeekReportViewModel>("Không tìm thấy bản ghi");
                }

                _dbContext.sm_ConstructionWeekReport.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse<ConstructionWeekReportViewModel>(null, "Xoá thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<ConstructionWeekReportViewModel>(ex);
            }
        }

        public static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Loại bỏ script và style
            html = Regex.Replace(html, "<(script|style)[^>]*>.*?</\\1>", "",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Thay thế <br>, <br/> thành xuống dòng
            html = Regex.Replace(html, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            // Thay thế <p> thành xuống dòng
            html = Regex.Replace(html, @"</p\s*>", "\n", RegexOptions.IgnoreCase);

            // Loại bỏ các thẻ HTML còn lại
            html = Regex.Replace(html, "<.*?>", string.Empty);

            // Giải mã các HTML entities như &nbsp;, &amp;, ...
            string plainText = WebUtility.HtmlDecode(html);

            // Xoá khoảng trắng dư
            return Regex.Replace(plainText, @"\s{2,}", " ").Trim();
        }

        public async Task<(Response, Stream, string)> ExportExcelFile(ConstructionWeekReportQueryModel model)
        {
            MemoryStream outputStream = null;
            try
            {
                var predicate = BuildQuery(model);

                var query = _dbContext.sm_ConstructionWeekReport.AsNoTracking()
                    .Where(predicate);

                if (model.Size >= 0)
                {
                    query = query.Skip(((model.Page ?? 1) - 1) * (model.Size ?? 10));
                    query = query.Take(model.Size ?? 10);
                }

                var constructionsWeekReports = await query
                    .OrderByDescending(x => x.CreatedOnDate)
                    .ToListAsync();

                var templateFilePath = Utils.CombineUnixPath(
                    ConfigCollection.Instance.StaticFiles_Folder,
                    "excel-template",
                    "ConstructionWeekReport.xlsx");

                if (!File.Exists(templateFilePath))
                {
                    Log.Error("Construction Template Not Found: {Path}", templateFilePath);
                    var templateNotFoundResponse = Helper.CreateExceptionResponse("Không tìm thấy file template");
                    return (templateNotFoundResponse, null, null);
                }

                ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
                using var package = new ExcelPackage(new FileInfo(templateFilePath));

                // Kiểm tra và lấy worksheet theo tên hoặc index
                var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Báo cáo công việc tuần")
                                ?? package.Workbook.Worksheets[0];

                // Dòng bắt đầu ghi dữ liệu (dòng 4 trong file mẫu)
                const int startRow = 4;
                
                // Lấy style từ dòng mẫu (dòng 4)
                var templateStyle = worksheet.Cells[startRow, 1, startRow, 8].Style;

                // Xóa dữ liệu cũ nếu có (từ dòng startRow trở đi)
                if (worksheet.Dimension?.End.Row >= startRow)
                {
                    worksheet.DeleteRow(startRow, worksheet.Dimension.End.Row - startRow + 1);
                }

                // Ghi dữ liệu mới
                for (var i = 0; i < constructionsWeekReports.Count; i++)
                {
                    var currentRow = startRow + i;
                    var report = constructionsWeekReports[i];

                    worksheet.Cells[currentRow, 1].Value = i + 1; // STT
                    worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, 2].Value = report.Code; // Mã báo cáo
                    worksheet.Cells[currentRow, 3].Value = report.Title; // Tiêu đề
                    worksheet.Cells[currentRow, 4].Value = report.StatusName; // Trạng thái
                    worksheet.Cells[currentRow, 5].Value =
                        $"{report.StartDate?.ToString("dd/MM/yyyy")} - {report.EndDate?.ToString("dd/MM/yyyy")}"; // Tuần báo cáo
                    worksheet.Cells[currentRow, 6].Value = HtmlToPlainText(report.LastWeekPlan) ?? null; // Kế hoạch tuần trước
                    worksheet.Cells[currentRow, 7].Value = HtmlToPlainText(report.ProcessResult) ?? null; // Kết quả thực hiện
                    worksheet.Cells[currentRow, 8].Value = HtmlToPlainText(report.NextWeekPlan) ?? null; // Kế hoạch tuần sau
                    
                    // Áp dụng border cho dòng mới
                    var currentCellRange = worksheet.Cells[currentRow, 1, currentRow, 8];
                    currentCellRange.Style.Border.Top.Style = templateStyle.Border.Top.Style;
                    currentCellRange.Style.Border.Left.Style = templateStyle.Border.Left.Style;
                    currentCellRange.Style.Border.Right.Style = templateStyle.Border.Right.Style;
                    currentCellRange.Style.Border.Bottom.Style = templateStyle.Border.Bottom.Style;
                }

                // Tự động điều chỉnh độ rộng cột cho phù hợp
                for (int col = 1; col <= 8; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
                
                // Đặt border cho toàn bộ vùng dữ liệu
                var dataRange = worksheet.Cells[startRow, 1, startRow + constructionsWeekReports.Count - 1, 8];
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                const string fileName = "Danh sách báo cáo công việc tuần.xlsx";
                outputStream = new MemoryStream();
                await package.SaveAsAsync(outputStream);
                outputStream.Position = 0;

                Log.Information("Xuất file Excel báo cáo tuần thành công. Số bản ghi: {Count}",
                    constructionsWeekReports.Count);
                return (null, outputStream, fileName);
            }
            catch (Exception e)
            {
                outputStream?.Dispose();
                Log.Error(e, "Lỗi khi tạo file Excel cho danh sách báo cáo tuần");
                var errorResponse = Helper.CreateExceptionResponse(e);
                return (errorResponse, null, null);
            }
        }

        // public async Task<Response<string>> ExportListToExcel(ConstructionWeekReportQueryModel query, RequestUser currentUser)
        // {
        //     try
        //     {
        //         // Tạo predicate để lọc dựa trên các tham số trong query
        //         var predicate = BuildQuery(query);
        //
        //         // Lấy danh sách công trình từ cơ sở dữ liệu dựa trên lọc và phân trang
        //         var constructionEntity = await _dbContext.sm_ConstructionWeekReport.AsNoTracking()
        //             .Where(predicate)
        //             .OrderByDescending(x => x.CreatedOnDate)
        //             .GetPageAsync(query);
        //
        //         // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
        //         if (constructionEntity == null || constructionEntity.Content == null ||
        //             constructionEntity.Content.Count == 0)
        //             return Helper.CreateNotFoundResponse<string>("Không có công trình nào tồn tại trong hệ thống.");
        //
        //         // Đặt tên file và đường dẫn template
        //         var fileName = $"Danh sách báo cáo công việc tuần_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
        //         var filePath = Path.Combine(_staticsFolder, fileName);
        //         var templatePath = Path.Combine(_staticsFolder, "excel-template/ConstructionWeekReport.xlsx");
        //
        //         if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
        //             return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");
        //
        //         ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
        //
        //         // Mở template Excel và điền dữ liệu vào file
        //         using (var package = new ExcelPackage(new FileInfo(templatePath)))
        //         {
        //             var worksheet = package.Workbook.Worksheets[0]; // Sử dụng worksheet đầu tiên
        //
        //             // Đặt độ rộng cố định cho cột A (cột STT) là 8
        //             worksheet.Column(1).Width = 8;
        //
        //             // Điền dữ liệu vào bảng Excel (giả sử bắt đầu từ hàng thứ 4)
        //             int startRow = 4;
        //             int index = 1;
        //
        //             foreach (var order in constructionEntity.Content)
        //             {
        //                 worksheet.Cells[startRow, 1].Value = index; // STT
        //                 worksheet.Cells[startRow, 2].Value = order.Code ?? null; // Mã báo cáo
        //                 worksheet.Cells[startRow, 3].Value = order.Title ?? null; // Tiêu đề
        //                 worksheet.Cells[startRow, 4].Value =
        //                     order.StatusName ?? null; // Trạng thái
        //                 worksheet.Cells[startRow, 5].Value = $"{order.StartDate?.ToString("dd/MM/yyyy")} - {order.EndDate?.ToString("dd/MM/yyyy")}"; // Tuần báo cáo
        //                 worksheet.Cells[startRow, 6].Value = HtmlToPlainText(order.LastWeekPlan); // Kế hoạch tuần trước
        //                 worksheet.Cells[startRow, 7].Value = HtmlToPlainText(order.ProcessResult); // Kết quả thực hiện
        //                 worksheet.Cells[startRow, 8].Value = HtmlToPlainText(order.NextWeekPlan); // Kế hoạch tuần sau
        //
        //                 startRow++;
        //                 index++;
        //             }
        //
        //             int lastDataRow = startRow - 1;
        //
        //             // Thêm đường viền cho các ô đã điền dữ liệu
        //             using (var range =
        //                    worksheet.Cells[4, 1, lastDataRow,
        //                        8]) // điều chỉnh cột cuối (18) tùy theo số lượng cột bạn có
        //             {
        //                 range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                 range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                 range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                 range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //             }
        //
        //             // Xóa các dòng thừa sau khi điền dữ liệu
        //             int totalRows = worksheet.Dimension.End.Row;
        //             if (totalRows > lastDataRow)
        //             {
        //                 worksheet.DeleteRow(lastDataRow + 1, totalRows - lastDataRow);
        //             }
        //
        //             // Tự động điều chỉnh kích thước các cột từ cột thứ hai đến cột cuối cùng
        //             worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();
        //
        //             // Lưu file Excel đã điền dữ liệu
        //             await package.SaveAsAsync(new FileInfo(filePath));
        //         }
        //
        //         Log.Information(
        //             $"Xuất file thành công, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
        //         return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex.Message, "Xuất file thất bại");
        //         Log.Information("Params: Query: {@Param}", query);
        //         return Helper.CreateExceptionResponse<string>(ex);
        //     }
        // }

        private Expression<Func<sm_ConstructionWeekReport, bool>> BuildQuery(ConstructionWeekReportQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_ConstructionWeekReport>(true);

            if (query.ConstructionId.HasValue)
            {
                predicate.And(x => x.ConstructionId == query.ConstructionId);
            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date < query.DateRange[1].Value.AddDays(1));
            }

            return predicate;
        }
    }
}