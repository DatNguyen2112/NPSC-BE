using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSPC.Business.Services.BHXH;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;

namespace NSPC.Business.Services.ChatBot;

public class ChatBotHandler : IChatBotHandler
{
    private readonly SMDbContext _dbContext;

    private const string BotInstruction =
        "Bạn là một chatbot hữu ích của Geneat Software. Tiếp theo bạn sẽ được cung cấp thông tin chi tiết về một khách hàng của công ty. Người tương tác với bạn là nhân viên hỗ trợ khách hàng và công việc của bạn là hỗ trợ, đưa ra những gợi, lời khuyên, nhắc nhở để giúp họ hỗ trợ khách hàng tốt hơn. Nhân viên hỗ trợ cũng nắm được thông tin khách hàng rồi nên không cần nhắc lại chi tiết thông tin khi trả lời, các câu trả lời cũng cần ngắn gọn nhưng đủ nội dung";

    public ChatBotHandler(SMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response> GetResponseFromBot(ChatBotPromptModel model, Stream outputStream)
    {
        try
        {
            var customer = await _dbContext.sm_Customer.FindAsync(model.CustomerId);

            if (string.IsNullOrWhiteSpace(model.Prompt))
            {
                return Helper.CreateBadRequestResponse();
            }

            if (customer == null)
            {
                return Helper.CreateNotFoundResponse();
            }

            var customerInfo = await GetCustomerInfo(customer);
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                ConfigCollection.Instance.ChatGPT_Token);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                model = ConfigCollection.Instance.ChatGPT_Model,
                stream = true,
                messages = new List<object>
                {
                    new
                    {
                        role = "system",
                        content = BotInstruction
                    },
                    new
                    {
                        role = "system",
                        content = customerInfo
                    },
                    new
                    {
                        role = "user",
                        content = model.Prompt
                    },
                }
            }));
            var request = new HttpRequestMessage(HttpMethod.Post, ConfigCollection.Instance.ChatGPT_ChatCompletionsUrl)
            {
                Content = content
            };

            using var completion = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            completion.EnsureSuccessStatusCode();

            await using var stream = await completion.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);

            while (await streamReader.ReadLineAsync() is { } line)
            {
                if (!line.StartsWith("data:"))
                    continue;

                var data = line[6..];

                if (data.StartsWith("[DONE]"))
                {
                    break;
                }

                var responseObject = JsonConvert.DeserializeObject<ChatGptResponse>(data);
                var text = responseObject.Choices[0].Delta?.Content ?? "";
                var bytes = Encoding.UTF8.GetBytes(text);

                await outputStream.WriteAsync(bytes);
                await outputStream.FlushAsync();
            }

            return Helper.CreateSuccessResponse();
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            return Helper.CreateExceptionResponse(e);
        }
    }

    private async Task<string> GetCustomerInfo(sm_Customer customer)
    {
        var saleOrders = await _dbContext.sm_SalesOrder
            .AsNoTracking()
            .Include(x => x.SalesOrderItems)
            .ThenInclude(x => x.sm_Product)
            .Include(x => x.mk_DuAn)
            .Where(x => x.CustomerId == customer.Id)
            .ToListAsync();
        // var debt = await _dbContext.sm_DebtTransaction
        //     .Where(x => x.EntityId == id)
        //     .ToListAsync();
        var customerServices = await _dbContext.sm_LichSuChamSoc
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .Include(x => x.mk_DuAn)
            .Where(x => x.CustomerId == customer.Id)
            .ToListAsync();
        var commentsInCustomer = await _dbContext.sm_CustomerServiceComment
            // .Where(x => x.CustomerId == customer.Id)
            // .OrderByDescending(x => x.CreatedOnDate)
            .ToListAsync();
        var customerServiceIdList = customerServices.Select(y => y.Id).ToList();
        var commentsInService = await _dbContext.sm_CustomerServiceComment
            // .Where(x => x.CustomerServiceId != null &&
            //             customerServiceIdList.Contains(x.CustomerServiceId.Value))
            // .OrderByDescending(x => x.CreatedOnDate)
            .ToListAsync();
        // var commentsInServiceDict = commentsInService.GroupBy(x => x.CustomerServiceId)
        //     .ToDictionary(x => x.Key, x => x.ToList());
        var customerInfo = new List<string>()
        {
            "Thông tin khách hàng",
            "Thông tin cá nhân:",
            "  Tên: " + customer.Name,
            "  Số điện thoại: " + customer.PhoneNumber,
            "  Nhóm khách hàng: " + CodeTypeCollection.Instance
                .FetchCode(customer.CustomerGroupCode, LanguageConstants.Default, customer.TenantId)?.Title,
            "  Email: " + customer.Email,
            "  Giới tính: " + (string.IsNullOrWhiteSpace(customer.Sex) ? "Nam" : customer.Sex),
            "  Ngày sinh: " + customer.Birthdate?.ToString("d"),
            "  Địa chỉ: " + string.Join(", ", new List<string>
            {
                customer.Address,
                customer.WardName,
                customer.DistrictName,
                customer.ProvinceName
            }.Where(x => !string.IsNullOrWhiteSpace(x))),
            "  Mã số thuế: " + customer.TaxCode,
            "  Số fax: " + customer.Fax,
            "  Mô tả: " + customer.Note,
            "Thông tin mua hàng:",
            "  Tổng chi tiêu: " + customer.ExpenseAmount,
            "  Tổng SL đơn hàng: " + customer.OrderCount,
            "  Tổng SL báo giá: " + customer.TotalQuotationCount,
            "  Công nợ hiện tại: " + customer.DebtAmount,
            "Thông tin gợi ý bán hàng",
            "  Chăm sóc gần nhất: " + customer.LastCareOnDate?.ToString("s"),
            "  Nguồn khách hàng: " +
            (customer.CustomerSource != null ? string.Join(", ", customer.CustomerSource) : null),
            "  Số lần chăm sóc: " + customer.TotalCareTimes,
            "Trao đổi:"
        };

        foreach (var comment in commentsInCustomer)
        {
            customerInfo.Add("  - Người tạo: " + comment.CreatedByUserName);
            customerInfo.Add("    Thời gian: " + comment.CreatedOnDate.ToString("s"));
            customerInfo.Add("    Nội dung: '" + comment.Content + "'");
        }

        customerInfo.Add("Lịch sử mua hàng:");

        foreach (var saleOrder in saleOrders)
        {
            customerInfo.Add("  - Mã đơn hàng: " + saleOrder.OrderCode);
            customerInfo.Add("    Ngày tạo: " + saleOrder.CreatedOnDate.ToString("s"));
            customerInfo.Add("    Người tạo: " + saleOrder.CreatedByUserName);
            customerInfo.Add("    Tham chiếu: " + saleOrder.Reference);
            customerInfo.Add("    Phương thức dự kiến: " + CodeTypeCollection.Instance
                .FetchCode(saleOrder.PaymentMethodCode, LanguageConstants.Default, saleOrder.TenantId)?.Title);
            customerInfo.Add("    Dự án: " + saleOrder.mk_DuAn?.TenDuAn);
            customerInfo.Add("    Kho xuất: " + saleOrder.WareName);
            customerInfo.Add("    Trạng thái: " + SalesOrderConstants.FetchStatus(saleOrder.StatusCode)?.Name);
            customerInfo.Add("    Ghi chú: " + saleOrder.Note);
            customerInfo.Add("    Sản phẩm:");

            foreach (var item in saleOrder.SalesOrderItems)
            {
                customerInfo.Add("    - Tên sản phẩm: " + item.ProductName);
                customerInfo.Add("      Đơn vị tính: " + item.Unit);
                customerInfo.Add("      Đơn giá: " + item.UnitPrice);
                customerInfo.Add("      Số lượng: " + item.Quantity);
                customerInfo.Add("      Chiết khấu: " + item.UnitPriceDiscountAmount);
                customerInfo.Add("      Thành tiền: " + item.AfterLineDiscountGoodsAmount);
            }

            customerInfo.Add("    Tổng tiền: " + saleOrder.SubTotal);
            customerInfo.Add("    Chiết khấu (%): " + saleOrder.DiscountPercent);
            customerInfo.Add("    Chi phí vận chuyển: " + saleOrder.DeliveryFee);
            customerInfo.Add("    Chi phí khác: " + saleOrder.OtherCostAmount);
            customerInfo.Add("    Thuế: " + saleOrder.VATAmount);
            customerInfo.Add("    Khách phải trả: " + saleOrder.Total);
            customerInfo.Add("    Trạng thái thanh toán: " + saleOrder.PaymentStatusCode switch
            {
                "DA_THANH_TOAN" => "Đã thanh toán",
                "THANH_TOAN_MOT_NUA" => "Thanh toán một nửa",
                _ => "Chưa thanh toán"
            });
        }

        customerInfo.Add("Lịch sử chăm sóc:");

        foreach (var customerService in customerServices)
        {
            customerInfo.Add("  - Mã chăm sóc: " + customerService.Code);
            customerInfo.Add("    Đánh giá từ khách: " + customerService.DanhGia + "/5");
            customerInfo.Add("    Nội dung chính: " + customerService.CustomerServiceContent);
            customerInfo.Add("    Độ ưu tiên: " + customerService.Priority);
            customerInfo.Add("    Chi tiết: " + customerService.GhiChu);
            customerInfo.Add("    Thời gian: " + customerService.DateRange[0]?.ToString("s") + " - " +
                             customerService.DateRange[1]?.ToString("s"));
            customerInfo.Add("    Dự án: " + customerService.mk_DuAn?.TenDuAn);
            customerInfo.Add("    Người tạo: " +
                             (customerService.CreatedByUser?.Name ?? customerService.CreatedByUserName));
            customerInfo.Add("    Trao đổi:");

            // if (commentsInServiceDict.TryGetValue(customerService.Id, out var comments))
            // {
            //     foreach (var comment in comments)
            //     {
            //         customerInfo.Add("    - Người tạo: " + comment.CreatedByUserName);
            //         customerInfo.Add("      Thời gian: " + comment.CreatedOnDate.ToString("s"));
            //         customerInfo.Add("      Nội dung: '" + comment.Content + "'");
            //     }
            // }
        }

        return string.Join('\n', customerInfo);
    }
}