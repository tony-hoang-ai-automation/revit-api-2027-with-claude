using System.Collections.Generic;

namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Dữ liệu thuần (không Revit) mô tả bộ thép cần tạo vào model.
   ///     ViewModel dựng từ trạng thái hiện tại → truyền cho RebarCreationService map sang Revit API.
   ///     Toạ độ Bars/CrossTies tính theo mm, gốc tâm tiết diện (X→b, Y→h).
   /// </summary>
   public sealed record RebarCreationRequest(
      double BMm,
      double HMm,
      double HeightMm,
      double CoverMm,
      int LongBarDiameterMm,
      int StirrupDiameterMm,
      IReadOnlyList<BarPoint> Bars,
      IReadOnlyList<CrossTie> CrossTies,
      IReadOnlyList<StirrupZone> Zones,
      double LneoMm);
}
