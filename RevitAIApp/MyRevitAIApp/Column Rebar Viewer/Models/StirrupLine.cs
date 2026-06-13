namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Vị trí 1 lớp thép đai trên mặt đứng cột.
   ///     Y tính từ chân cột (mm). IsConfinement = nằm trong vùng gia cường đầu cột.
   /// </summary>
   public readonly record struct StirrupLine(double Y, bool IsConfinement);
}
