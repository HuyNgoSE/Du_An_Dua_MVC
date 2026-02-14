using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Du_An_Dua_MVC.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangUser : Migration
    {
        // ==========================================
        // 🔐 HÀNH ĐỘNG: ĐI TỚI (UP) - Xây phòng bảo vệ
        // ==========================================
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Lệnh xây bảng tên là "User"
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    // Số thứ tự nhân viên (1, 2, 3...). Tự động tăng.
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Tên đăng nhập (Ví dụ: admin, cha_huy...)
                    // nvarchar(max): Cho viết dài thoải mái (nhưng hơi tốn bộ nhớ nha, sau này tối ưu sau)
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // Mật khẩu (Ví dụ: 123456)
                    // Lưu ý: Ở bản V1 này mình đang lưu mật khẩu trần (thấy rõ chữ). 
                    // Sau này lên Senior mình sẽ mã hóa nó thành chuỗi loằng ngoằng.
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // Tên thật (Ví dụ: Nguyễn Văn A) - Để hiện câu "Xin chào, bác A"
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // Chức vụ (Ví dụ: Admin, NhanVien)
                    // Cột này dùng để phân quyền: Admin được xóa, NhanVien chỉ được xem.
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    // Khóa chính (Mã số định danh duy nhất của nhân viên)
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        // ==========================================
        // 🔙 HÀNH ĐỘNG: ĐI LUI (DOWN)
        // ==========================================
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Đuổi việc hết, dẹp phòng bảo vệ (Xóa bảng User)
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}