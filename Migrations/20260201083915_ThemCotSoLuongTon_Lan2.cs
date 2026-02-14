using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Du_An_Dua_MVC.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotSoLuongTon_Lan2 : Migration
    {
        // ==========================================
        // ⬆️ HÀNH ĐỘNG: ĐI TỚI (UP) - Nâng cấp bảng Dừa
        // ==========================================
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. SIẾT CHẶT QUY ĐỊNH GIÁ (Alter Column)
            // Trước đây: Giá tiền được phép để trống (null).
            // Bây giờ: Bắt buộc phải có giá (nullable: false).
            // defaultValue: 0m -> Nếu dừa nào chưa có giá, tự động điền số 0 vào (để không bị lỗi).
            migrationBuilder.AlterColumn<decimal>(
                name: "GiaThamKhao",
                table: "LoaiDua",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            // 2. THÊM CỘT TỒN KHO (Add Column) - QUAN TRỌNG NHẤT
            // Đây là nơi lưu con số "Còn bao nhiêu trái" sau mỗi lần mua bán.
            migrationBuilder.AddColumn<int>(
                name: "SoLuongTon",
                table: "LoaiDua",
                type: "int",
                nullable: false,
                defaultValue: 0); // Mặc định ban đầu là 0 trái
        }

        // ==========================================
        // ⬇️ HÀNH ĐỘNG: ĐI LUI (DOWN) - Undo
        // ==========================================
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nếu hối hận, xóa cột Tồn kho đi
            migrationBuilder.DropColumn(
                name: "SoLuongTon",
                table: "LoaiDua");

            // Trả lại quy định cũ: Cho phép giá tiền để trống
            migrationBuilder.AlterColumn<decimal>(
                name: "GiaThamKhao",
                table: "LoaiDua",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}