using Du_An_Dua_MVC.Data;
using Microsoft.EntityFrameworkCore;
using Du_An_Dua_MVC.Models;
// 👇 Thư viện này là "Người phát vé" (Cookie)
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- 🔌 KẾT NỐI DATABASE ---
// Lấy chìa khóa trong két sắt (appsettings.json) để mở cửa SQL Server.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DuAnDuaDbContext>(options =>
    options.UseSqlServer(connectionString));

// =========================================================
// 👇 STAGE 14.3.a: THIẾT LẬP QUY TẮC AN NINH 👇
// =========================================================
// 🎫 Đây là khâu "In vé vào cổng" (Setup Service).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // 1. Nếu khách cố tình vào trang "Giao Dịch" mà chưa đăng nhập?
        // -> 🦶 ĐÁ văng về trang Login ngay lập tức!
        options.LoginPath = "/Account/Login";

        // 2. Độ bền của vé (Cookie).
        // -> Sau 20 phút không làm gì, vé tự hủy => Phải đăng nhập lại.
        // (Để lỡ Cha quên tắt máy đi ra ngoài, người khác không vào nghịch được).
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });
// =========================================================

var app = builder.Build();

// --- ĐOẠN SEED DATA (ĐÃ VIETSUB Ở BÀI TRƯỚC) ---
// Tự động đẻ ra Admin nếu chưa có.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi tạo Admin.");
    }
}

// Cấu hình báo lỗi khi Web chạy thật (Production).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép tải ảnh, CSS, JS...

app.UseRouting(); // Định vị bản đồ đường đi.

// =========================================================
// 👇 STAGE 14.3.b: BỐ TRÍ LÍNH GÁC (THỰC THI) 👇
// ⚠️ QUAN TRỌNG: Thứ tự này là BẮT BUỘC. Đảo lộn là toang.
// =========================================================

app.UseAuthentication();
// 👮‍♂️ LÍNH GÁC 1 (Kiểm Tra Danh Tính):
// "Anh là ai? Đưa vé (Cookie) tôi xem?"
// Nếu vé hợp lệ -> Cho qua. Nếu không -> Đuổi về LoginPath.

app.UseAuthorization();
// 👮‍♂️ LÍNH GÁC 2 (Kiểm Tra Quyền Hạn):
// "Anh vào được rồi, nhưng anh có quyền vào kho lấy tiền không?"
// (Hiện tại mình là Admin nên quyền to nhất).

// =========================================================

// Bản đồ mặc định: Vào web là nhảy tới Home/Index đầu tiên.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // 🚀 PHÓNG TÊN LỬA!