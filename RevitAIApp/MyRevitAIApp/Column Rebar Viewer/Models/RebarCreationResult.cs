namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Kết quả tạo thép vào model: số phần tử tạo được từng loại + số lỗi + thông báo lỗi đầu tiên.
   ///     Dùng để hiển thị rõ stage nào hỏng (debug) thay vì 1 thông báo "internal error" chung chung.
   /// </summary>
   public sealed record RebarCreationResult(
      int Longitudinal,
      int Stirrups,
      int CrossTies,
      int Failures,
      string? FirstError)
   {
      public int Total => Longitudinal + Stirrups + CrossTies;
   }
}
