using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSPC.Data.Data.Entity.KiemKho;
using OfficeOpenXml;
using NSPC.Data.Data.Entity.VatTu;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc;
using Amazon.Auth.AccessControlPolicy;
using FirebaseAdmin.Messaging;
using System.Security.Policy;
using OfficeOpenXml.Style;

namespace NSPC.Business
{
    public class KiemKhoHandler : IKiemKhoHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public KiemKhoHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        public async Task<Response<KiemKhoDetailViewModel>> Create(KiemKhoCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.mk_KiemKho.Any(x => x.CheckInventoryCode == model.CheckInventoryCode))
                    return Helper.CreateBadRequestResponse<KiemKhoDetailViewModel>(string.Format("Mã phiên kiểm kho {0} đã tồn tại", model.CheckInventoryCode));

                if (model.StockInvetories != null)
                    foreach (var item in model.StockInvetories)
                    {
                        if (!_dbContext.sm_Product.Any(x => x.Code == item.ProductCode))
                            return Helper.CreateBadRequestResponse<KiemKhoDetailViewModel>(string.Format("Vật tư {0} không tồn tại", item.ProductCode));
                    }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<mk_KiemKho>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.Status = KiemKhoConstants.DANG_KIEM_KHO;
                entity.Sm_Stock_Transactions.ForEach(x => x.CreatedByUserId = currentUser.UserId);
                //entity.Sm_Stock_Transactions.ForEach(x => x.LoaiXuatNhapTon = LoaiXuatNhapTonConstants.TON_KHO);
                entity.Sm_Stock_Transactions.ForEach(x => x.ProductId = _dbContext.sm_Product.FirstOrDefault(s => s.Code == x.ProductCode).Id);

