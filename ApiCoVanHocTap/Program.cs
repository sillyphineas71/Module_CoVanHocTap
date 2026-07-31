using ApiCoVanHocTap.Common;
using ApiCoVanHocTap.Hubs;
using ApiCoVanHocTap.Middleware;
using ApiCoVanHocTap.Repositories;
using ApiCoVanHocTap.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// ============================================================
// 1. SERVICES REGISTRATION
// ============================================================

// --- HttpContextAccessor ---
// QUAN TRỌNG: Bắt buộc phải đăng ký trước ServiceWrapper
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// --- Controllers ---
builder.Services.AddControllers(options =>
{
    // Bật MustLogged filter khi đã implement xác thực JWT
    // options.Filters.Add<MustLogged>();
})
.AddJsonOptions(opts =>
{
    // Giữ nguyên tên field (không camelCase) – chuẩn của hệ thống công ty
    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// --- CORS ---
var corsOrigins = configuration["CorsWithOrigins"]?.Split(',') ?? ["http://localhost:3000"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins(corsOrigins)
              .AllowCredentials()                    // Bắt buộc cho SignalR WebSocket
              .WithExposedHeaders("Content-Disposition"); // Cho phép client đọc header khi download file
    });
});

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoVanHocTap API",
        Version = "v1",
        Description = "API hỗ trợ hệ thống Cố vấn học tập – .NET"
    });

    // Hỗ trợ nhập JWT token trực tiếp trên Swagger UI
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Format: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --- SignalR – Real-time Chat ---
builder.Services.AddSignalR(options =>
{
    // Bật chi tiết lỗi trong môi trường dev để dễ debug
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB
});

// Đăng ký CustomUserIdProvider để SignalR định tuyến message theo userId từ query string
// Client connect: wss://host/hubs/chat?userId=<userId>
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// --- Core services ---
// JwtTokenService không giữ state (chỉ đọc claim từ HttpContext truyền vào)
// → Singleton là an toàn và cho phép exception handler lấy được từ lúc khởi động.
ConfigurationHelper.RepositorysConfig(builder.Services);

// --- Hybrid DI: Repository + Service Wrapper ---
// Scoped: mỗi HTTP request nhận 1 instance riêng → hợp với Lazy Loading + HttpContext lifecycle.
ConfigurationHelper.ServicesConfig(builder.Services);

// --- Forwarded Headers (khi deploy sau reverse proxy Nginx/Apache/IIS) ---
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Xoá whitelist để trust tất cả proxy nội bộ
    options.KnownProxies.Clear();
});

// ============================================================
// 2. KHỞI TẠO APP SETTINGS TĨNH
// ============================================================
SerilogConfig.Configure(configuration);
AppSettings.Initialize(configuration);

var app = builder.Build();

// ============================================================
// 3. MIDDLEWARE PIPELINE
// ============================================================

// Phải đặt UseForwardedHeaders TRƯỚC mọi middleware khác
app.UseForwardedHeaders();

// Exception handler đặt ngoài cùng để bắt lỗi của mọi middleware/controller phía sau,
// trả về JSON lỗi thống nhất.
app.ConfigureExceptionHandler(app.Services.GetRequiredService<IJwtTokenService>());

// Ghi log request/response ra file (bật/tắt qua "EnableRequestLog" trong appsettings).
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger chỉ expose ở môi trường dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoVanHocTap API V1"));
}

// Serve file tĩnh từ Assets/Template (ví dụ: template Word/Excel)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Assets", "Template")),
    RequestPath = "/Assets/Template"
});

// Serve file tĩnh từ Assets/Upload (ảnh, tài liệu upload)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Assets", "Upload")),
    RequestPath = "/Assets/Upload"
});

app.UseHttpsRedirection();
app.UseRouting();

// UseCors PHẢI đặt sau UseRouting và trước UseAuthentication
app.UseCors("ClientPermission");

app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// 4. ENDPOINT MAPPING
// ============================================================

// Map tất cả API Controllers
app.MapControllers();

// Map SignalR Hub cho chat real-time
// Client kết nối: new HubConnectionBuilder().withUrl("/hubs/chat?userId=123").build()
app.MapHub<ChatHub>("/hubs/chat");

try
{
    app.Run();
}
finally
{
    // Đảm bảo ghi nốt log còn trong buffer khi app tắt.
    Serilog.Log.CloseAndFlush();
}
