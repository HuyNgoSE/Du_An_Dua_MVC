using Du_An_Dua_MVC.Data; // Đã khớp với namespace của con
using Du_An_Dua_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Để dùng lệnh .Include()
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Du_An_Dua_MVC.Controllers
{
    // 👇 Dán bùa vào đây! Cấm người lạ vào trang chủ.
    // Ai chưa đăng nhập mà mon men vào đây là bị đá văng về trang Login ngay.
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // 1. SỬA TÊN CONTEXT CHO ĐÚNG VỚI FILE CON GỬI
        private readonly DuAnDuaDbContext _context;

        // 2. SỬA LUÔN Ở HÀM KHỞI TẠO
        // Dependency Injection: Nhận chìa khóa kho (Context) và cuốn sổ nhật ký (Logger)
        public HomeController(ILogger<HomeController> logger, DuAnDuaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // =========================================================
        // 📊 TRANG CHỦ (DASHBOARD) - Nơi xem báo cáo nhanh
        // =========================================================
        public IActionResult Index()
        {
            // 1. Lấy dữ liệu (Kèm Loại Dừa)
            // Lệnh này lấy TOÀN BỘ lịch sử giao dịch từ trước đến nay.
            // .Include: Kêu nó lấy luôn cái tên loại dừa đi kèm (để đỡ bị null)
            var giaoDichs = _context.GiaoDichs.Include(g => g.LoaiDua).ToList();

            // 2. Tính toán Tồn Kho (LOGIC CHẠY BẰNG CƠM - RAM)
            // Thay vì tin vào con số lưu cứng trong bảng LoaiDua,
            // đoạn code này tính toán lại từ đầu dựa trên lịch sử giao dịch (để đối chiếu).
            var danhSachTonKho = giaoDichs
                // 🔥 FIX LỖI CS8602 Ở ĐÂY:
                // Ý nghĩa: Nếu giao dịch đó có Loại Dừa thì lấy Tên, 
                // còn nếu lỡ tay xóa mất loại dừa đó rồi (null) thì gọi tạm là "Dừa Lạ"
                .GroupBy(g => g.LoaiDua != null ? g.LoaiDua.TenLoai : "Dừa Lạ")
                .Select(nhom => new BaoCaoTonKho
                {
                    // Gán tên loại dừa (Key chính là cái tên đã Group ở trên)
                    TenLoaiDua = nhom.Key,

                    // 🧮 CÔNG THỨC TOÁN HỌC:
                    // Tồn Kho = Tổng Số Lượng MUA (Nhập) - Tổng Số Lượng BÁN (Xuất)
                    SoLuongTon = nhom.Where(g => g.IsMuaHang).Sum(g => g.SoLuong)
                               - nhom.Where(g => !g.IsMuaHang).Sum(g => g.SoLuong)
                })
                .ToList();

            // 3. Trả về View để vẽ biểu đồ hoặc bảng
            return View(danhSachTonKho);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Trang báo lỗi (Khi code sập thì nó chạy vào đây)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}