using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services
{
    public class CustomerReturnItemCreateUpdateModel
    {
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;
        
        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        public decimal InitialQuantity { get; set; }

        /// <summary>
        /// Số lượng sản phẩm khách muốn trả
        /// </summary>
        public decimal ReturnedQuantity { get; set; }

        /// <summary>
        /// Đơn giá trả
        /// </summary>
        public decimal ReturnedUnitPrice { get; set; }

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Id phiếu trả hàng
        /// </summary>
        public Guid ReturnOrderId { get; set; }
    }

    public class CustomerReturnItemViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã vật tư / sản phẩm
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Tên vật tư / sản phẩm
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNo { get; set; } = 0;

        /// <summary>
        /// Số lượng sản phẩm khách muốn trả
        /// </summary>
        public decimal ReturnedQuantity { get; set; }

        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        public decimal InitialQuantity { get; set; }

        /// <summary>
        /// Số lượng SP còn lại
        /// </summary>
        public decimal RemainingQuantity { get; set; }

        /// <summary>
        /// Đơn giá gốc
        /// </summary>
        public decimal InitialUnitPrice { get; set; }

        /// <summary>
        /// Đơn giá trả
        /// </summary>
        public decimal ReturnedUnitPrice { get; set; }

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal LineAmount { get; set; }

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Id phiếu trả hàng
        /// </summary>
        public Guid ReturnOrderId { get; set; }
    }
}
