using NSPC.Common;

namespace NSPC.Business.Services
{
    public class WarehouseTransferNoteCreateModel
    {
        public string TransferNoteCode { get; set; }
        public string ExportWarehouseCode { get; set; }
        public string ImportWarehouseCode { get; set; }
        public DateTime? TransferredOnDate { get; set; }
        public string Note { get; set; }
        public List<WarehouseTransferNoteItemCreateUpdateModel> Items { get; set; }
    }

    public class WarehouseTransferNoteViewModel
    {
        public Guid Id { get; set; }
        public string TransferNoteCode { get; set; }
        public CodeTypeListModel ExportWarehouse { get; set; }
        public CodeTypeListModel ImportWarehouse { get; set; }
        public string ImportWarehouseName { get; set; }
        public string ExportWarehouseName { get; set; }
        public string TransferredByUserName { get; set; }
        public DateTime? TransferredOnDate { get; set; }
        public string StatusCode { get; set; }
        public string Note { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public List<WarehouseTransferNoteItemViewModel> Items { get; set; }
    }

    public class WarehouseTransferNoteQuery : PaginationRequest
    {
        public string StatusCode { get; set; }
        public DateTime?[] DateRange { get; set; }
        public string OrderCode { get; set; }
    }
}
