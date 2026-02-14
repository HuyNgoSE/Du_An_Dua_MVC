using Microsoft.EntityFrameworkCore;
using Du_An_Dua_MVC.Data;

namespace Du_An_Dua_MVC.Models
{
    // 🌱 Class này gọi là "Gieo Mầm" (Seed Data).
    // Nhiệm vụ: Tự động nạp dữ liệu mẫu khi ứng dụng chạy lần đầu tiên.
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            // Mở kết nối vào Database...
            using (var context = new DuAnDuaDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<DuAnDuaDbContext>>()))
            {
                // 1. CHỐT CHẶN KIỂM TRA (Logic quan trọng nhất)
                // Hỏi SQL: "Trong bảng User đã có ai chưa?"
                if (context.Users.Any())
                {
                    return;   // ✋ Có rồi thì thôi! Dừng lại ngay.
                              // Nếu thiếu dòng này: Mỗi lần em khởi động lại Web, nó sẽ đẻ ra thêm 1 ông Admin nữa (Trùng lặp).
                }

                // 2. TẠO "CHÌA KHÓA VẠN NĂNG"
                // Nếu bảng đang rỗng (Lần chạy đầu tiên), ta tạo một ông Admin mặc định.
                // Mục đích: Để có cái tài khoản mà đăng nhập vào hệ thống.
                context.Users.Add(new User
                {
                    Username = "admin",
                    Password = "123", // 🔑 Mật khẩu "sơ cua". Đăng nhập xong nhớ đổi, hoặc cứ để test cũng được.
                    FullName = "Quản Trị Viên",
                    Role = "Admin" // Vai trò to nhất
                });

                // 3. CHỐT ĐƠN
                // Lệnh này mới thực sự ghi dữ liệu xuống ổ cứng SQL Server.
                context.SaveChanges();
            }
        }
    }
}