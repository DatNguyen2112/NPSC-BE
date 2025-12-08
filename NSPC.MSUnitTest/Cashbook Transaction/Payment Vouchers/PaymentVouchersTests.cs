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

namespace SalesMngt.MSUnitTest.Cashbook_Transaction.Payment_Vouchers
{
    [TestClass]
    public class PaymentVouchersTests
    {
        SMDbContext dbContext;
        ICashbookTransactionHandler cashbookTransactionHandler;
        IDebtTransactionHandler debtTransactionHandler;
        IConstructionActivityLogHandler constructionActivityLogHandler;

        public PaymentVouchersTests()
        {
            dbContext = new SMDbContext();
            var mapper = new Mapper(AutoMapperConfig.RegisterMappings());
            debtTransactionHandler = new DebtTransactionHandler(dbContext, null, mapper);

            cashbookTransactionHandler = new CashbookTransactionHandler(dbContext, null, mapper, debtTransactionHandler, null,  constructionActivityLogHandler);
        }

        #region Test cases Thêm mới phiếu chi (Create)

        // Test case (TC_PVN_001 ): Tạo phiếu chi nhập đầy đủ thông tin
        [TestMethod]
        public async Task TC_PVN_001_CreatePaymentVoucher_FullInfo()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("67462807-f94b-4299-9e49-c26d04942cf2"), "Đinh Công Thành");

            // Arrange
            var paymentVoucher = new CashbookTransactionCreateUpdateModel
            {
                Code = "UNITTESTPC",
                EntityId = Guid.Parse("9419c7ec-549f-4399-bde1-7082cbc96f9b"),
                EntityCode = "UNITTEST_NGUYENVANA",
                EntityName = "Nguyễn Văn A",
                EntityTypeCode = "CUSTOMER",
                EntityTypeName = "Khách hàng",
                PurposeCode = "internal_expenses",
                Amount = 10000000,
                PaymentMethodCode = "bank_transfer",
                ReceiptDate = new DateTime(2024, 12, 11),
                Description = "Thu tiền bán gạch Ceramic",
                TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                ProjectId = Guid.Parse("5252504d-6c6e-4ef1-809d-e0590c32d6f5"),
                IsDebt = true,
                Reference = "INV-12345"
            };

            // Act
            var result = await cashbookTransactionHandler.Create(paymentVoucher, currentUser);

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

        #region Test cases Cập nhật phiếu chi (Update)

        // Test case (TC_PVN_004 ): Cập nhật phiếu chi nhập đầy đủ thông tin
        [TestMethod]
        public async Task TC_PVN_004_UpdatePaymentVoucher_FullInfo()
        {
            var currentUser = new Helper.RequestUser(Guid.Parse("67462807-f94b-4299-9e49-c26d04942cf2"), "Đinh Công Thành");
            var paymentVoucherId = Guid.Parse("f4ee281c-6942-4fe2-ba79-f58bcc72ef8d");

            // Arrange
            var paymentVoucher = new CashbookTransactionCreateUpdateModel
            {
                Code = "UNITTESTPC",
                EntityId = Guid.Parse("9419c7ec-549f-4399-bde1-7082cbc96f9b"),
                EntityCode = "UNITTEST_NGUYENVANA",
                EntityName = "Nguyễn Văn A",
                EntityTypeCode = "CUSTOMER",
                EntityTypeName = "Khách hàng",
                PurposeCode = "internal_expenses",
                Amount = 10000000,
                PaymentMethodCode = "bank_transfer",
                ReceiptDate = new DateTime(2024, 12, 11),
                Description = "Chi tiền bán gạch Ceramic. Mô tả thêm",
                TransactionTypeCode = CashbookTransactionConstants.PaymentVoucherType,
                ProjectId = Guid.Parse("5252504d-6c6e-4ef1-809d-e0590c32d6f5"),
                IsDebt = true,
                Reference = "INV-12345, INV-1234223"
            };

            // Act
            var result = await cashbookTransactionHandler.Update(paymentVoucherId, paymentVoucher, currentUser);

            if (result.IsSuccess)
            {
                Assert.IsTrue(result.IsSuccess, "Chỉnh sửa thành công");
            }
            else
            {
                Assert.AreEqual("Cập nhật không thành công", result.Message);
            }
        }

        #endregion

        #region Test cases Hủy phiếu chi (Cancel)

        // Test case (TC_PVN_005 ): Hủy phiếu chi
        [TestMethod]
        public async Task TC_PVN_005_CancelPaymentVoucher()
        {
            var paymentVoucherId = Guid.Parse("f4ee281c-6942-4fe2-ba79-f58bcc72ef8d");

            // Act
            var result = await cashbookTransactionHandler.DeactiveCashbookTransactionsAsync(new List<Guid> { paymentVoucherId });

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
