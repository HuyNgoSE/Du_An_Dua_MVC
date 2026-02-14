namespace Du_An_Dua_MVC.Models
{
    // 💡 LƯU Ý QUAN TRỌNG: Class này là DTO (Data Transfer Object) - hiểu nôm na là cái "Khay bưng bê".
    // Nó KHÔNG tạo ra bảng trong Database (SQL).
    // Nhiệm vụ duy nhất: Hứng kết quả tính toán phức tạp từ Database để bưng lên màn hình cho Cha xem.
    public class BaoCaoTonKho
    {
        // Tên loại dừa (Xiêm, Dứa, Lai...).
        // 🛡️ Kỹ thuật phòng thủ: Gán sẵn = "" (chuỗi rỗng) để tránh lỗi Null (CS8618)
        // Giống như đưa tờ giấy trắng trước, chưa viết gì cũng không sao, còn hơn là không có giấy.
        public string TenLoaiDua { get; set; } = string.Empty;

        // Số lượng trái dừa đang nằm "ế" trong kho chưa bán được.
        public int SoLuongTon { get; set; }

        // Con số Cha quan tâm nhất: Tổng tiền vốn đang bị "chôn" trong đống hàng tồn này.
        // (Để biết đường mà lo xả hàng thu hồi vốn).
        public decimal TongTienVon { get; set; }
    }
}