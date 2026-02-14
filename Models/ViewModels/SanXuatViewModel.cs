using System;
using System.Collections.Generic;

namespace Du_An_Dua_MVC.ViewModels // Hoặc .ViewModels tùy em đặt
{
    // C class cha: Chứa thông tin chung
    public class SanXuatViewModel
    {
        // 1. Thông tin chung
        public DateTime NgaySanXuat { get; set; } = DateTime.Now;

        // 2. ĐẦU VÀO (Nguyên liệu sẽ bị trừ kho)
        public int NguyenLieuId { get; set; } // ID của loại dừa đem đi đập (VD: Dừa Khô)
        public int SoLuongVao { get; set; }   // Số trái đem đi đập

        // 3. ĐẦU RA (Thành phẩm thu được)
        // Dùng List để chứa được nhiều loại thành phẩm cùng lúc (Cơm, Nước, Gáo...)
        public List<ThanhPhamItem> DanhSachThanhPham { get; set; } = new List<ThanhPhamItem>();
    }

    // Class con: Đại diện cho từng dòng thành phẩm
    public class ThanhPhamItem
    {
        public int ThanhPhamId { get; set; } // ID của Cơm dừa/Nước dừa...
        public int SoLuongRa { get; set; }   // Số kg/lít thu được
        // Giá thành phẩm tạm thời lấy theo giá thị trường trong DB, không cần nhập ở đây cho rối
    }
}