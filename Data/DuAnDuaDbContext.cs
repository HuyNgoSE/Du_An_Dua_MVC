using Microsoft.EntityFrameworkCore;
using Du_An_Dua_MVC.Models;

namespace Du_An_Dua_MVC.Data
{
    // Đây là Class quan trọng nhất trong việc kết nối SQL.
    // Nó kế thừa từ DbContext (của Microsoft) để có các siêu năng lực truy vấn.
    public class DuAnDuaDbContext : DbContext
    {
        // Hàm khởi tạo (Constructor): Nhận cấu hình (chuỗi kết nối) từ bên ngoài nạp vào
        public DuAnDuaDbContext(DbContextOptions<DuAnDuaDbContext> options)
            : base(options)
        {
        }

        // =========================================================
        // 📂 KHU VỰC KHAI BÁO CÁC BẢNG (DbSet)
        // Đây là các "Cánh Cổng" để code C# chui vào lấy dữ liệu trong SQL
        // =========================================================

        // 1. Cổng vào bảng Loại Dừa
        // Đặt tên biến là DSLoaiDua -> Sau này code gõ _context.DSLoaiDua là ra
        public DbSet<LoaiDua> DSLoaiDua { get; set; }

        // 2. Cổng vào bảng Đối Tác
        public DbSet<DoiTac> DoiTacs { get; set; }

        // 3. Cổng vào bảng Giao Dịch (Sổ cái ghi chép)
        public DbSet<GiaoDich> GiaoDichs { get; set; }

        // 4. Cổng vào bảng User (Tài khoản đăng nhập)
        public DbSet<User> Users { get; set; }

        // =========================================================
        // 🛠️ KHU VỰC CẤU HÌNH CHI TIẾT (OnModelCreating)
        // Chỗ này giống như "Kiến trúc sư", quy định hình dáng bảng trong SQL
        // =========================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- CẤU HÌNH CHO LOẠI DỪA ---
            // Quy định 1: Tên bảng trong SQL là "LoaiDua" (số ít), thay vì mặc định nó tự đặt là "LoaiDuas"
            modelBuilder.Entity<LoaiDua>().ToTable("LoaiDua");

            // Quy định 2: Cột Giá Tiền phải là kiểu decimal(18,2)
            // (18 số tổng, 2 số lẻ) để tính tiền cho chính xác, không bị lỗi làm tròn.
            modelBuilder.Entity<LoaiDua>()
                .Property(p => p.GiaThamKhao)
                .HasColumnType("decimal(18,2)");

            // --- CẤU HÌNH CHO ĐỐI TÁC ---
            // Ép tên bảng trong SQL thành "DoiTac" (cho đẹp và chuyên nghiệp)
            modelBuilder.Entity<DoiTac>().ToTable("DoiTac");

            // --- CẤU HÌNH CHO USER ---
            // Ép tên bảng trong SQL thành "User"
            modelBuilder.Entity<User>().ToTable("User");

            // ⚠️ LƯU Ý NHỎ CỦA THẦY:
            // Em chưa cấu hình ToTable("GiaoDich") cho bảng GiaoDich.
            // Nên mặc định trong SQL nó sẽ tên là "GiaoDichs" (có chữ s).
            // Không sao cả, code vẫn chạy tốt, chỉ là tên bảng hơi khác quy chuẩn số ít chút thôi.
        }
    }
}