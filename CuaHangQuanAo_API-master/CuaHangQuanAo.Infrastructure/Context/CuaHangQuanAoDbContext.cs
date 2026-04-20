using CuaHangQuanAo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CuaHangQuanAo.Infrastructure.Context
{
    public class CuaHangQuanAoDbContext : DbContext
    {
        // Constructor để nhận cấu hình chuỗi kết nối từ tầng API truyền xuống
        public CuaHangQuanAoDbContext(DbContextOptions<CuaHangQuanAoDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<PhanQuyen> PhanQuyens { get; set; }
        public DbSet<LoaiSP> LoaiSPs { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }

        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<Size> Sizes { get; set; }

        public DbSet<SanPham> SanPhams { get; set; }

        public DbSet<HoaDon> HoaDons { get; set; }

        public DbSet<SanPhamBienThe> SanPhamBienThes { get; set; }
        public DbSet<AnhSanPham> AnhSanPhams { get; set; }

        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // [TÍNH NĂNG MỚI]: Thêm chỉ mục (Indexing) để tăng tốc độ truy vấn
            modelBuilder.Entity<SanPham>().HasIndex(p => p.Ten);
            modelBuilder.Entity<Users>().HasIndex(u => u.TenDangNhap).IsUnique(); 
            modelBuilder.Entity<LoaiSP>().HasIndex(l => l.TenLSP);
            modelBuilder.Entity<ThuongHieu>().HasIndex(t => t.TenTH);

            // TẮT TÍNH NĂNG XÓA DÂY CHUYỀN (CASCADE DELETE) ĐỂ TRÁNH LỖI VÒNG LẶP
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            // Ép tất cả các property kiểu decimal trong C# thành cột DECIMAL(18,2) trong SQL Server
            configurationBuilder.Properties<decimal>().HaveColumnType("decimal(18,2)");
        }
    }
}