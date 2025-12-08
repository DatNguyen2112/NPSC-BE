using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services;
using NSPC.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.Quotation;
using AutoMapper;
using NSPC.Business.AutoMapper;
using NSPC.Business;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Common;

namespace SalesMngt.MSUnitTest
{
    [TestClass]
    public class UnitTestQuotation
    {
        SMDbContext dbContext;
        IQuotationHandler handler;
        IKhachHangHandler cusHandler;
        IDebtTransactionHandler debtTransactionHandler;
        ICustomerServiceCommentHandler customerServiceCommentHandler;

        public UnitTestQuotation()
        {
            dbContext = new SMDbContext();
            var mapper = new Mapper(AutoMapperConfig.RegisterMappings());

            debtTransactionHandler = new DebtTransactionHandler(dbContext, null, mapper);

            cusHandler = new KhachHangHandler(dbContext, null, mapper, debtTransactionHandler, customerServiceCommentHandler);

            handler = new QuotationHandler(dbContext, null, mapper, cusHandler);
        }

        [TestMethod]
        public async Task CreateAsync()
        {
            var items = new List<QuotationItemCreateUpdateModel>();
            // Búa VAT 10%
            items.Add(new QuotationItemCreateUpdateModel
            {
                LineNumber = 1,
                ProductId = Guid.Parse("8a00e4a7-a4e9-4811-bc90-c0275d5ab986"),
                Specifications = "test",
                Quantity = 10,
                UnitPrice = 200000,
                UnitPriceDiscountPercent = 10,
                UnitPriceDiscountAmount = 20000,
                UnitPriceDiscountType = QuotationConstants.UnitPriceDiscountType.Percent,
                LineNote = "Ghi chú 1"
            });

            var quotation = await handler.Create(
                new QuotationCreateUpdateModel
                {
                    CustomerId = Guid.Parse("290b6f26-018b-42df-b6b7-aee589facef7"),
                    ProjectId = Guid.Parse("dc30dc16-2a5b-4969-a21b-50a3a13b9b1a"),
                    TypeCode = "QuotationProduct",
                    DueDate = DateTime.Now,
                    Note = "Ghi chú",
                    DiscountType = QuotationConstants.DiscountType.Percent,
                    DiscountAmount = 30000,
                    DiscountPercent = 10,
                    DiscountReason = "Không có chiết khấu",
                    ShippingCostAmount = 0,
                    OtherCostAmount = 0,
                    PaymentMethodCode = "cash",
                    Status = QuotationConstants.StatusCode.CUSTOMER_APPROVED,
                    QuotationItem = items
                }, new Helper.RequestUser(Guid.Parse("67462807-f94b-4299-9e49-c26d04942cf2"), "Quỳnh Nhi"));

            Assert.IsTrue(quotation.IsSuccess);
        }
    }
}
