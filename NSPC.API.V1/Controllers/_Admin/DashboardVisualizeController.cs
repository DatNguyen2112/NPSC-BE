using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business.Services.Dashboard;
using NSPC.Common;

namespace NSPC.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{api-version:apiVersion}/dashboard-visualize")]
[ApiExplorerSettings(GroupName = "Dashboard Visualize")]
public class DashboardVisualizeController : ControllerBase
{
    private readonly IDashboardHandler _dashboardHandler;

    public DashboardVisualizeController(IDashboardHandler dashboardHandler)
    {
        _dashboardHandler = dashboardHandler;
    }

    /// <summary>
    /// Sale revenue by time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("sales-revenue")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesRevenue([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.RevenueDashboard(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Top 5 customers by total amount and by time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("top-five-revenue")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTop5Customer([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.Top5CustomerWithTotalAmountDashboard(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Statistics of returned orders by customers over time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("returned-orders")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReturnedOrders([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.ReturnedOrdersDashboard(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Total value of returned orders (customer returned orders) over time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("value-returned-orders")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalReturnedAmount([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.TotalReturnedAmountDashBoard(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Total value paid over time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("total-amount")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalAmount([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.TotalAmountDashboard(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Statistics of number of orders by status
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("order-by-status")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrdersByStatus()
    {
        var result = await _dashboardHandler.OrderStatusDashBoard();
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Statistics of quantity and value of Quotes in all statuses over time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("value-quantity-quotes")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuantityQuotes()
    {
        var result = await _dashboardHandler.QuantityValueQuotationDashBoard();
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Statistics of quantity and percentage of Quotations with Orders over time
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("value-percentage-quotes")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPercentageQuotes([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.QuantityPercentQuotationDashBoard(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Top 5 customers with the most debt
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize, Route("top-five-debt")]
    [ProducesResponseType(typeof(Response<DashboardViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTop5Debt([FromQuery] string filter = "{}")
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        };
        var filterObject = JsonConvert.DeserializeObject<DashboardFilter>(filter, settings);
        var result = await _dashboardHandler.Top5CustomersInDebtDashboard(filterObject);
        return Helper.TransformData(result);
    }
}