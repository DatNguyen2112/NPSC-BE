using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi.Models;
using NSPC.API.V1.Auth;
using NSPC.Business;
using NSPC.Business.AssetLiquidationSheet;
using NSPC.Business.AutoMapper;
using NSPC.Business.Services;
using NSPC.Business.Services.AdvanceRequest;
using NSPC.Business.Services.BangTinhLuong;
using NSPC.Business.Services.BHXH;
using NSPC.Business.Services.Bom;
using NSPC.Business.Services.Business;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.CauHinhNhanSu;
using NSPC.Business.Services.ChamCong;
using NSPC.Business.Services.ChatBot;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.ConstructionActitvityLog;
using NSPC.Business.Services.ConstructionWeekReport;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.Dashboard;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.DuAn;
using NSPC.Business.Services.EInvoice;
using NSPC.Business.Services.Feedback;
using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.Investor;
using NSPC.Business.Services.InvestorType;
using NSPC.Business.Services.NhaCungCap;
using NSPC.Business.Services.NhomVatTu;
using NSPC.Business.Services.PhongBan;
using NSPC.Business.Services.ProjectTemplate;
using NSPC.Business.Services.QuanLyLaiXe;
using NSPC.Business.Services.QuanLyLoaiXe;
using NSPC.Business.Services.QuanLyPhuongTien;
using NSPC.Business.Services.Quotation;
using NSPC.Business.Services.StockTransaction;
using NSPC.Business.Services.TaskNotification;
using NSPC.Business.Services.TaskUsageHistory;
using NSPC.Business.Services.VatTu;
using NSPC.Business.Services.VehicleRequest;
//using NSPC.Business.Services.QuanLyPhieu;
using NSPC.Business.Services.WorkingDay;
using NSPC.Business.Services.WorkItem;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using OpenData.Common;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = ConfigCollection.Instance.GetConfiguration();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    //.WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Start NSPC API");
// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Custom Scheme";
    options.DefaultChallengeScheme = "Custom Scheme";
}).AddCustomAuth(o => { });
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddLogging();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSingleton<AutoMapper.IConfigurationProvider>(AutoMapperConfig.RegisterMappings());
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddDbContext<SMDbContext>((serviceProvider, options) =>
{
    var tenantService = serviceProvider.GetRequiredService<ITenantProvider>();
    var tenantId = tenantService.GetTenantId();
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLDatabase"))
           .ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>()
           .EnableSensitiveDataLogging();
});
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
// Services
//  services.AddCAP(this.Configuration);

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IDatabaseFactory, DatabaseFactory>();
builder.Services
    .AddHttpContextAccessor()
    .AddScoped<IEmailHandler, EmailHandler>()
    .AddScoped<IEmailService, EmailService>()
    .AddScoped<IParameterHandler, ParameterHandler>()
    .AddScoped<FacebookService, FacebookService>()
    .AddScoped<INavigationHandler, NavigationHandler>()
    .AddScoped<IAdminDashboardHandler, AdminDashboardHandler>()
    .AddScoped<INotificationHandler, NotificationHandler>()
    .AddScoped<ICodeTypeHandler, CodeTypeHandler>()
    .AddScoped<IPushNotificationHandler, PushNotificationHandler>()
    .AddScoped<IAddressHandler, AddressHandler>()
    .AddScoped<IPhongBanHandler, PhongBanHandler>()
    .AddScoped<IChucVuHandler, ChucVuHandler>()
    .AddScoped<INhomVatTuHandler, NhomVatTuHandler>()
    .AddScoped<IBomHandler, BomHandler>()
    .AddScoped<IVatTuHandler, VatTuHandler>()
    .AddScoped<IDuAnHandler, DuAnHandler>()
    .AddScoped<INhaCungCapHandler, NhaCungCapHandler>()
    .AddScoped<IPurchaseOrderHandler, PurchaseOrderHandler>()
    //.AddScoped<IQuanLyPhieuHandler, QuanLyPhieuHandler>()
    .AddScoped<IKiemKhoHandler, KiemKhoHandler>()
    .AddScoped<IThongKeHandler, ThongKeHandler>()
    .AddScoped<IWorkingDayHandler, WorkingDayHandler>()
    .AddScoped<IBHXHHandler, BHXHHandler>()
    .AddScoped<IChamCongHandler, ChamCongHandler>()
    .AddScoped<IBangTinhLuongHandler, BangTinhLuongHandler>()
    .AddScoped<ICauHinhNhanSuHandler, CauHinhNhanSuHandler>()
    .AddScoped<IDashboardHandler, DashboardHandler>()
     .AddScoped<ISearchSampleHandler, SearchSampleHandler>()
     .AddScoped<ICashbookTransactionHandler, CashbookTransactionHandler>()
     .AddScoped<IStockTransactionHandler, StockTransationHandler>()
     .AddScoped<ISalesOrderHandler, SalesOrderHandler>()
     .AddScoped<IQuotationHandler, QuotationHandler>()
     .AddScoped<IDebtTransactionHandler, DebtTransactionHandler>()
     .AddScoped<ICustomerReturnHandler, CustomerReturnHandler>()
     .AddScoped<ISupplierReturnHandler, SupplierReturnHandler>()
     // Inventory Note
     .AddScoped<IInventoryNoteHandler, InventoryNoteHandler>()
    // Inventory Check Note 
    .AddScoped<IInventoryCheckNoteHandler, InventoryCheckNoteHandler>()
    // WarehouseTransferNote
    .AddScoped<IWarehouseTransferNoteHandler, WarehouseTransferNoteHandler>()
    .AddScoped<IProductInventoryHandler, ProductInventoryHandler>()
    .AddScoped<ICustomerServiceCommentHandler, CustomerServiceCommentHandler>()
    // EInvoice
    .AddScoped<IEInvoiceHandler, EInvoiceHandler>()
    // Asset
    .AddScoped<IAssetHandler, AssetHandler>()
    .AddScoped<IAssetGroupHandler, AssetGroupHandler>()
    .AddScoped<IAssetTypeHandler, AssetTypeHandler>()
    .AddScoped<IAssetMaintenanceSheetHandler, AssetMaintenanceSheetHandler>()
    .AddScoped<IAssetLiquidationSheetHandler, AssetLiquidationSheetHandler>()
    .AddScoped<IAssetLocationHandler, AssetLocationHandler>()
    .AddScoped<IAssetUsageHistoryHandler, AssetUsageHistoryHandler>()
    .AddScoped<IMeasureUnitHandler, MeasureUnitHandler>()
    .AddScoped<IAssetAllocationHandler, AssetAllocationHandler>()
    // Chatbot
    .AddScoped<IChatBotHandler, ChatBotHandler>()
    .AddScoped<IApplicationHandler, ApplicationHandler>()
    .AddScoped<ITenantHandler, TenantHandler>()
    .AddScoped<IConstructionHandler, ConstructionHandler>()
    // Contract
    .AddScoped<IContractHandler, ContractHandler>()
    .AddScoped<IConstructionActivityLogHandler, ConstructionActivityLogHandler>()
    // Material Request
    .AddScoped<IMaterialRequestHandler, MaterialRequestHandler>()
    // Yêu cầu tạm ứng
    .AddScoped<IAdvanceRequestHandler, AdvanceRequestHandler>()
    // Task Management
    .AddScoped<ITaskManagementHandler, TaskManagementHandler>()
    // Góp ý
    .AddScoped<IFeedbackHandler, FeedbackHandler>()
    .AddScoped<IPhuongTienHandler, PhuongTienHandler>()
    .AddScoped<ILaiXeHandler, LaiXeHandler>()
    .AddScoped<IKhoHandler, KhoHandler>()
    .AddScoped<ILoaiXeHandler, LoaiXeHandler>()
    // Template dự án
    .AddScoped<IProjectTemplateHandler, ProjectTemplateHandler>()
    // Vehicle Request
    .AddScoped<IVehicleRequestHandler, VehicleRequestHandler>()

    // Social Media
    .AddScoped<ISocialMediaHandler, SocialMediaHandler>()

    // IssueManagement
    .AddScoped<IIssueManagementHandler, IssueManagementHandler>()

    //// IssueActivityLog
    .AddScoped<IIssueActivityLogHandler, IssueActivityLogHandler>()

    // Chủ đầu tư / Loại chủ đầu tư
    .AddScoped<InterfaceInvestorHandler, InvestorHandler>()
    .AddScoped<InterfaceInvestorTypeHandler, InvestorTypeHandler>()

    // Báo cáo tuần
    .AddScoped<IConstructionWeekReportHandler, ConstructionWeekReportHandler>()

    // Lịch sử sử dụng công việc dự án
    .AddScoped<ITaskUsageHistoryHandler, TaskUsageHistoryHandler>()

    // Thông báo công việc
    .AddScoped<ITaskNotificationHandler, TaskNotificationHandler>()
    .AddScoped<ITaskPersonalHandler, TaskPersonalHandler>()
