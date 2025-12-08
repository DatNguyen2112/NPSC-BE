using NSPC.Common;
namespace NSPC.Business.Services.QuanLyLoaiXe
{
    public class LoaiXeViewModel
    {
        public Guid Id { get; set; }
        public string TenLoaiXe { get; set; }
        public string MoTa { get; set; }
    }
    public class LoaiXeCreateUpdateModel
    {
        public string TenLoaiXe { get; set; }
        public string MoTa { get; set; }
    }
    public class LoaiXeQueryModel : PaginationRequest
    {

    }
}
