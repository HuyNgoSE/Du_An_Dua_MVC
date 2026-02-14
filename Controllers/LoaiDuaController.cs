using Du_An_Dua_MVC.Data; // Để nhận diện cái chìa khóa
using Du_An_Dua_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq; // Để dùng lệnh .ToList()
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Du_An_Dua_MVC.Controllers
{
    // 👇 Dán bùa [Authorize] vào đây! Bảo vệ kho dừa khỏi người lạ.
    [Authorize]
    public class LoaiDuaController : Controller
    {
        // --- BẮT ĐẦU BƯỚC 4.2: KHỞI TẠO ---
        // 1. May cái túi (Biến private để dùng nội bộ)
        private readonly DuAnDuaDbContext _context;

        // 2. Lễ trao chìa khóa (Constructor)
        public LoaiDuaController(DuAnDuaDbContext context)
        {
            // 3. Cất chìa vào túi để lát nữa dùng
            _context = context;
        }
        // --- KẾT THÚC BƯỚC 4.2 ---

        // ==========================================
        // 📋 1. XEM DANH SÁCH (READ)
        // ==========================================
        // --- BẮT ĐẦU BƯỚC 4.3 ---
        public IActionResult Index()
        {
            // 1. Viết phiếu xuất kho: Lấy hết dừa trong database ra, ép thành danh sách (List)
            var danhSachDua = _context.DSLoaiDua.ToList();

            // 2. Giao hàng cho shipper: Đưa danh sách này sang bên View để hiển thị
            return View(danhSachDua);
        }
        // --- KẾT THÚC BƯỚC 4.3 ---

        // ==========================================
        // ➕ 2. THÊM MỚI (CREATE)
        // ==========================================

        // --- BẮT ĐẦU BƯỚC 5.2 (Mở Form trắng) ---
        // GET: LoaiDua/Create
        public IActionResult Create()
        {
            return View();
        }
        // --- KẾT THÚC BƯỚC 5.2 ---

        // --- BẮT ĐẦU BƯỚC 5.4 (Xử lý Lưu khi bấm nút) ---
        [HttpPost] // 1. Cái Tem này cực quan trọng: Nó báo hiệu hàm này dùng để NHẬN dữ liệu
        public IActionResult Create(LoaiDua dua) // Nhận về một đối tượng "Dừa" từ Form
        {
            // 2. Lệnh thêm vào hàng chờ (chưa lưu hẳn)
            _context.Add(dua);

            // 3. Lệnh CHỐT ĐƠN (Lưu xuống SQL Server) -> Không có dòng này là công cốc!
            _context.SaveChanges();

            // 4. Lưu xong thì đá người dùng về lại trang Danh Sách (Index) để nhìn thấy kết quả ngay
            return RedirectToAction("Index");
        }
        // --- KẾT THÚC BƯỚC 5.4 ---

        // ==========================================
        // ✏️ 3. CHỈNH SỬA (UPDATE)
        // ==========================================

        // --- PHẦN 1: MỞ FORM SỬA (GET) ---
        // GET: LoaiDua/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Tìm trái dừa cần sửa
            var loaiDua = await _context.DSLoaiDua.FindAsync(id);
            if (loaiDua == null) return NotFound();

            // Đưa thông tin cũ ra View để người dùng sửa
            return View(loaiDua);
        }

        // --- PHẦN 2: LƯU DỮ LIỆU ĐÃ SỬA (POST) ---
        // POST: LoaiDua/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống hack form
        public async Task<IActionResult> Edit(int id, LoaiDua dua)
        {
            // Kiểm tra bảo mật: ID trên URL phải khớp với ID của dữ liệu gửi lên
            if (id != dua.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dua); // Đánh dấu "Cần cập nhật"
                    await _context.SaveChangesAsync(); // Lưu xuống DB
                }
                catch (Exception)
                {
                    // Kiểm tra nếu dừa đã bị ai đó xóa mất tiêu rồi
                    if (!_context.DSLoaiDua.Any(e => e.Id == dua.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dua);
        }

        // ==========================================
        // ❌ 4. XÓA (DELETE)
        // ==========================================

        // --- BẮT ĐẦU BƯỚC 6.3 (Màn hình xác nhận xóa - GET) ---
        // GET: LoaiDua/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // 1. Kiểm tra ID đầu vào
            if (id == null) return NotFound();

            // 2. Tìm trái dừa trong kho
            var loaiDua = await _context.DSLoaiDua
                .FirstOrDefaultAsync(m => m.Id == id);

            // 3. Nếu không thấy thì báo lỗi
            if (loaiDua == null) return NotFound();

            // 4. Đưa thông tin sang View để hiện câu hỏi: "Bạn có chắc muốn xóa trái dừa X này không?"
            return View(loaiDua);
        }

        // --- BẮT ĐẦU BƯỚC 6.4 (Xử lý XÓA THẬT - POST) ---
        // POST: LoaiDua/Delete/5
        [HttpPost, ActionName("Delete")] // Mẹo: Dùng tên giả là "Delete" để khớp với Form bên View
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Tìm đối tượng cần xóa lần cuối
            var loaiDua = await _context.DSLoaiDua.FindAsync(id);

            // 2. Nếu tìm thấy thì thực hiện xóa
            if (loaiDua != null)
            {
                _context.DSLoaiDua.Remove(loaiDua); // Lệnh xóa khỏi bộ nhớ đệm
            }

            // 3. Cập nhật xuống SQL (Lúc này mới mất thật sự)
            await _context.SaveChangesAsync();

            // 4. Quay về danh sách
            return RedirectToAction(nameof(Index));
        }
        // --- KẾT THÚC BƯỚC 6.4 ---
    }
}