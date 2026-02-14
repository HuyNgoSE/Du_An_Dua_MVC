using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Du_An_Dua_MVC.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangDoiTac_V1 : Migration
    {
        // ==========================================
        // ⬆️ HÀNH ĐỘNG: ĐI TỚI (UP)
        // Khi em gõ lệnh: update-database
        // Nó sẽ chạy hàm này để "Xây nhà"
        // ==========================================
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Lệnh xây bảng "DoiTac"
            migrationBuilder.CreateTable(
                name: "DoiTac",
                columns: table => new
                {
                    // Cột ID:
                    // - int: Số nguyên
                    // - Identity 1,1: Tự động tăng (Người dùng ko cần nhập, máy tự đếm 1, 2, 3...)
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Cột Tên: Chứa chữ (nvarchar), tối đa 100 ký tự (đỡ tốn bộ nhớ)
                    TenDoiTac = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),

                    // Cột SDT: Tối đa 20 số, được phép để trống (nullable: true)
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),

                    // Cột Loại: Khách Hàng hay Nhà Cung Cấp
                    LoaiDoiTac = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    // Thiết lập KHÓA CHÍNH (Primary Key) là cột Id
                    // (Giống như số CMND, không được trùng)
                    table.PrimaryKey("PK_DoiTac", x => x.Id);
                });

            // 2. Lệnh xây bảng "LoaiDua"
            // (Có vẻ ở bản V1 này em tạo cùng lúc 2 bảng luôn)
            migrationBuilder.CreateTable(
                name: "LoaiDua",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Tự tăng

                    TenLoai = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // decimal(18,2): Số thực, dùng để lưu TIỀN cho chính xác
                    GiaThamKhao = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiDua", x => x.Id);
                });
        }

        // ==========================================
        // ⬇️ HÀNH ĐỘNG: ĐI LUI (DOWN)
        // Khi em gõ lệnh: remove-migration (để quay xe, hối hận)
        // Nó sẽ chạy hàm này để "Đập nhà", trả lại hiện trạng cũ
        // ==========================================
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nếu lùi lại thì phải XÓA sổ 2 cái bảng vừa tạo đi
            migrationBuilder.DropTable(
                name: "DoiTac");

            migrationBuilder.DropTable(
                name: "LoaiDua");
        }
    }
}