using CuaHangQuanAo.Application.Interfaces;
using CuaHangQuanAo.Application.Services;
using CuaHangQuanAo.Infrastructure.Context;
using CuaHangQuanAo.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
       
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

// Giữ nguyên ánh xạ mặc định để tương thích tốt nhất với ClaimTypes
// JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


// Cấu hình Swagger có nút nhập Token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CuaHangQuanAo API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Dán Token vào đây (Không cần chữ Bearer)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
/*
// Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CuaHangQuanAoDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("CuaHangQuanAo.Infrastructure")));
*/
// Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CuaHangQuanAoDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 30)), // Khuyên dùng: Hardcode version để tránh lỗi AutoDetect khi startup
        b => b.MigrationsAssembly("CuaHangQuanAo.Infrastructure")
    )
);
// Cấu hình Authentication & JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Đọc chuẩn 3 thông tin từ appsettings.json
    var secretKey = builder.Configuration["JwtSettings:SecretKey"];
    var issuer = builder.Configuration["JwtSettings:Issuer"];
    var audience = builder.Configuration["JwtSettings:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),

        RoleClaimType = ClaimTypes.Role, // Quay lại dùng chuẩn ClaimTypes.Role
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

// Đăng ký Services (Dependency Injection)
builder.Services.AddScoped(typeof(CuaHangQuanAo.Domain.Interfaces.IGenericRepository<>), typeof(CuaHangQuanAo.Infrastructure.Repositories.GenericRepository<>));
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<ILoaiSPService, LoaiSPService>();
builder.Services.AddScoped<IThuongHieuService, ThuongHieuService>();
builder.Services.AddScoped<IMauSacService, MauSacService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ISanPhamBienTheService, SanPhamBienTheService>();
builder.Services.AddScoped<IAnhSanPhamService, AnhSanPhamService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPhanQuyenService, PhanQuyenService>();
builder.Services.AddScoped<IGioHangService, GioHangService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVnpayService, VnpayService>();

builder.Services.AddCors(options => {
    options.AddPolicy("FrontendPolicy", b => 
        b.AllowAnyOrigin() // TẠM THỜI: Cho phép tất cả để chẩn đoán lỗi 500
         .AllowAnyMethod()
         .AllowAnyHeader());
});

var app = builder.Build();

// [SỬA LỖI]: Thêm middleware bắt lỗi toàn cục để hiện log trên Render
app.Use(async (context, next) => {
    try {
        await next();
    } catch (Exception ex) {
        Console.WriteLine($"[CRITICAL ERROR]: {ex.Message}");
        Console.WriteLine($"[STACK TRACE]: {ex.StackTrace}");
        
        context.Response.StatusCode = 500;
        context.Response.Headers.Append("Access-Control-Allow-Origin", "*"); // Đảm bảo lỗi vẫn có header CORS
        await context.Response.WriteAsJsonAsync(new { 
            error = "Internal Server Error", 
            message = ex.Message,
            details = app.Environment.IsDevelopment() ? ex.StackTrace : "Xem log trên Render dashboard"
        });
    }
});

app.UseCors("FrontendPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();


// Bắt buộc theo đúng thứ tự này
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Data & Diagnostic Check
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CuaHangQuanAoDbContext>();
    
    try
    {
        Console.WriteLine("--- DIAGNOSTIC: Checking DB Connection ---");
        if (context.Database.CanConnect()) {
            Console.WriteLine("--- DIAGNOSTIC: DB Connection SUCCESSFUL ---");
            
            Console.WriteLine("--- DIAGNOSTIC: Running Migrations ---");
            context.Database.Migrate(); // Tự động tạo bảng nếu chưa có
            Console.WriteLine("--- DIAGNOSTIC: Migrations Completed ---");
        } else {
            Console.WriteLine("--- DIAGNOSTIC: DB Connection FAILED (CanConnect returned false) ---");
        }
        
        DbInitializer.Initialize(context);
        Console.WriteLine("--- DIAGNOSTIC: Seed Data Completed ---");
    }
    catch (Exception ex)
    {
        Console.WriteLine("--- DIAGNOSTIC: ERROR ---");
        Console.WriteLine("Lỗi Database: " + ex.Message);
        if (ex.InnerException != null) Console.WriteLine("Inner Error: " + ex.InnerException.Message);
        Console.WriteLine("Stack Trace: " + ex.StackTrace);
    }
}

app.Run();