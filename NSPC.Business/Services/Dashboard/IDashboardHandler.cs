using NSPC.Common;

namespace NSPC.Business.Services.Dashboard;

public interface IDashboardHandler
{
    public Task<Response<DashboardViewModel>> RevenueDashboard(DashboardFilter filter);
    public Task<Response<DashboardViewModel>> Top5CustomerWithTotalAmountDashboard(DashboardFilter filter);
    public Task<Response<DashboardViewModel>> ReturnedOrdersDashboard(DashboardFilter filter);
    public Task<Response<DashboardViewModel>> TotalReturnedAmountDashBoard(DashboardFilter filter);
    public Task<Response<DashboardViewModel>> TotalAmountDashboard(DashboardFilter filter);
    public Task<Response<DashboardViewModel>> OrderStatusDashBoard();
    public Task<Response<DashboardViewModel>> QuantityValueQuotationDashBoard();
    public Task<Response<DashboardViewModel>> QuantityPercentQuotationDashBoard(DashboardFilter filter);
    public Task<Response<DashboardViewModel>> Top5CustomersInDebtDashboard(DashboardFilter filter);
}