#region Attachment

    .AddScoped<IAttachmentHandler, AttachmentHandler>()
    // Công việc
    .AddScoped<ITaskHandler, TaskHandler>()
    .AddScoped<ITaskCommentHandler, TaskCommentHandler>()
    .AddScoped<TaskHandler>()
#endregion Attachment

#region IDM

    // .AddScoped<INavigationHandler, NavigationHandler>()
    // .AddScoped<IEmailService, EmailService>()
    // .AddScoped<IEmailHandler, EmailHandler>()
    .AddScoped<IRoleHandler, RoleHandler>()
    .AddScoped<IUserHandler, UserHandler>()
    .AddScoped<IRightHandler, RightHandler>()
    .AddScoped<IRightMapRoleHandler, RightMapRoleHandler>()

#endregion

#region KhachHang
        .AddScoped<IKhachHangHandler, KhachHangHandler>()
        .AddScoped<ILichSuChamSocHandler, LichSuChamSocHandler>();
#endregion KhachHang

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
        options => options.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("Content-Disposition"));
});


builder.Services.AddApiVersioning(cfg =>
{
    cfg.DefaultApiVersion = new ApiVersion(1, 0);
    cfg.AssumeDefaultVersionWhenUnspecified = true;
    cfg.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(cfg =>
{
    cfg.GroupNameFormat = "'v'VVV";
    cfg.SubstituteApiVersionInUrl = true;
});
builder.Services.AddSwaggerGen(options =>
{
    var assembly = typeof(Program).Assembly;
    var assemblyProduct = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;

    options.DescribeAllParametersInCamelCase();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Sử dụng Authen JWT. VD: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });
    options.DocInclusionPredicate((name, api) => true);
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    var executingAssembly = Assembly.GetExecutingAssembly();
    var referencedProjectsXmlDocPaths = executingAssembly.GetReferencedAssemblies()
        .Where(assembly => assembly.Name != null &&
                           assembly.Name.StartsWith("BLANK", StringComparison.InvariantCultureIgnoreCase))
        .Select(assembly => Path.Combine(AppContext.BaseDirectory, $"{assembly.Name}.xml"))
        .Where(path => System.IO.File.Exists(path));
    foreach (var xmlDocPath in referencedProjectsXmlDocPaths)
    {
        options.IncludeXmlComments(xmlDocPath);
    }
});
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddMemoryCache();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AttachmentUploadedMessage>());
builder.Services.AddHostedService<MidnightTaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelRendering(ModelRendering.Model);
    options.DisplayRequestDuration();
    options.DocExpansion(DocExpansion.None);

    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
    options.EnableValidator();
    options.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head, SubmitMethod.Post, SubmitMethod.Delete,
        SubmitMethod.Put);
    options.InjectStylesheet("/swagger.css");
});

app.UseRouting();
app.UseCors("AllowOrigin"); // Apply the CORS policy   -----> fix lỗi trên FE không get được res.headers.get('Content-Disposition')
app.UseCors(options => options.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
