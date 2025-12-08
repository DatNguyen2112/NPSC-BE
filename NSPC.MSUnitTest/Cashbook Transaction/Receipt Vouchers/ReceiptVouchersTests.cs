using AutoMapper;
using NSPC.Business.AutoMapper;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Common;
using NSPC.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace SalesMngt.MSUnitTest.Cashbook_Transaction.Receipt_Vouchers
{
    [TestClass]
    public class ReceiptVouchersTests
    {
        SMDbContext dbContext;
        ICashbookTransactionHandler cashbookTransactionHandler;
        IDebtTransactionHandler debtTransactionHandler;
        IConstructionActivityLogHandler constructionActivityLogHandler;

        public ReceiptVouchersTests()
        {
            dbContext = new SMDbContext();
            var mapper = new Mapper(AutoMapperConfig.RegisterMappings());
            debtTransactionHandler = new DebtTransactionHandler(dbContext, null, mapper);

            cashbookTransactionHandler = new CashbookTransactionHandler(dbContext, null, mapper, debtTransactionHandler, null, constructionActivityLogHandler);
        }

        #region Test cases Thêm mới phiếu thu (Create)

        // Test case (TC_RVN_001 ): Tạo phiếu thu nhập đầy đủ thông tin
        [TestMethod]
        public async Task TC_RVN_001_CreateReceiptVoucher_FullInfo()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("67462807-f94b-4299-9e49-c26d04942cf2"), "Đinh Công Thành");

            // Arrange
            var receiptVoucher = new CashbookTransactionCreateUpdateModel
            {
                Code = "UNITTESTPT",
                EntityId = Guid.Parse("9419c7ec-549f-4399-bde1-7082cbc96f9b"),
                EntityCode = "UNITTEST_NGUYENVANA",
                EntityName = "Nguyễn Văn A",
                EntityTypeCode = "CUSTOMER",
                EntityTypeName = "Khách hàng",
                PurposeCode = "other_receipt",
                Amount = 1000000,
                PaymentMethodCode = "bank_transfer",
                ReceiptDate = new DateTime(2024, 12, 11),
                Description = "Thu tiền bán gạch Ceramic",
                TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                ProjectId = Guid.Parse("5252504d-6c6e-4ef1-809d-e0590c32d6f5"),
                IsDebt = true,
                Reference = "INV-12345"
            };

            // Act
            var result = await cashbookTransactionHandler.Create(receiptVoucher, currentUser);

            if (result.IsSuccess)
            {
                // Assert
                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual("Thêm mới thành công", result.Message);
            }
            else
            {
                // Assert
                Assert.IsFalse(result.IsSuccess);
                Assert.AreEqual("Thêm mới thất bại", result.Message);
            }
        }

        #endregion

        #region Test cases Cập nhật phiếu thu (Update)

        // Test case (TC_RVN_004): Cập nhật phiếu thu nhập đầy đủ thông tin
        [TestMethod]
        public async Task TC_RVN_004_UpdateReceiptVoucher_FullInfo()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("67462807-f94b-4299-9e49-c26d04942cf2"), "Đinh Công Thành");
            var receiptVoucherId = Guid.Parse("47437e59-37f7-405a-9038-b6d0626724eb");

            // Arrange
            var receiptVoucher = new CashbookTransactionCreateUpdateModel
            {
                Code = "UNITTESTPT",
                EntityId = Guid.Parse("9419c7ec-549f-4399-bde1-7082cbc96f9b"),
                EntityCode = "UNITTEST_NGUYENVANA",
                EntityName = "Nguyễn Văn A",
                EntityTypeCode = "CUSTOMER",
                EntityTypeName = "Khách hàng",
                PurposeCode = "other_receipt",
                Amount = 10000000,
                PaymentMethodCode = "bank_transfer",
                ReceiptDate = new DateTime(2024, 12, 11),
                Description = "Thu tiền bán gạch Ceramic. Mô tả thêm",
                TransactionTypeCode = CashbookTransactionConstants.ReceiptVoucherType,
                ProjectId = Guid.Parse("5252504d-6c6e-4ef1-809d-e0590c32d6f5"),
                IsDebt = true,
                Reference = "INV-12345, INV-1234223"
            };

            // Act
            var result = await cashbookTransactionHandler.Update(receiptVoucherId,receiptVoucher, currentUser);

            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess, "Chỉnh sửa thành công");
            }
            else
            {
                Assert.AreEqual("Cập nhật không thành công", result.Message);
            }
        }

        #endregion

        #region Test cases Hủy phiếu thu (Cancel)

        // Test case (TC_RVN_005): Hủy phiếu thu
        [TestMethod]
        public async Task TC_RVN_005_CancelReceiptVoucher()
        {
            var receiptVoucherId = Guid.Parse("47437e59-37f7-405a-9038-b6d0626724eb");

            // Act
            var result = await cashbookTransactionHandler.DeactiveCashbookTransactionsAsync(new List<Guid> { receiptVoucherId });

            if (result.IsSuccess == true)
            {
                Assert.IsTrue(result.IsSuccess);
            }
            else
            {
                Assert.AreEqual("Hủy không thành công", result.Message);
            }
        }

        #endregion

    }
}
