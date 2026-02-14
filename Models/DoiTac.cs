using System.ComponentModel.DataAnnotations;

namespace Du_An_Dua_MVC.Models
{
    // 🏗️ Class này sẽ biến hình thành Bảng 'DoiTac' trong SQL Server.
    // Chứa chung cả: Người bán dừa cho mình (Đầu vào) và Người mua dừa của mình (Đầu ra).
    public class DoiTac
    {
        // 🔑 Đây là CMND/CCCD trong thế giới Database.
        // Duy nhất, không trùng, tự động tăng (1, 2, 3...). Sống chết phải có.
        [Key]
        public int Id { get; set; }

        // [Display]: Cái "Mặt nạ" để hiển thị lên Web cho đẹp (User thấy chữ "Tên Đối Tác" thân thiện hơn là "TenDoiTac").
        [Display(Name = "Tên Đối Tác")]
        // [Required]: Ông bảo vệ khó tính. Quên nhập tên là nó chặn cửa ngay, hiện lỗi đỏ lòm.
        [Required(ErrorMessage = "Vui lòng nhập tên đối tác!")]
        // [StringLength]: Quy hoạch đất đai. Chỉ cho tối đa 100 ký tự, tránh mấy ông copy cả bài văn vào đây làm nặng Database.
        [StringLength(100, ErrorMessage = "Tên quá dài.")]
        // 👇 THUỐC TRỊ BỆNH (QUAN TRỌNG): Gán = "" (Chuỗi rỗng) ngay khi đẻ ra.
        // Ý nghĩa: Thề với trình biên dịch là "Biến này SẠCH, không bao giờ NULL". Tránh lỗi sập web ngớ ngẩn (NullReferenceException).
        public string TenDoiTac { get; set; } = "";

        [Display(Name = "Số Điện Thoại")]
        [StringLength(20)]
        // ❓ Dấu hỏi (?): Cho phép "Vô gia cư" (Null).
        // Ý nghĩa: Thực tế có ông nông dân không xài điện thoại, thì vẫn phải cho lưu chứ không bắt buộc.
        public string? SoDienThoai { get; set; } // Có dấu ? rồi nên không cần trị

        [Display(Name = "Loại Đối Tác")]
        [Required(ErrorMessage = "Phải chọn loại đối tác!")]
        // 👇 LOGIC CỐT LÕI: Phân loại để biết đường tính tiền.
        // Ví dụ: Lưu chữ "Mua" (Khách đem tiền tới) hoặc "Ban" (Mình trả tiền cho họ).
        // Cũng phải uống "Thuốc trị bệnh" (= "") để đảm bảo an toàn.
        public string LoaiDoiTac { get; set; } = "";
    }
}