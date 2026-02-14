using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

// QUAN TRỌNG: Namespace phải có đuôi .Models (Để hệ thống biết tìm ở ngăn tủ nào).
namespace Du_An_Dua_MVC.Models
{
    // 🥥 Class này định nghĩa "Một loại dừa là gì?".
    public class LoaiDua
    {
        // 1. Khóa chính (Primary Key)
        // SQL Server dùng số này để quản lý (Dừa Xiêm là số 1, Dừa Dứa là số 2...).
        public int Id { get; set; }

        // 2. Tên loại dừa
        [DisplayName("Tên Loại Dừa")] // Cái nhãn dán lên màn hình cho đẹp.
        [Required(ErrorMessage = "Tên dừa không được để trống")] // 👮‍♂️ Lính gác: Không cho phép tạo dừa "Vô Danh".
        public string TenLoai { get; set; } = string.Empty; // 💉 Tiêm vaccine chống lỗi Null.

        // 3. Giá tiền
        [DisplayName("Giá Tham Khảo")]
        // Tại sao là "Tham Khảo"? Vì giá thị trường lên xuống thất thường.
        // Đây chỉ là giá gợi ý mặc định, lúc bán có thể sửa lại.
        public double GiaThamKhao { get; set; }

        // --- 🏆 NGÔI SAO SÁNG (Micro-win Stage 12) ---
        // 4. Số Lượng Tồn
        // Ý nghĩa: Đây là cái "Đồng hồ nước".
        // - Khi MUA vào: Cộng thêm vào đây.
        // - Khi BÁN ra: Trừ bớt đi.
        // Mặc định = 0 (Mới tạo ra chưa có trái nào).
        [DisplayName("Số Lượng Tồn")]
        public int SoLuongTon { get; set; } = 0;
    }
}