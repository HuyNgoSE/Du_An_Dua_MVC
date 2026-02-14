using Du_An_Dua_MVC.Data;
using Du_An_Dua_MVC.Models;
using Du_An_Dua_MVC.ViewModels;
using Du_An_Dua_MVC.NguoiHoTro; // ✅ Đã sửa đúng tên Namespace của em
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Du_An_Dua_MVC.Controllers
{
    public class SanXuatController : Controller
    {
        private readonly DuAnDuaDbContext _context;

        public SanXuatController(DuAnDuaDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 🏭 1. MỞ CỬA XƯỞNG (GET - Hiện Form)
        // ==========================================
        public IActionResult Index()
        {
            PrepareDropdowns(); // Chuẩn bị danh sách dừa để chọn

            // Tạo một phiếu sản xuất trắng
            var model = new SanXuatViewModel();

            // Tạo sẵn 5 dòng trống để nhập Thành Phẩm
            // (Cha em chỉ cần điền vào, đỡ phải bấm nút "Thêm dòng" nhiều lần)
            model.DanhSachThanhPham.Add(new ThanhPhamItem());
            model.DanhSachThanhPham.Add(new ThanhPhamItem());
            model.DanhSachThanhPham.Add(new ThanhPhamItem());
            model.DanhSachThanhPham.Add(new ThanhPhamItem()); // Mới thêm
            model.DanhSachThanhPham.Add(new ThanhPhamItem()); // Mới thêm

            return View(model);
        }

        // ==========================================
        // ⚙️ 2. VẬN HÀNH MÁY (POST - Xử lý)
        // ==========================================
        [HttpPost]
        public IActionResult Index(SanXuatViewModel model)
        {
            // --- BƯỚC 1: KIỂM TRA LOGIC (Sanity Check) ---
            // Tính tổng số lượng thành phẩm tạo ra
            var tongDauRa = model.DanhSachThanhPham.Sum(x => x.SoLuongRa);

            // Luật bất biến: Không thể tạo ra cái gì nhiều hơn cái mình bỏ vào
            // Ví dụ: Bỏ vào 10 trái dừa to -> Không thể ra 12 trái dừa nhỏ được.
            if (tongDauRa > model.SoLuongVao)
            {
                ModelState.AddModelError("", "⛔ Vô lý! Tổng thành phẩm lớn hơn nguyên liệu.");
                PrepareDropdowns();
                return View(model); // Trả lại form để sửa
            }

            // --- BƯỚC 2: GIAO DỊCH AN TOÀN (Transaction) ---
            // Bắt đầu chế độ "Được ăn cả, ngã về không".
            // Nếu có bất kỳ lỗi nào xảy ra ở giữa chừng, mọi thứ sẽ quay về như cũ.
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // A. XUẤT KHO NGUYÊN LIỆU (Giống như BÁN cho xưởng)
                    var xuatKho = new GiaoDich
                    {
                        NgayGiaoDich = model.NgaySanXuat,
                        DoiTacId = SystemConstants.ID_DOI_TAC_NOI_BO, // Người mua là "Xưởng Nội Bộ"
                        LoaiDuaId = model.NguyenLieuId,
                        SoLuong = model.SoLuongVao,

                        // 👇 QUAN TRỌNG: IsMuaHang = false (BÁN RA/XUẤT ĐI)
                        // Kho sẽ bị TRỪ đi số lượng này.
                        IsMuaHang = false,

                        DonGia = 0, // Chuyển nội bộ nên không tính tiền
                        ThanhTien = 0,
                        GhiChu = "Xuất nguyên liệu đi chế biến"
                    };
                    _context.Add(xuatKho); // 1. Lệnh trừ kho nguyên liệu

                    // B. NHẬP KHO THÀNH PHẨM (Giống như MUA từ xưởng)
                    foreach (var item in model.DanhSachThanhPham)
                    {
                        // Chỉ lưu những dòng có nhập số lượng ( > 0)
                        if (item.SoLuongRa > 0)
                        {
                            var nhapKho = new GiaoDich
                            {
                                NgayGiaoDich = model.NgaySanXuat,
                                DoiTacId = SystemConstants.ID_DOI_TAC_NOI_BO, // Người bán là "Xưởng Nội Bộ"
                                LoaiDuaId = item.ThanhPhamId,
                                SoLuong = item.SoLuongRa,

                                // 👇 QUAN TRỌNG: IsMuaHang = true (MUA VÀO/NHẬP VỀ)
                                // Kho sẽ được CỘNG thêm số lượng này.
                                IsMuaHang = true,

                                DonGia = 0,
                                ThanhTien = 0,
                                GhiChu = "Thu thành phẩm sau chế biến"
                            };
                            _context.Add(nhapKho); // 2. Lệnh cộng kho thành phẩm
                        }
                    }

                    // C. CHỐT ĐƠN (COMMIT)
                    _context.SaveChanges(); // Lưu tất cả các lệnh trên vào Database
                    transaction.Commit();   // Đóng dấu xác nhận: "Giao dịch thành công!"

                    TempData["Success"] = "✅ Đã chế biến thành công!";
                    return RedirectToAction("Index"); // Quay về trang ban đầu
                }
                catch (Exception ex)
                {
                    // 🚨 CÓ BIẾN! (Lỗi hệ thống, mất điện, rớt mạng...)
                    transaction.Rollback(); // Hủy hết mọi lệnh nãy giờ (Trả lại nguyên hiện trạng)

                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                    PrepareDropdowns();
                    return View(model);
                }
            }
        }

        // Hàm phụ: Chuẩn bị dữ liệu cho Dropdown
        private void PrepareDropdowns()
        {
            ViewBag.NguyenLieuList = new SelectList(_context.DSLoaiDua, "Id", "TenLoai");
            ViewBag.ThanhPhamList = _context.DSLoaiDua.ToList();
        }
    }
}