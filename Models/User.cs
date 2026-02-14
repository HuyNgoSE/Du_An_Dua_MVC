using System.ComponentModel.DataAnnotations;

namespace Du_An_Dua_MVC.Models
{
    // 👮‍♂️ Class này quản lý danh sách "Những người được phép vào nhà".
    // (Bảng User trong SQL).
    public class User
    {
        // [Key]: Số thứ tự, chứng minh thư nhân dân.
        [Key]
        public int Id { get; set; }

        // 1. TÊN ĐĂNG NHẬP
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")] // Không được vô danh.
        [Display(Name = "Tên đăng nhập")]
        // ⚠️ Kỹ thuật lạ: "= null!;"
        // Dịch nôm na: "Tôi thề với trình biên dịch là biến này chắc chắn sẽ có dữ liệu, đừng báo lỗi Null nữa".
        // (Đây là cách viết tắt để đỡ phải gán = "" thủ công).
        public string Username { get; set; } = null!;

        // 2. MẬT KHẨU
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        // 🎭 Mặt nạ bảo mật (QUAN TRỌNG):
        // Dòng này ra lệnh cho Web: "Khi người dùng gõ vào ô này, hãy biến chữ thành dấu chấm tròn (••••••)".
        // Để người ngồi cạnh không nhìn trộm được pass của Cha.
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;

        // 3. VAI TRÒ (Phân quyền)
        [Display(Name = "Vai trò")]
        // Mặc định gán luôn là "Admin".
        // Vì Coco-Web hiện tại chỉ có mình Cha (hoặc em) dùng, nên cứ cho quyền to nhất cho tiện.
        public string Role { get; set; } = "Admin";
    }
}