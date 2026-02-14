using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Du_An_Dua_MVC.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangGiaoDich_v1 : Migration
    {
        // ==========================================
        // ⬆️ HÀNH ĐỘNG: ĐI TỚI (UP) - Xây bảng Giao Dịch
        // ==========================================
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaoDichs", // Tên bảng (Do em quên cấu hình ToTable bên Context nên nó có chữ 's')
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Tự tăng 1, 2, 3...

                    NgayGiaoDich = table.Column<DateTime>(type: "datetime2", nullable: false), // Lưu ngày giờ

                    // bit: Kiểu True/False trong SQL (0 hoặc 1)
                    IsMuaHang = table.Column<bool>(type: "bit", nullable: false),

                    SoLuong = table.Column<int>(type: "int", nullable: false),

                    // float: Số thực (lưu tiền). Lưu ý: decimal tốt hơn float cho tiền tệ, 
                    // nhưng float vẫn dùng được (chỉ là cẩn thận sai số nhỏ xíu).
                    DonGia = table.Column<double>(type: "float", nullable: false),
                    ThanhTien = table.Column<double>(type: "float", nullable: false),

                    // nvarchar(max): Cho phép ghi chú dài vô tận
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    // HAI CỘT QUAN TRỌNG NHẤT (CẦU NỐI):
                    DoiTacId = table.Column<int>(type: "int", nullable: false), // Nối sang bảng DoiTac
                    LoaiDuaId = table.Column<int>(type: "int", nullable: false) // Nối sang bảng LoaiDua
                },
                constraints: table =>
                {
                    // 1. Khóa chính (CMND của giao dịch)
                    table.PrimaryKey("PK_GiaoDichs", x => x.Id);

                    // 2. KHÓA NGOẠI 1 (Sợi dây nối sang Đối Tác)
                    table.ForeignKey(
                        name: "FK_GiaoDichs_DoiTac_DoiTacId",
                        column: x => x.DoiTacId, // Cột ở bảng này
                        principalTable: "DoiTac", // Bảng đích đến
                        principalColumn: "Id",    // Cột đích đến

                        // ReferentialAction.Cascade: CỰC NGUY HIỂM VÀ MẠNH MẼ
                        // Ý nghĩa: Nếu em xóa ông Đối Tác A, thì TẤT CẢ giao dịch của ông A cũng bị xóa theo.
                        // (Hiệu ứng Domino).
                        onDelete: ReferentialAction.Cascade);

                    // 3. KHÓA NGOẠI 2 (Sợi dây nối sang Loại Dừa)
                    table.ForeignKey(
                        name: "FK_GiaoDichs_LoaiDua_LoaiDuaId",
                        column: x => x.LoaiDuaId,
                        principalTable: "LoaiDua",
                        principalColumn: "Id",

                        // Cũng là hiệu ứng Domino: Xóa loại dừa -> Mất hết lịch sử mua bán loại đó.
                        onDelete: ReferentialAction.Cascade);
                });

            // TẠO MỤC LỤC (Index)
            // Giúp tìm kiếm siêu nhanh. Khi em tìm giao dịch của ông A, 
            // nó không cần lục tung cả bảng mà nhìn vào Index này là thấy ngay.
            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichs_DoiTacId",
                table: "GiaoDichs",
                column: "DoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichs_LoaiDuaId",
                table: "GiaoDichs",
                column: "LoaiDuaId");
        }

        // ==========================================
        // ⬇️ HÀNH ĐỘNG: ĐI LUI (DOWN)
        // ==========================================
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Chỉ cần xóa bảng GiaoDichs là xong (các khóa ngoại tự đứt)
            migrationBuilder.DropTable(
                name: "GiaoDichs");
        }
    }
}