using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_An_Dua_MVC.Models
{
    // 📒 Bảng GIAO DỊCH: Nơi ghi chép sổ sách làm ăn hằng ngày.
    public class GiaoDich
    {
        // 🔑 Mã hóa đơn (Duy nhất).
        [Key]
        public int Id { get; set; }

        // --- PHẦN 1: DỮ LIỆU THÔ (Raw Data) ---

        [Display(Name = "Ngày Giao Dịch")]
        [Required(ErrorMessage = "Ngày giao dịch không được để trống")]
        // 🕒 Tự động lấy giờ hiện tại (DateTime.Now) khi vừa mở form.
        // Giúp Cha đỡ phải gõ ngày tháng thủ công, tiết kiệm thời gian.
        public DateTime NgayGiaoDich { get; set; } = DateTime.Now;

        [Display(Name = "Là Mua Hàng?")]
        // 💡 Công tắc bật/tắt:
        // TRUE = Mình mua vào (Tốn tiền, Tăng dừa).
        // FALSE = Mình bán ra (Thu tiền, Giảm dừa).
        public bool IsMuaHang { get; set; } = true;

        [Display(Name = "Số Lượng (Trái)")]
        [Required(ErrorMessage = "Hãy nhập số lượng")]
        // 🛡️ Ông bảo vệ: Cấm nhập số âm hoặc số 0 (Không ai giao dịch 0 trái dừa cả).
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải từ 1 trái trở lên")]
        public int SoLuong { get; set; }

        [Display(Name = "Đơn Giá (VNĐ)")]
        [Required(ErrorMessage = "Hãy nhập đơn giá")]
        // 🛡️ Ông bảo vệ: Giá tiền không được âm. (Nhưng cho phép = 0 trong trường hợp khuyến mãi/tặng).
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm")]
        public double DonGia { get; set; }

        [Display(Name = "Thành Tiền")]
        // 💰 Biến này để "Lưu kết quả" (Số Lượng * Đơn Giá).
        // Lưu ý: Class này chỉ chứa dữ liệu, việc tính toán nhân chia sẽ do Controller làm.
        public double ThanhTien { get; set; }

        [Display(Name = "Ghi Chú")]
        public string? GhiChu { get; set; } // Cho phép rỗng (không bắt buộc ghi).

        // --- PHẦN 2: LIÊN KẾT THẦN THÁNH (RELATIONSHIPS) ---
        // 🧠 Tư duy: Giao dịch này không đứng một mình, nó dính líu tới "Ai" và "Cái gì".

        // 🔗 Sợi dây 1: Nối sang bảng Đối Tác (Người mua/bán)
        [Display(Name = "Đối Tác")]
        [Required(ErrorMessage = "Vui lòng chọn Đối tác")]
        // 1. Cái Khóa (Lưu trong SQL): Chỉ lưu con số (Ví dụ: 5). Nhẹ, tốn ít bộ nhớ.
        public int DoiTacId { get; set; }

        // 2. Cái Cửa Thần Kỳ (Virtual):
        // Không lưu trong SQL. Chỉ dùng trong Code C#.
        // Khi em gọi biến này, Entity Framework tự động "nhảy" sang bảng DoiTac,
        // tìm thằng có Id=5 và bê nguyên thông tin (Tên, SĐT...) về cho em dùng.
        [ForeignKey("DoiTacId")]
        public virtual DoiTac? DoiTac { get; set; }


        // 🔗 Sợi dây 2: Nối sang bảng Loại Dừa (Hàng hóa)
        [Display(Name = "Loại Dừa")]
        [Required(ErrorMessage = "Vui lòng chọn Loại dừa")]
        // 1. Cái Khóa (Lưu con số ID).
        public int LoaiDuaId { get; set; }

        // 2. Cái Cửa Thần Kỳ (Để lấy tên dừa, ảnh dừa...).
        [ForeignKey("LoaiDuaId")]
        public virtual LoaiDua? LoaiDua { get; set; }
    }
}