namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Tâm 1 thanh thép dọc trên tiết diện (mm), gốc toạ độ là tâm tiết diện.
   ///     X → phải (cạnh b), Y → lên (cạnh h).
   /// </summary>
   public readonly record struct BarPoint(double X, double Y, double DiameterMm);
}