                _dbContext.mk_KiemKho.Add(entity);

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<KiemKhoDetailViewModel>(entity), "Tạo mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<KiemKhoDetailViewModel>(ex);
            }
        }

        public async Task<Response<KiemKhoDetailViewModel>> Update(Guid id, KiemKhoCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_KiemKho.Include(x => x.Sm_Stock_Transactions).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KiemKhoDetailViewModel>(string.Format("Phiên kiểm kho không tồn tại trong hệ thống"));

                if (_dbContext.mk_KiemKho.Any(x => x.CheckInventoryCode == model.CheckInventoryCode && x.Id != id))
                    return Helper.CreateBadRequestResponse<KiemKhoDetailViewModel>(string.Format("Mã phiên kiểm kho {0} đã tồn tại", model.CheckInventoryCode));



                if (model.StockInvetories != null)
                    foreach (var item in model.StockInvetories)
                    {
                        if (!_dbContext.sm_Product.Any(x => x.Code == item.ProductCode))
                            return Helper.CreateBadRequestResponse<KiemKhoDetailViewModel>(string.Format("Vật tư {0} không tồn tại", item.ProductCode));
                    }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                if (model.StockInvetories != null && model.StockInvetories.Any() && entity.Sm_Stock_Transactions != null)
                {
                    var listKhoEntity = entity.Sm_Stock_Transactions.Where(x => !model.StockInvetories.Select(x => x.Id).ToList().Contains(x.Id)).ToList();

                    if (listKhoEntity.Any())
                        _dbContext.RemoveRange(listKhoEntity);
                }
                else
                {
                    if (entity.Sm_Stock_Transactions != null && entity.Sm_Stock_Transactions.Count() > 0)
                        _dbContext.RemoveRange(entity.Sm_Stock_Transactions);
                }

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.Sm_Stock_Transactions.ForEach(x => x.CreatedByUserId = currentUser.UserId);
                //entity.mk_XuatNhapTon.ForEach(x => x.LoaiXuatNhapTon = LoaiXuatNhapTonConstants.TON_KHO);
                entity.Sm_Stock_Transactions.ForEach(x => x.ProductId = _dbContext.sm_Product.FirstOrDefault(s => s.Code == x.ProductCode).Id);


                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<KiemKhoDetailViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<KiemKhoDetailViewModel>(ex);
            }
        }

        public async Task<Response<KiemKhoDetailViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_KiemKho.Include(x => x.Sm_Stock_Transactions).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KiemKhoDetailViewModel>(string.Format("Phiên kiểm kho không tồn tại trong hệ thống"));

                var result = _mapper.Map<KiemKhoDetailViewModel>(entity);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<KiemKhoDetailViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<KiemKhoViewModel>>> GetPage(KiemKhoQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.mk_KiemKho.Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<KiemKhoViewModel>>(data);

                return Helper.CreateSuccessResponse(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<KiemKhoViewModel>>(ex);
            }
        }

        private Expression<Func<mk_KiemKho, bool>> BuildQuery(KiemKhoQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_KiemKho>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.CheckInventoryCode.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.WareCode))
                predicate.And(s => s.ListWare.Contains(query.WareCode));

            return predicate;
        }

        public async Task<Response<KiemKhoDetailViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_KiemKho.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KiemKhoDetailViewModel>(string.Format("Phiên kiểm kho không tồn tại trong hệ thống"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<KiemKhoDetailViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<KiemKhoDetailViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entity = await _dbContext.mk_KiemKho.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (!entity.Any())
                    return Helper.CreateBadRequestResponse("Vui lòng chọn ít nhất 1 phiên kiểm kho");

                _dbContext.RemoveRange(entity);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }
        /*public async Task<Response<VatTuTonKhoViewModel>> GetSoLuongTonKhoVatTu(string maVatTu, string maKho, int index)
        {
            try
            {
                var entity = await _dbContext.mk_XuatNhapTon.Where(x => x.MaVatTu == maVatTu && x.MaKho == maKho).ToListAsync();
                if (!entity.Any())
                    return Helper.CreateSuccessResponse(_mapper.Map<VatTuTonKhoViewModel>(new VatTuTonKhoViewModel() { SoLuong = 0, MaVatTu = maVatTu, MaKho = maKho, Stt = index }));
                else
                {
                    var soLuong = entity.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.NHAP_KHO)?.Sum(x => x.SoLuong) - entity.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.XUAT_KHO)?.Sum(x => x.SoLuong);
                    return Helper.CreateSuccessResponse(_mapper.Map<VatTuTonKhoViewModel>(new VatTuTonKhoViewModel() { SoLuong = soLuong.Value, MaVatTu = maVatTu, MaKho = maKho, Stt = index }));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<VatTuTonKhoViewModel>(ex);
            }
        }*/

        public async Task<Response<List<StockInventoryViewModel>>> Import(string path)
        {
            try
            {
                var xuatNhapTon = _dbContext.sm_Stock_Transaction;
                var vatTu = _dbContext.sm_Product;

                var filePath = _staticsFolder + "/" + path;
                var listTest = new List<StockInventoryViewModel>();

                if (string.IsNullOrEmpty(path) || !File.Exists(filePath))
                    return Helper.CreateBadRequestResponse<List<StockInventoryViewModel>>("Invalid path or files does not exist.");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(filePath))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Where(x => x.Name == "Sheet1").FirstOrDefault();
                    var lastRow = worksheet.Dimension.Rows;
                    var lastCol = worksheet.Dimension.Columns;

                    for (int rowIndex = 6; rowIndex <= lastRow; rowIndex++)
                    {
                        var cell = worksheet.Cells[rowIndex, 1];
                        if (cell == null || cell.Value == null)
                            break;
                        var item = new StockInventoryViewModel
                        {   
                            ProductCode = ExcelHelper.GetCellValue(worksheet, rowIndex, "Mã vật tư", 5, 1, "horizontal"),
                            ProductName = ExcelHelper.GetCellValue(worksheet, rowIndex, "Tên vật tư", 5, 1, "horizontal"),
                            WareCode = ExcelHelper.GetCellValue(worksheet, rowIndex, "Mã kho", 5, 1, "horizontal"),
                            Unit = ExcelHelper.GetCellValue(worksheet, rowIndex, "Đơn vị tính", 5, 1, "horizontal"),
                            ActualInventory = int.Parse(ExcelHelper.GetCellValue(worksheet, rowIndex, "Tồn kho", 5, 1, "horizontal")),
                            RealityInventory = int.Parse(ExcelHelper.GetCellValue(worksheet, rowIndex, "Tồn thực tế", 5, 1, "horizontal")),
                            NoteInventory = ExcelHelper.GetCellValue(worksheet, rowIndex, "Ghi chú", 5, 1, "horizontal"),
                            ReasonInventory = ExcelHelper.GetCellValue(worksheet, rowIndex, "Lý do", 5, 1, "horizontal"),
                            //Stt = rowIndex - 5,

                        };

                        if (listTest.Any(x => x.ProductCode == item.ProductCode && x.WareCode == item.WareCode))
                            return new Response<List<StockInventoryViewModel>>(System.Net.HttpStatusCode.BadRequest, null, string.Format("Vật tư {0} nằm trong kho {1} đã bị lặp lại", item.ProductCode, item.WareCode));

                        item.ProductName = vatTu.FirstOrDefault(x => x.Code == item.ProductCode)?.Name;
                        item.Unit = vatTu.FirstOrDefault(x => x.Code == item.ProductCode)?.Unit;
                        var entity = await xuatNhapTon.Where(x => x.ProductCode == item.ProductCode && x.WareCode == item.WareCode).ToListAsync();
                        if (!entity.Any())
                        {
                            item.Quantity = 0;
                            item.DiffereceQuantity = item.DiffereceQuantity;
                        }
                        else
                        {
                            //item.Quantity = entity.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.NHAP_KHO)?.Sum(x => x.SoLuong) - entity.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.XUAT_KHO)?.Sum(x => x.SoLuong);
                            item.DiffereceQuantity = Math.Abs(item.DiffereceQuantity.Value - item.Quantity.Value);
                        }

                        listTest.Add(item);
                    }
                }

                return new Response<List<StockInventoryViewModel>>(System.Net.HttpStatusCode.OK, listTest, string.Format("Thêm vào {0} vật tư thành công", listTest.Count()));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Path: {@path}", path);
                return Helper.CreateExceptionResponse<List<StockInventoryViewModel>>(ex);
            }
        }
        public async Task<Response<KiemKhoExcelImport>> Export(Guid id, string path)
        {
            try
            {
                var entity = await _dbContext.mk_KiemKho.Include(x => x.Sm_Stock_Transactions).Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                    return Helper.CreateNotFoundResponse<KiemKhoExcelImport>(string.Format("Phiên kiểm kho không tồn tại trong hệ thống"));

                var filePath = _staticsFolder + "/" + path;
                var listTest = new List<StockInventoryViewModel>();

                if (string.IsNullOrEmpty(path) || !File.Exists(filePath))
                    return Helper.CreateBadRequestResponse<KiemKhoExcelImport> ("Invalid path or files does not exist.");

                var headerPosition = 13;
                var contentPosition = 14;
                var columPosition = 1;
                var result = new KiemKhoExcelImport();
                var directoryPath = Directory.GetCurrentDirectory();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage MyExcel = new ExcelPackage(filePath))
                {
                    ExcelWorksheet worksheet = MyExcel.Workbook.Worksheets[0];

                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                    {
                        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                        {
                            if (worksheet.Cells[row, col].Text == "Start")
                            {
                                headerPosition = row;
                                contentPosition = row + 1;
                                columPosition = col;
                                break;
                            }
                        }
                    }

                    worksheet.Name = entity.CheckInventoryCode;
                    worksheet.DefaultColWidth = 25;
                    worksheet.Cells[headerPosition, columPosition].Value = "Mã vật tư";
                    worksheet.Cells[headerPosition, columPosition].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[headerPosition, columPosition + 1].Value = "Tên vật tư";
                    worksheet.Cells[headerPosition, columPosition + 1].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[headerPosition, columPosition + 2].Value = "Đơn vị tính";
                    worksheet.Cells[headerPosition, columPosition + 2].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[headerPosition, columPosition + 3].Value = "Kho";
                    worksheet.Cells[headerPosition, columPosition + 3].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[headerPosition, columPosition + 4].Value = "Số lượng";
                    worksheet.Cells[headerPosition, columPosition + 4].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[headerPosition, columPosition + 5].Value = "Số lượng kiểm kê";
                    worksheet.Cells[headerPosition, columPosition + 5].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[headerPosition, columPosition + 6].Value = "Số lượng chênh lệch";
                    worksheet.Cells[headerPosition, columPosition + 6].Style.Font.Bold = true;
                    worksheet.Cells[headerPosition, columPosition + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    foreach (var item in entity.Sm_Stock_Transactions.ToList())
                    {
                        var index = entity.Sm_Stock_Transactions.ToList().IndexOf(item);
                        worksheet.Cells[contentPosition + index, columPosition].Value = item.ProductCode;
                        worksheet.Cells[contentPosition + index, columPosition].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[contentPosition + index, columPosition].Value = item.ProductName;
                        worksheet.Cells[contentPosition + index, columPosition].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[contentPosition + index, columPosition + 2].Value = item.Unit;
                        worksheet.Cells[contentPosition + index, columPosition + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[contentPosition + index, columPosition + 3].Value = CodeTypeCollection.Instance.FetchCode(item.WareCode,"vn", item.TenantId)?.Title;
                        worksheet.Cells[contentPosition + index, columPosition + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[contentPosition + index, columPosition + 4].Value = item.Quantity ?? 0;
                        worksheet.Cells[contentPosition + index, columPosition + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        /*worksheet.Cells[contentPosition + index, columPosition + 5].Value = item.InventoryQuantity ?? 0;
                        worksheet.Cells[contentPosition + index, columPosition + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[contentPosition + index, columPosition + 6].Value = item.DifferentAmount ?? 0;
                        worksheet.Cells[contentPosition + index, columPosition + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);*/
                    }

                    result.CheckInventoryCode = entity.CheckInventoryCode;
                    result.Data = await MyExcel.GetAsByteArrayAsync();
                    //Add additional info here
                    return Helper.CreateSuccessResponse<KiemKhoExcelImport>(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<KiemKhoExcelImport>(ex);
            }
        }

        public async Task<Response<KiemKhoDetailViewModel>> BalanceInventory(Guid id, KiemKhoCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_KiemKho.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KiemKhoDetailViewModel>(string.Format("Phiên kiểm kho không tồn tại trong hệ thống"));

                entity.Status = KiemKhoConstants.DA_CAN_BANG;

                if (model.StockInvetories != null)
                    foreach (var item in model.StockInvetories)
                    {
                        if (!_dbContext.sm_Product.Any(x => x.Code == item.ProductCode))
                            return Helper.CreateBadRequestResponse<KiemKhoDetailViewModel>(string.Format("Vật tư {0} không tồn tại", item.ProductCode));
                        else
                        {
                            entity.Sm_Stock_Transactions.ForEach(x => x.ClosingInventoryQuantity = item.ActualInventory);
                            entity.Sm_Stock_Transactions.ForEach(x => x.InventoryIncreased = item.ActualInventory);
                        }
                    }

                entity.Status = KiemKhoConstants.DA_CAN_BANG;
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<KiemKhoDetailViewModel>(entity), "Cân bằng kho thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<KiemKhoDetailViewModel>(ex);
            }
        }
    }
}
