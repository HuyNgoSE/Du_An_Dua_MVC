using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Du_An_Dua_MVC.Data;
using Du_An_Dua_MVC.Models;
using Microsoft.AspNetCore.Authorization; // 👈 Nhớ thêm dòng này

namespace Du_An_Dua_MVC.Controllers
{
    // 👇 Dán bùa vào đây! Người lạ không được xem sổ sách làm ăn.
    [Authorize]
    public class GiaoDichesController : Controller
    {
        private readonly DuAnDuaDbContext _context;

        public GiaoDichesController(DuAnDuaDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // 📊 HÀNH ĐỘNG 1: TRANG CHỦ & BÁO CÁO (INDEX)
        // =========================================================
        // [NÂNG CẤP 12.5]: Thêm bộ lọc ngày (Smart Filter)
        public async Task<IActionResult> Index(DateTime? searchDate, string showAll)
        {
            // 1. TẠO CÂU LỆNH (Query) - Chưa chạy ngay!
            // Include: Kêu SQL lấy luôn tên Đối Tác và Loại Dừa (để đỡ bị null)
            var query = _context.GiaoDichs
                .Include(g => g.DoiTac)
                .Include(g => g.LoaiDua)
                .OrderByDescending(g => g.NgayGiaoDich) // Xếp đơn mới nhất lên đầu cho dễ thấy
                .AsQueryable(); // Chuyển sang chế độ "Chờ lệnh tiếp theo"

            // 2. XỬ LÝ LOGIC LỌC (FILTER)
            if (showAll == "true")
            {
                // Nếu bấm nút "Xem Tất Cả" -> Không lọc gì hết, lấy hết lịch sử
                ViewData["CurrentFilter"] = "Tất Cả";
            }
            else
            {
                // Mặc định: Nếu mới vào hoặc không chọn ngày -> Lấy HÔM NAY
                DateTime dateToFilter = searchDate ?? DateTime.Now.Date;

                // Lọc dữ liệu: Chỉ lấy các đơn hàng của ngày đó
                query = query.Where(g => g.NgayGiaoDich.Date == dateToFilter);

                // Gửi lại ngày đang chọn ra View để hiện lên ô lịch
                ViewData["CurrentDate"] = dateToFilter.ToString("yyyy-MM-dd");
                ViewData["CurrentFilter"] = "Ngày " + dateToFilter.ToString("dd/MM/yyyy");
            }

            // 3. TÍNH TOÁN TIỀN (AGGREGATE)
            // Máy tính sẽ cộng tiền dựa trên danh sách ĐÃ LỌC ở trên (query)

            // Tổng Thu: Cộng tiền những đơn BÁN ra (IsMuaHang == false)
            var tongThu = await query
                .Where(g => g.IsMuaHang == false)
                .SumAsync(g => g.ThanhTien);

            // Tổng Chi: Cộng tiền những đơn MUA vào (IsMuaHang == true)
            var tongChi = await query
                .Where(g => g.IsMuaHang == true)
                .SumAsync(g => g.ThanhTien);

            // Gửi con số tổng kết sang View để hiện lên mấy cái thẻ màu
            ViewBag.TongThu = tongThu;
            ViewBag.TongChi = tongChi;
            ViewBag.LoiNhuan = tongThu - tongChi; // Lời = Thu - Chi

            // 4. Bây giờ mới chạy lệnh xuống Database lấy danh sách về
            return View(await query.ToListAsync());
        }

        // =========================================================
        // 🔍 HÀNH ĐỘNG 2: XEM CHI TIẾT (DETAILS)
        // =========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin đơn hàng, kèm theo tên Khách và tên Dừa
            var giaoDich = await _context.GiaoDichs
                .Include(g => g.DoiTac)
                .Include(g => g.LoaiDua)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (giaoDich == null)
            {
                return NotFound();
            }

            return View(giaoDich);
        }

        // =========================================================
        // ➕ HÀNH ĐỘNG 3: NHẬP HÀNG/BÁN HÀNG (CREATE)
        // =========================================================

        // [GET]: Mở form nhập liệu
        // [UPDATE 11.9]: Có thêm tính năng "Ký ức" (Nhớ ngày & người vừa chọn)
        public IActionResult Create(int? lastDoiTacId, DateTime? lastNgayGiaoDich)
        {
            // 1. Đổ dữ liệu vào Dropdown (Danh sách xổ xuống)
            // lastDoiTacId: Giúp tự động chọn lại ông khách cũ (đỡ phải tìm lại)
            ViewData["DoiTacId"] = new SelectList(_context.DoiTacs, "Id", "TenDoiTac", lastDoiTacId);

            // Lấy danh sách Dừa
            ViewBag.ListLoaiDua = _context.DSLoaiDua.ToList();

            // 2. KHỞI TẠO GIÁ TRỊ MẶC ĐỊNH CHO FORM
            var giaoDichMacDinh = new GiaoDich();

            // --- LOGIC NHỚ NGÀY (STICKY DATE) ---
            if (lastNgayGiaoDich.HasValue)
            {
                giaoDichMacDinh.NgayGiaoDich = lastNgayGiaoDich.Value; // Dùng ngày cũ
            }
            else
            {
                giaoDichMacDinh.NgayGiaoDich = DateTime.Now; // Mặc định là hôm nay
            }

            // --- LOGIC NHỚ ĐỐI TÁC (STICKY CUSTOMER) ---
            if (lastDoiTacId.HasValue)
            {
                giaoDichMacDinh.DoiTacId = lastDoiTacId.Value;
            }

            // Các mặc định khác (để số 0 cho đẹp đội hình)
            giaoDichMacDinh.IsMuaHang = true; // Mặc định tick vào Mua
            giaoDichMacDinh.SoLuong = 0;
            giaoDichMacDinh.DonGia = 0;

            return View(giaoDichMacDinh);
        }

