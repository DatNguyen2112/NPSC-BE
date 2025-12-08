using NSPC.Common;

namespace NSPC.Business.Services.Dashboard;

public class DashboardViewModel
{
    public Object MetaObjects { get; set; }
    public Object DataObjects { get; set; }
}

public class DashboardForDataRevenueModel
{
    public string Code { get; set; }
    public Decimal Total { get; set; }
    public string Time { get; set; }
    public string Note { get; set; }
}

public class DailyReturnOrderCountModel
{
    public string Date { get; set; }
    public string OrderCode { get; set; }
}

public class DashboardForOrderStatusModel
{
    public string Type { get; set; }
    public string Name { get; set; }
    public object Data { get; set; }
}

public class DashboardFilter
{
    public DateTime?[] DateRange { get; set; }
    public string Type { get; set; }
}


