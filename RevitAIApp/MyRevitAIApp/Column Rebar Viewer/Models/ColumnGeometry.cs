namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Kích thước hình học cột đọc từ Revit (đơn vị mm).
   ///     BMm/HMm cho cột chữ nhật; DiameterMm cho cột tròn.
   /// </summary>
   public sealed record ColumnGeometry(
      RebarColumnShape Shape,
      double BMm,
      double HMm,
      double HeightMm,
      double DiameterMm);
}
