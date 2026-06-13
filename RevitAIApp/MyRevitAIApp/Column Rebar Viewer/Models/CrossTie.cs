namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Đai phụ hình chữ C (crosstie) trên mặt cắt — móc 1 thanh thép dọc sang mặt đối diện.
   ///     (X1,Y1) = tâm thanh được móc; (X2,Y2) = điểm cắm vào mặt đối diện (cùng trục).
   ///     IsVertical = chân đai chạy theo phương đứng (nối cạnh trên ↔ dưới); ngược lại nằm ngang.
   ///     AnchorBarIndex = index thanh trong <see cref="RebarSection2D.Bars"/> để toggle.
   ///     Đơn vị mm, gốc toạ độ tâm tiết diện.
   /// </summary>
   public readonly record struct CrossTie(
      double X1, double Y1,
      double X2, double Y2,
      bool IsVertical,
      int AnchorBarIndex);
}
