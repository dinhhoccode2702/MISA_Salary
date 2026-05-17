using MISA.Salary.API.Middleware;
using MISA.Salary.BL.Base;
using MISA.Salary.BL.Interfaces;
using MISA.Salary.BL.Services;
using MISA.Salary.DL.Base;
using MISA.Salary.DL.Interfaces;
using MISA.Salary.DL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// 1. Cấu hình Services (Dependency Injection)
// =============================================

// Lấy chuỗi kết nối MySQL từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Chưa cấu hình chuỗi kết nối 'DefaultConnection' trong appsettings.json");

// --- Đăng ký Repository (DL Layer) ---
// Truyền connectionString qua constructor
builder.Services.AddScoped<ISalaryCompositionRepository>(sp =>
    new SalaryCompositionRepository(connectionString));
builder.Services.AddScoped<ISalarySystemRepository>(sp =>
    new SalarySystemRepository(connectionString));
builder.Services.AddScoped<IOrganizationRepository>(sp =>
    new OrganizationRepository(connectionString));
builder.Services.AddScoped<IGridConfigRepository>(sp =>
    new GridConfigRepository(connectionString));

// --- Đăng ký Service (BL Layer) ---
builder.Services.AddScoped<ISalaryCompositionService, SalaryCompositionService>();
builder.Services.AddScoped<ISalarySystemService, SalarySystemService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IGridConfigService, GridConfigService>();

// --- Đăng ký Controllers ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Sử dụng camelCase cho JSON response
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Cho phép đọc number từ string
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });

// --- Cấu hình CORS cho Frontend (Vue.js chạy trên port 5173) ---
builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowVueDev", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// --- Swagger/OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MISA Salary API",
        Version = "v1",
        Description = "API quản lý danh mục Thành phần lương - AMIS Tiền Lương"
    });
});

// Cấu hình Dapper tự động map snake_case (DB) sang PascalCase (C#)
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// =============================================
// 2. Cấu hình Middleware Pipeline
// =============================================

// Swagger UI (chỉ bật ở môi trường Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MISA Salary API v1");
        c.RoutePrefix = "swagger"; // Truy cập tại /swagger
    });
}

// Middleware xử lý exception tập trung (PHẢI đặt trước UseRouting)
app.UseMiddleware<ExceptionMiddleware>();

// CORS
app.UseCors("AllowVueDev");

// Routing & Controllers
app.UseAuthorization();
app.MapControllers();

// Chạy ứng dụng
app.Run();
