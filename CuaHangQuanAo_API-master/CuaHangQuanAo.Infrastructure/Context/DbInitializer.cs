/* ==========================================================================
   DỮ LIỆU KHỞI TẠO HỆ THỐNG (SYSTEM SEED)
   ========================================================================== */
namespace CuaHangQuanAo.Infrastructure.Context
{
    public static class DbInitializer
    {
        public static void Initialize(CuaHangQuanAoDbContext context)
        {
            /*
            // Reset toàn bộ dữ liệu (Xóa sạch và đặt lại Identity)
            ResetDatabase(context); 
            
            // 1. Phân quyền người dùng
            var roles = new PhanQuyen[]
            {
                new PhanQuyen { TenQuyen = "Admin1", MoTa = "Chủ quản trang web (SuperAdmin)" },
                new PhanQuyen { TenQuyen = "Admin", MoTa = "Quản trị viên" },
                new PhanQuyen { TenQuyen = "Nhân viên", MoTa = "Người phụ trách bán hàng" },
                new PhanQuyen { TenQuyen = "Khách hàng", MoTa = "Người dùng cơ bản" }
            };
            context.PhanQuyens.AddRange(roles);
            context.SaveChanges();

            // 2. Danh sách tài khoản mẫu (Mật khẩu mặc định: 123456)
            string hashedPass = BCrypt.Net.BCrypt.HashPassword("123456");
            var users = new Users[]
            {
                new Users { TenDangNhap = "admin1", MatKhau = hashedPass, Email = "admin1@gmail.com", HoVaTen = "Trần Hữu Nam", PhanQuyenId = roles[0].Id },
                new Users { TenDangNhap = "admin", MatKhau = hashedPass, Email = "admin@gmail.com", HoVaTen = "Nguyễn Thị Lan", PhanQuyenId = roles[1].Id },
                new Users { TenDangNhap = "khach1", MatKhau = hashedPass, Email = "khach1@gmail.com", HoVaTen = "Phạm Hoàng Long", PhanQuyenId = roles[3].Id }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // 3. Danh mục sản phẩm (Loại hàng)
            var loaiSPs = new LoaiSP[]
            {
                new LoaiSP { TenLSP = "Áo Polo", MoTa = "Sản phẩm thời trang kinh điển" },
                new LoaiSP { TenLSP = "Áo Sơ Mi", MoTa = "Lịch lãm, chuyên nghiệp" },
                new LoaiSP { TenLSP = "Áo Khoác", MoTa = "Phong cách, ấm áp" },
                new LoaiSP { TenLSP = "Quần Âu", MoTa = "Sang trọng công sở" },
                new LoaiSP { TenLSP = "Quần Jean", MoTa = "Năng động, bền bỉ" }
            };
            context.LoaiSPs.AddRange(loaiSPs);
            context.SaveChanges();

            // 4. Các thương hiệu hợp tác
            var thuongHieus = new ThuongHieu[]
            {
                new ThuongHieu { TenTH = "Luxury Brand" },
                new ThuongHieu { TenTH = "Casual Wear" },
                new ThuongHieu { TenTH = "Business Class" }
            };
            context.ThuongHieus.AddRange(thuongHieus);
            context.SaveChanges();

            // 5. Cấu hình Màu sắc & Size theo từng nhóm hàng
            var allMauSacs = new List<MauSac>();
            var allSizes = new List<Size>();

            // --- Màu sắc cho nhóm Áo (Polo, Sơ Mi, Áo Khoác) ---
            var commonShirtColors = new[] {
                new { H = "#000000", N = "Đen" }, new { H = "#FFFFFF", N = "Trắng" },
                new { H = "#000080", N = "Xanh Navy" }, new { H = "#808080", N = "Xám Ghi" },
                new { H = "#ADD8E6", N = "Xanh Nhạt" }, new { H = "#FFC0CB", N = "Hồng Nhạt" }
            };
            foreach(var l in loaiSPs.Take(3)) {
                allMauSacs.AddRange(commonShirtColors.Select(c => new MauSac { MaMau = c.H, TenMau = c.N, LoaiId = l.Id }));
            }

            // --- Màu sắc cho nhóm Quần (Âu, Jean) ---
            var commonPantsColors = new[] {
                new { H = "#000000", N = "Đen" }, new { H = "#36454F", N = "Charcoal" },
                new { H = "#2F4F4F", N = "Xám Đậm" }, new { H = "#000080", N = "Navy Dark" },
                new { H = "#4B3621", N = "Nâu Đất" }, new { H = "#1A225E", N = "Xanh Indigo" }
            };
            foreach(var l in loaiSPs.Skip(3)) {
                allMauSacs.AddRange(commonPantsColors.Select(c => new MauSac { MaMau = c.H, TenMau = c.N, LoaiId = l.Id }));
            }

            context.MauSacs.AddRange(allMauSacs);

            // --- Kích cỡ cho nhóm Áo (S -> XXL) ---
            string[] shirtSizes = { "S", "M", "L", "XL", "XXL" };
            foreach(var l in loaiSPs.Take(3)) {
                allSizes.AddRange(shirtSizes.Select(s => new Size { TenSize = s, LoaiId = l.Id }));
            }

            // --- Kích cỡ cho nhóm Quần (Size số 28 -> 34) ---
            string[] pantsSizes = { "28", "29", "30", "31", "32", "33", "34" };
            foreach(var l in loaiSPs.Skip(3)) {
                allSizes.AddRange(pantsSizes.Select(s => new Size { TenSize = s, LoaiId = l.Id }));
            }

            context.Sizes.AddRange(allSizes);
            context.SaveChanges();

            // 6. Nạp Sản phẩm mẫu (Đã cào từ Aristino - 15 sản phẩm)
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "anhsp");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var productData = GetScrapedProductData();
            var rnd = new Random();

            foreach (var item in productData)
            {
                var matchedLoai = loaiSPs.FirstOrDefault(l => l.TenLSP == item.Category) ?? loaiSPs[0];
                var sp = new SanPham
                {
                    Ten = item.Name,
                    MoTa = $"Mẫu {item.Name} thiết kế cao cấp, chất liệu thoáng mát, bền màu, phù hợp với phong cách lịch lãm.",
                    GiaBan = item.Price,
                    GiaNhap = (int)(item.Price * 0.55),
                    KhuyenMai = rnd.Next(0, 10) > 7 ? (decimal)(item.Price * 0.1) : 0,
                    HuongDan = "Giặt máy chế độ nhẹ. Không dùng hóa chất tẩy.",
                    ThanhPhan = "Cotton Luxury Soft / Nano",
                    LoaiId = matchedLoai.Id,
                    ThuongHieuId = thuongHieus[rnd.Next(thuongHieus.Length)].Id,
                    NgayTao = DateTime.Now,
                    TrangThaiHoatDong = true,
                    GioiTinh = 0 // Nam
                };
                context.SanPhams.Add(sp);
                context.SaveChanges(); 

                // Tải và lưu ảnh
                for (int i = 0; i < item.ImageUrls.Count; i++)
                {
                    string fileName = $"prod_{sp.Id}_{i + 1}.jpg";
                    string localPath = DownloadImageSync(item.ImageUrls[i], uploadsPath, fileName);
                    context.AnhSanPhams.Add(new AnhSanPham { SanPhamId = sp.Id, TenAnhSP = localPath });
                }

                // Gán màu và size PHÙ HỢP VỚI LOẠI SP để có biến thể
                var mauOptions = allMauSacs.Where(m => m.LoaiId == matchedLoai.Id).ToList();
                var sizeOptions = allSizes.Where(s => s.LoaiId == matchedLoai.Id).ToList();

                if (mauOptions.Any() && sizeOptions.Any())
                {
                    // Mỗi sản phẩm cho 2-3 màu, mỗi màu có vài size
                    var pickedColors = mauOptions.OrderBy(x => Guid.NewGuid()).Take(rnd.Next(2, 4)).ToList();
                    foreach (var color in pickedColors)
                    {
                        var pickedSizes = sizeOptions.OrderBy(x => Guid.NewGuid()).Take(rnd.Next(2, 4)).ToList();
                        foreach (var size in pickedSizes)
                        {
                            context.SanPhamBienThes.Add(new SanPhamBienThe { 
                                SanPhamId = sp.Id, 
                                MauId = color.Id, 
                                SizeId = size.Id, 
                                SoLuongTon = rnd.Next(20, 100) 
                            });
                        }
                    }
                }
            }
            context.SaveChanges();
        }

        private static void ResetDatabase(CuaHangQuanAoDbContext context)
        {
            try
            {
                // Vô hiệu hóa tất cả ràng buộc khóa ngoại tạm thời
                context.Database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

                // Danh sách bảng theo thứ tự xóa an toàn (từ con đến cha)
                var tables = new[] { 
                    "ChiTietHoaDons", "SanPhamBienThes", "AnhSanPhams", "GioHangs", 
                    "HoaDons", "SanPhams", "LoaiSPs", "ThuongHieus", "MauSacs", "Sizes", 
                    "Users", "PhanQuyens"
                };

                foreach (var table in tables)
                {
                    // Xóa dữ liệu và Reset Identity (Vòng lặp này đảm bảo xóa trắng DB)
                    context.Database.ExecuteSqlRaw($"DELETE FROM {table}");
                    try {
                        context.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('{table}', RESEED, 0)");
                    } catch { /* Bảng không có identity thì bỏ qua  }
                }

                // Kích hoạt lại ràng buộc khóa ngoại
                context.Database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
                Console.WriteLine(">>> RESET DATABASE THÀNH CÔNG");
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(">>> LỖI RESET DB: " + ex.Message); 
                // Fallback: Xóa từng bảng một cách thủ công nếu lệnh SQL đặc biệt bị lỗi
                foreach (var table in tables)
                {
                    try { context.Database.ExecuteSqlRaw($"DELETE FROM [{table}]"); } catch { }
                }
            }
        }

        private static string DownloadImageSync(string url, string folderPath, string fileName)
        {
            try
            {
                var fullPath = Path.Combine(folderPath, fileName);
                // Nếu file đã tồn tại và không trống thì bỏ qua để tiết kiệm tài nguyên
                if (File.Exists(fullPath) && new FileInfo(fullPath).Length > 0) return "/uploads/anhsp/" + fileName;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var response = client.GetAsync(url).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        File.WriteAllBytes(fullPath, content);
                        return "/uploads/anhsp/" + fileName;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Lỗi tải ảnh {url}: {ex.Message}"); }
            return url;
        }

        private class ProductSeed
        {
            public string Category { get; set; } = "";
            public string Name { get; set; } = "";
            public int Price { get; set; }
            public List<string> ImageUrls { get; set; } = new List<string>();
        }

        private static List<ProductSeed> GetScrapedProductData()
        {
            return new List<ProductSeed>
            {
                // ÁO POLO
                new ProductSeed { Category = "Áo Polo", Name = "Áo Polo Nam Mercerized Cotton Regular Fit", Price = 1395000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc05250_copy_301f700bfcaf4e39a7f0eaf6a42da84e.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc05268_copy_45231824587c488f8b2c5a81c97363fb.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc05270_copy_7d569d31610f40ee8a25844358987937.jpg" 
                    } 
                },
                new ProductSeed { Category = "Áo Polo", Name = "Áo Polo Nam Kẻ Slim Fit", Price = 950000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/22_809333f436a047c884e262e452e1b55d.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/33_43178950ffce4b5d951a8d4ce0411e4a.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/11_8b9b51cf0e0a4598931e9913169b0b47.jpg" 
                    } 
                },
                new ProductSeed { Category = "Áo Polo", Name = "Áo Elite Polo Nam Xanh Kẻ Slim", Price = 910000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc02933_copy_0722078baf174627af5bce40de1755bd.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc02936_copy_72b642f12609469693e056109f96afc3.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc02940_copy_9849dc4387f844b68c28cd2e39c0d7b8.jpg" 
                    } 
                },

                // ÁO SƠ MI
                new ProductSeed { Category = "Áo Sơ Mi", Name = "Áo Sơ Mi Nam Kẻ Nâu ASS567", Price = 1100000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/22_177ef8b38d2e44f695b03c531fac3a18.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/33_9e0d7c01c9214806abe69ae8816627e0.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/11_9b9574ad6575413aa1945682a5cfc6b2.jpg" 
                    } 
                },
                new ProductSeed { Category = "Áo Sơ Mi", Name = "Áo Sơ Mi Nam Perfect Fit Trắng", Price = 950000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/11_69ea4730c6874f3a96b05f7bda9058d7.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/33_41a00f96cdb84d76b1293be556e70eac.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/22_6553fb90bb1f4457be678f3334c5448e.jpg" 
                    } 
                },
                new ProductSeed { Category = "Áo Sơ Mi", Name = "Áo Sơ Mi Nam Họa Tiết Bamboo", Price = 995000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc01412_copy_ea44eb7b0e124dfa87552ec45c89c456.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc01411_copy_7ef3b42d3bee49bc9857d5944c408adb.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc01404_copy_d63617c3caa8475a94cabd3af7ea859f.jpg" 
                    } 
                },

                // ÁO KHOÁC
                new ProductSeed { Category = "Áo Khoác", Name = "Áo Khoác 1 Lớp Nam Regular Fit", Price = 2750000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc07919_8dc1e4e776b6467d9c8eeb6bc8312b42_e872a4393fa549e4b4bdc34a32092764.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc07926_1e54922b7c594a8a84d2e94297032745_da89011c4bb142d2b9bac65a5955c0e8.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc07925_a6ecbdc813a5428b853969e36ce90cb6_0c3a6b43d7ff4b11b27b96dff2eaf4b2.jpg" 
                    } 
                },
                new ProductSeed { Category = "Áo Khoác", Name = "Áo Khoác Len Nam Họa Tiết Business", Price = 6850000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc08495_copy_e7562fd1a4374d69a3ed1fbfea2a76d2.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc08501_copy_4c196f9907fd4304bc25170a35c417ea.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc08510_copy_747df6da6bcb49d385e8b7a78691c042.jpg" 
                    } 
                },
                new ProductSeed { Category = "Áo Khoác", Name = "Áo Khoác Da Thật Premium Business", Price = 17500000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc08423_copy_469ee27944dc487eba886efc06691c3d_022a51bb2e2f48269fcff2a374d4fbed.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc08429_copy_f02d9b596593493aa0e19527630da552_0951b76b5b0145d0ba69c6e731060ef6.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc08424_copy_aa9b9213836b4b919b6b266d31ee6a38_55ac00d5230e4fd085e921f21ad242ab.jpg" 
                    } 
                },

                // QUẦN ÂU
                new ProductSeed { Category = "Quần Âu", Name = "Quần Âu Nam Regular Fit Xám", Price = 1250000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc05073_copy_00454d3dd1c447aa8ec2f57647bff6c0.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc05069_copy_ceb4ba16193742e793e612655956c610.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc05095_copy_1ca4dabfe0f843d88c09e3009f6bd9db.jpg" 
                    } 
                },
                new ProductSeed { Category = "Quần Âu", Name = "Quần Âu Nam Kẻ Wool Business", Price = 2500000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc01506_copy_fd1f94d870a24ceab5cc19b823012d0b_c32a38c3d34741a0855e9e025fce7c33.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc01504_copy_ac1d2da480bd43bca08c9274bd64a216_287cdd7ae0664137a74b433c866a3657.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc01501_copy_a441cb4fc86845d6b073b411de01ec9d_23ceca9258d74eed8ca41e0009b6e504.jpg" 
                    } 
                },
                new ProductSeed { Category = "Quần Âu", Name = "Quần Âu Nam Wool Business Navy", Price = 2500000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc01290_copy_0dce37c96f074631b704eea5bf2aee4f.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc01295_copy_6860078b08144a18b4c76dec8b0a3875.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc01308_copy_96916bb256f74713b9c30ba8b2762480.jpg" 
                    } 
                },

                // QUẦN JEAN
                new ProductSeed { Category = "Quần Jean", Name = "Quần Jeans Nam Hiệu Ứng Giặt Mài", Price = 1250000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc08060_3f23d42f550d44cb930e9cee2692424e.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc08061_859bb6d4c9694c8087f6211fd8bc6913.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc08059_13480135a7a1401fadf016cbe436969a.jpg" 
                    } 
                },
                new ProductSeed { Category = "Quần Jean", Name = "Quần Jeans Nam Business Regular", Price = 2800000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/dsc02706_74e7fafcf7e446ed83362eb877f5b471.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc02700_9e1b4697409849c4bc532448ce4bd4c0.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/dsc02704_2381c97f430d405ea1615849d8ae6789.jpg" 
                    } 
                },
                new ProductSeed { Category = "Quần Jean", Name = "Quần Jeans Nam Xanh Đậm Wash", Price = 1195000, 
                    ImageUrls = new List<string> { 
                        "https://cdn.hstatic.net/products/200000887901/xanh-cham-dam_6e47db8a5e2b4f3c83c93abce9efff09_e5235069ac294f0f95f2caa7e9ddb1e1.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/xanh-cham-dam-2_1d0ef2e6bf9b44f5a3de9eb0ba227cd7_f4a2c54c5db345e8a0c9c2017b74902b.jpg", 
                        "https://cdn.hstatic.net/products/200000887901/xanh-cham-dam-3_14e1dc6ab73c4727baf2b9c2c01623d6_3fcd88872d0b4ffab802008aa6550848.jpg" 
                    } 
                }
            };
        }*/
        }
    }
}
