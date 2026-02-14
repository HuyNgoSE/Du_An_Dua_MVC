namespace Du_An_Dua_MVC.Models
{
    // 🚑 Class này là "Cái Bảng Cáo Phó".
    // Khi Web bị sập (Lỗi 500, 404...), nó sẽ cầm thông tin lỗi chạy ra màn hình thông báo cho người dùng biết.
    public class ErrorViewModel
    {
        // 🆔 "Mã Số Vụ Án" (Trace Identifier).
        // Ví dụ: Khi web sập, server sinh ra một mã loằng ngoằng: "00-abc-123".
        // Em copy mã này, đưa cho thợ code (chính là em), em tra trong Log server là biết ngay lỗi ở đâu.
        public string? RequestId { get; set; }

        // 💡 Logic hiển thị thông minh (Expression Body).
        // Dịch: "Nếu có Mã Số Vụ Án (RequestId) thì trả về TRUE (Cho hiện ra)".
        // "Nếu không có mã (Null hoặc Rỗng) thì trả về FALSE (Giấu đi cho đỡ chật màn hình)".
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}