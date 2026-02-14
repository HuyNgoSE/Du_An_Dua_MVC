using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Du_An_Dua_MVC.Data;
using Du_An_Dua_MVC.Models;
using Microsoft.AspNetCore.Authorization; // 👈 Nhớ thêm dòng này: Để dùng tính năng bảo vệ

namespace Du_An_Dua_MVC.Controllers
{
    // 👇 Dán bùa [Authorize] vào đây! 
    // Ý nghĩa: Ai muốn vào trang Quản lý Đối tác thì PHẢI ĐĂNG NHẬP trước.
    // Nếu chưa đăng nhập, tự động bị đá về trang Login.
    [Authorize]
    public class DoiTacsController : Controller
    {
        private readonly DuAnDuaDbContext _context;

        // Mượn chìa khóa kho Database
        public DoiTacsController(DuAnDuaDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. XEM DANH SÁCH (READ)
        // ==========================================
        // GET: DoiTacs
        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách đối tác trong kho và ném ra View để hiển thị bảng
            return View(await _context.DoiTacs.ToListAsync());
        }

        // ==========================================
        // 2. XEM CHI TIẾT (READ - DETAIL)
        // ==========================================
        // GET: DoiTacs/Details/5 (Số 5 là Id của đối tác muốn xem)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Không đưa Id thì biết xem ai? -> Lỗi
            }

            // Tìm đối tác có Id khớp với id truyền vào
            var doiTac = await _context.DoiTacs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doiTac == null)
            {
                return NotFound(); // Tìm không thấy -> Lỗi
            }

            return View(doiTac); // Tìm thấy -> Hiện thông tin chi tiết
        }

        // ==========================================
        // 3. TẠO MỚI (CREATE)
        // ==========================================

        // Bước 1: Mở cái Form trắng để nhập liệu (GET)
        // GET: DoiTacs/Create
        public IActionResult Create()
        {
            return View();
        }

        // Bước 2: Nhận dữ liệu từ Form gửi lên và lưu vào kho (POST)
        // POST: DoiTacs/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống hack (CSRF) - Bảo vệ form không bị giả mạo
        public async Task<IActionResult> Create([Bind("Id,TenDoiTac,SoDienThoai,LoaiDoiTac")] DoiTac doiTac)
        {
            // Kiểm tra dữ liệu hợp lệ (VD: Tên không được để trống)
            if (ModelState.IsValid)
            {
                _context.Add(doiTac); // 1. Xếp hàng vào kho
                await _context.SaveChangesAsync(); // 2. Lưu kho (Chốt đơn)

                // 💉 MŨI TIÊM 1: Thông báo Thêm Mới
                // TempData giúp hiện thông báo màu xanh "Thành công" bên View
                TempData["Success"] = "Đã thêm mới đối tác thành công!";

                return RedirectToAction(nameof(Index)); // Xong thì quay về danh sách
            }
            return View(doiTac); // Lỗi thì trả lại form để sửa
        }

        // ==========================================
        // 4. CHỈNH SỬA (UPDATE)
        // ==========================================

        // Bước 1: Mở Form cũ lên, điền sẵn thông tin cũ vào ô (GET)
        // GET: DoiTacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doiTac = await _context.DoiTacs.FindAsync(id); // Tìm ông cần sửa
            if (doiTac == null)
            {
                return NotFound();
            }
            return View(doiTac); // Đổ dữ liệu cũ vào View
        }

        // Bước 2: Nhận dữ liệu MỚI đè lên dữ liệu CŨ (POST)
        // POST: DoiTacs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenDoiTac,SoDienThoai,LoaiDoiTac")] DoiTac doiTac)
        {
            // Kiểm tra bảo mật: Id trên URL phải khớp với Id của dữ liệu gửi lên
            if (id != doiTac.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doiTac); // 1. Đóng dấu "Cần cập nhật"
                    await _context.SaveChangesAsync(); // 2. Lưu kho

                    // 💉 MŨI TIÊM 2: Thông báo Cập Nhật
                    TempData["Success"] = "Đã cập nhật thông tin thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Lỗi hiếm: 2 người cùng sửa 1 lúc (bỏ qua không cần quan tâm sâu lúc này)
                    if (!DoiTacExists(doiTac.Id))
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
            return View(doiTac);
        }

        // ==========================================
        // 5. XÓA (DELETE)
        // ==========================================

        // Bước 1: Hỏi lại "Bạn có chắc chắn muốn xóa không?" (GET)
        // GET: DoiTacs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doiTac = await _context.DoiTacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doiTac == null)
            {
                return NotFound();
            }

            return View(doiTac); // Hiện thông tin lên lần cuối để người dùng xác nhận
        }

        // Bước 2: Xóa thật (POST)
        // POST: DoiTacs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doiTac = await _context.DoiTacs.FindAsync(id);
            if (doiTac != null)
            {
                _context.DoiTacs.Remove(doiTac); // 1. Đóng dấu "Cần hủy"
            }

            await _context.SaveChangesAsync(); // 2. Lưu kho (Biến mất thật sự)

            // 💉 MŨI TIÊM 3: Thông báo Xóa
            TempData["Success"] = "Đã xóa đối tác thành công!";

            return RedirectToAction(nameof(Index));
        }

        // Hàm phụ: Kiểm tra xem đối tác có tồn tại không (dùng cho code Update)
        private bool DoiTacExists(int id)
        {
            return _context.DoiTacs.Any(e => e.Id == id);
        }
    }
}