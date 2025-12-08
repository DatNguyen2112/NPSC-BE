using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSPC.Business.Services.WorkItem;

namespace NSPC.Business.Services
{
    public class MidnightTaskService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;


        public MidnightTaskService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //5 dây chạy 1 lần
                //await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                var now = DateTime.Now;
                var nextMidnight = now.Date.AddDays(1);
                var delay = nextMidnight - now;

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<TaskHandler>();
                        await handler.CheckAndNotifyTasksExpiringSoon();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi chạy TaskNotification: {ex.Message}");
                }
            }
        }
    }
}