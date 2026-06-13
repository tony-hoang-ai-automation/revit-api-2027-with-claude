namespace MyRevitAIApp.GeometryToDirectShape.Models
{
   /// <summary>
   ///     Kết quả tạo một DirectShape — dùng để hiển thị bảng so sánh Instance vs Symbol.
   ///     Toạ độ tâm (Center*) tính từ bounding box của DirectShape vừa tạo, đơn vị mm,
   ///     để minh hoạ khác biệt vị trí giữa 2 mode.
   /// </summary>
   public sealed record DirectShapeCreationResult(
      GeometrySourceMode Mode,
      int SolidCount,
      double VolumeCubicMeters,
      double CenterXmm,
      double CenterYmm,
      double CenterZmm,
      long DirectShapeId,
      string Note)
   {
      public string ModeLabel => Mode == GeometrySourceMode.Instance ? "Instance Geometry" : "Symbol Geometry";

      public string CenterLabel => $"({CenterXmm:F0}, {CenterYmm:F0}, {CenterZmm:F0})";

      public string VolumeLabel => VolumeCubicMeters > 0 ? $"{VolumeCubicMeters:F4}" : "—";
   }
}
