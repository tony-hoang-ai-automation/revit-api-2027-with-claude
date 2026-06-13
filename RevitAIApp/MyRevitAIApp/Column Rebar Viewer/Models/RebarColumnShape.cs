namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Hình dạng tiết diện cột phát hiện từ geometry probe.
   ///     v1 chỉ vẽ Rectangular; Round/Unknown bị Command từ chối.
   /// </summary>
   public enum RebarColumnShape
   {
      Rectangular,
      Round,
      Unknown
   }
}