        // [POST]: Lưu đơn hàng vào Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [UPDATE 11.9]: Thêm tham số submitButton để biết người dùng bấm nút nào
        public async Task<IActionResult> Create([Bind("Id,NgayGiaoDich,IsMuaHang,SoLuong,DonGia,ThanhTien,GhiChu,DoiTacId,LoaiDuaId")] GiaoDich giaoDich, string submitButton)
        {
            if (ModelState.IsValid)
            {
                // --- 1. TÍNH TIỀN TỰ ĐỘNG ---
                giaoDich.ThanhTien = giaoDich.SoLuong * giaoDich.DonGia;

                // Đảm bảo tiền luôn dương (tránh nhập nhầm số âm)
                if (giaoDich.ThanhTien < 0)
                {
                    giaoDich.ThanhTien = Math.Abs((double)giaoDich.ThanhTien);
                }

                // =========================================================
                // ❤️ 2. LOGIC CẬP NHẬT KHO HÀNG (QUAN TRỌNG NHẤT)
                // =========================================================

                // Tìm loại dừa đang giao dịch trong kho
                var loaiDuaCanTim = await _context.DSLoaiDua.FindAsync(giaoDich.LoaiDuaId);

                if (loaiDuaCanTim != null)
                {
                    if (giaoDich.IsMuaHang == true)
                    {
                        // MUA VÀO -> Kho TĂNG lên
                        loaiDuaCanTim.SoLuongTon = loaiDuaCanTim.SoLuongTon + giaoDich.SoLuong;
                    }
                    else
                    {
                        // BÁN RA -> Kho GIẢM đi
                        loaiDuaCanTim.SoLuongTon = loaiDuaCanTim.SoLuongTon - giaoDich.SoLuong;
                    }

                    // Đóng dấu: Dữ liệu kho này đã bị thay đổi
                    _context.Update(loaiDuaCanTim);
                }

                // =========================================================
                // 3. LƯU TẤT CẢ VÀO DATABASE
                // =========================================================
                _context.Add(giaoDich); // Thêm giao dịch mới
                await _context.SaveChangesAsync(); // Lưu cả Giao dịch lẫn Kho hàng cùng lúc

                // --- 4. LOGIC ĐIỀU HƯỚNG THÔNG MINH ---

                // Nếu Cha bấm nút "LƯU & NHẬP TIẾP"
                if (submitButton == "SaveAndContinue")
                {
                    TempData["SuccessMessage"] = "Đã lưu phiếu! Mời nhập tiếp cho đối tác này.";

                    // Quay lại trang Create, nhưng kèm theo ID và Ngày vừa nhập (Sticky)
                    // Để Cha nhập liên tù tì mà không cần chọn lại tên người
                    return RedirectToAction(nameof(Create), new
                    {
                        lastDoiTacId = giaoDich.DoiTacId,
                        lastNgayGiaoDich = giaoDich.NgayGiaoDich
                    });
                }

                // Nếu bấm nút Lưu thường -> Về trang danh sách xem báo cáo
                TempData["SuccessMessage"] = "Đã nhập đơn hàng thành công rồi nha Cha!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi form -> Load lại trang cũ
            ViewData["DoiTacId"] = new SelectList(_context.DoiTacs, "Id", "TenDoiTac", giaoDich.DoiTacId);
            ViewData["LoaiDuaId"] = new SelectList(_context.DSLoaiDua, "Id", "Id", giaoDich.LoaiDuaId);
            return View(giaoDich);
        }

        // =========================================================
        // ✏️ HÀNH ĐỘNG 4: SỬA (EDIT) - Ít dùng nhưng vẫn cần
        // =========================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giaoDich = await _context.GiaoDichs.FindAsync(id);
            if (giaoDich == null) return NotFound();

            ViewData["DoiTacId"] = new SelectList(_context.DoiTacs, "Id", "TenDoiTac", giaoDich.DoiTacId);
            ViewData["LoaiDuaId"] = new SelectList(_context.DSLoaiDua, "Id", "Id", giaoDich.LoaiDuaId);
            return View(giaoDich);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NgayGiaoDich,IsMuaHang,SoLuong,DonGia,ThanhTien,GhiChu,DoiTacId,LoaiDuaId")] GiaoDich giaoDich)
        {
            if (id != giaoDich.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaoDich);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiaoDichExists(giaoDich.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoiTacId"] = new SelectList(_context.DoiTacs, "Id", "TenDoiTac", giaoDich.DoiTacId);
            ViewData["LoaiDuaId"] = new SelectList(_context.DSLoaiDua, "Id", "Id", giaoDich.LoaiDuaId);
            return View(giaoDich);
        }

        // =========================================================
        // ❌ HÀNH ĐỘNG 5: XÓA (DELETE) - Cẩn thận khi dùng
        // =========================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var giaoDich = await _context.GiaoDichs
                .Include(g => g.DoiTac)
                .Include(g => g.LoaiDua)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (giaoDich == null) return NotFound();

            return View(giaoDich);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giaoDich = await _context.GiaoDichs.FindAsync(id);
            if (giaoDich != null)
            {
                _context.GiaoDichs.Remove(giaoDich);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiaoDichExists(int id)
        {
            return _context.GiaoDichs.Any(e => e.Id == id);
        }
    }
}