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
        b.WithOrigins("https://cuahangquanao-nt-clothing.onrender.com", "http://127.0.0.1:5500", "http://localhost:5500")
         .AllowAnyMethod()
         .AllowAnyHeader());
});

var app = builder.Build();

// [SỬA LỖI CORS]: UseCors nên được đặt ngay sau Build() và trước các middleware khác
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

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CuaHangQuanAoDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi Seed Data: " + ex.Message);
    }
}

app.Run();