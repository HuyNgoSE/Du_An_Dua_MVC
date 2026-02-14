using Microsoft.AspNetCore.Mvc;
using Du_An_Dua_MVC.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Du_An_Dua_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly DuAnDuaDbContext _context;

        // 1. Khởi tạo: Nhận cái kho Database vào để dùng
        // (Dependency Injection - Mượn chìa khóa kho)
        public AccountController(DuAnDuaDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 👇 HÀNH ĐỘNG 1: HIỆN FORM ĐĂNG NHẬP (GET)
        // ==========================================
        // Khi người dùng gõ /Account/Login hoặc bị đá văng ra đây
        [HttpGet]
        public IActionResult Login()
        {
            // Kiểm tra: Nếu trong túi đã có "Thẻ Bài" (Cookie) rồi thì không cần đăng nhập nữa.
            // User.Identity: Biến toàn cục, chứa thông tin người đang lướt web.
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home"); // Đuổi về trang chủ làm việc đi
            }
            return View(); // Chưa có thẻ thì hiện cái View Login.cshtml cho điền
        }

        // ==========================================
        // 👇 HÀNH ĐỘNG 2: XỬ LÝ ĐĂNG NHẬP (POST)
        // ==========================================
        // Khi người dùng bấm nút "Đăng Nhập" trên màn hình
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. SOI ĐÈN: Quét trong Database xem có ông nào khớp Username & Password không?
            // FirstOrDefault: Tìm người đầu tiên khớp, không thấy thì trả về null.
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                // Sai thì báo lỗi
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu rồi cha ơi!";
                return View(); // Trả lại form để nhập lại
            }

            // 2. IN THẺ BÀI (Claims): Nếu đúng người -> Bắt đầu làm thủ tục cấp thẻ.
            // Claims giống như các dòng chữ in trên thẻ Nhân viên.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), // Dòng 1: Tên đăng nhập
                new Claim(ClaimTypes.Role, user.Role),     // Dòng 2: Chức vụ (Admin/NhanVien) -> Dùng để phân quyền sau này
                new Claim("FullName", user.FullName)       // Dòng 3: Tên thật (để hiện 'Xin chào, Huy')
            };

            // Tạo tấm thẻ danh tính từ các thông tin trên
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Cấu hình cho tấm thẻ
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // "Nhớ đăng nhập": Tắt trình duyệt mở lại vẫn còn (không bắt đăng nhập lại)
            };

            // 3. ĐÓNG DẤU MỘC (Quan trọng nhất): Lệnh này sẽ tạo ra Cookie và gửi về trình duyệt.
            // Từ giờ trình duyệt đi đâu cũng sẽ cầm theo cái Cookie này để Server biết là "Người nhà".
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // 4. Xong xuôi -> Mở cổng cho vào Trang Chủ
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // 👇 HÀNH ĐỘNG 3: ĐĂNG XUẤT (LOGOUT)
        // ==========================================
        public async Task<IActionResult> Logout()
        {
            // Xé bỏ thẻ bài (Xóa Cookie trong trình duyệt)
            // Lần sau vào lại sẽ thành "Người lạ"
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Quay về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }
    }
}