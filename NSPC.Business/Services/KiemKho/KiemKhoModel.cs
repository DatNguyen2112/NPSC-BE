using NSPC.Business.Services.VatTu;
using NSPC.Common;

namespace NSPC.Business
{
    public class KiemKhoViewModel
    {
        public Guid Id { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string WareCode { get; set; }
        public string Note {  get; set; }
        public string Tags { get; set; }
        public string Status { get; set; } // Trạng thái kiểm (Chưa kiểm, Đã cân bằng)
        //public string MucDich { get; set; }
        public string CheckInventoryCode { get; set; } // Mã phiên kiểm kho
        public List<string> ListWare { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class KiemKhoDetailViewModel
    {
        public Guid Id { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string CheckInventoryCode { get; set; } // Mã phiên kiểm kho
        public string WareCode { get; set; }
        public string Note { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; } // Trạng thái kiểm (Chưa kiểm, Đã cân bằng)
        public List<string> ListWare { get; set; }
        public DateTime? ToDate { get; set; }
        public List<StockInventoryViewModel> StockInvetories { get; set; } // Vat tu ton kho
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
    }

    public class StockInventoryViewModel
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string WareCode { get; set; }
        public int? Quantity { get; set; }
        public int? ActualInventory {  get; set; } // Tồn kho
        public int? RealityInventory { get; set; } // Tồn thực tế
        //public int? SoLuongKiemKe { get; set; }
        public int? DiffereceQuantity { get; set; } // Chênh lệch (âm hoặc dương)
        public string NoteInventory { get; set; } // Ghi chú cho vật tư tồn kho
        public string ReasonInventory { get; set; } // Lý do cho vật tư tồn kho
        //public string LoaiXuatNhapTon { get; set; }
        public string DifferenceType { get; set; } // Loại chênh lệch (Lệch, Khớp)
        public Guid ProductId { get; set; }
        //public VatTuViewModel mk_VatTu { get; set; }
        //public int? Stt { get; set; }
    }

    public class KiemKhoCreateUpdateModel
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string CheckInventoryCode { get; set; } // Mã phiên kiểm kho
        public string WareCode { get; set; }
        public string Note { get; set; }
        public string Tags { get; set; }
        public List<string> ListWare { get; set; }
        public DateTime? ToDate { get; set; }
        public List<StockInventoryCreateUpdateModel> StockInvetories { get; set; } // Vat tu ton kho
    }

    public class StockInventoryCreateUpdateModel
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string WareCode { get; set; }
        public int? Quantity { get; set; }
        public int? ActualInventory { get; set; } // Tồn kho
        public int? RealityInventory { get; set; } // Tồn thực tế
        //public int? SoLuongKiemKe { get; set; }
        public int? DiffereceQuantity { get; set; } // Chênh lệch (âm hoặc dương)
        public string NoteInventory { get; set; } // Ghi chú cho vật tư tồn kho
        public string ReasonInventory { get; set; } // Lý do cho vật tư tồn kho
        //public string LoaiXuatNhapTon { get; set; }
        public string DifferenceType { get; set; } // Loại chênh lệch (Lệch, Khớp)
        public Guid ProductId { get; set; }
        //public VatTuViewModel mk_VatTu { get; set; }
        //public int? Stt { get; set; }
    }

    public class KiemKhoQueryModel :PaginationRequest
    {
        public string WareCode { get; set; }
    }

    public class KiemKhoExcelImport
    {
        public byte[] Data { get; set; }
        public string CheckInventoryCode { get; set; }
    }
}
