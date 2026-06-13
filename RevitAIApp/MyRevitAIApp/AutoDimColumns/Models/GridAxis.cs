namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Trục của Grid trong mặt bằng. Horizontal = grid chạy ngang (số 1,2,3...),
   ///     Vertical = grid chạy dọc (chữ A,B,C...). Other = radial / non-straight grid (skip v1).
   /// </summary>
   public enum GridAxis
   {
      Horizontal,
      Vertical,
      Other
   }
}
