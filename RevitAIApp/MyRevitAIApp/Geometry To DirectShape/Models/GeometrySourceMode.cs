namespace MyRevitAIApp.GeometryToDirectShape.Models
{
   /// <summary>
   ///     Nguồn geometry được trích từ một phần tử Revit để dựng DirectShape.
   ///     Chỉ khác nhau khi phần tử là Family Instance (có <see cref="GeometryInstance"/>).
   /// </summary>
   public enum GeometrySourceMode
   {
      /// <summary>
      ///     Geometry đã được áp transform vị trí của instance
      ///     (<c>GeometryInstance.GetInstanceGeometry()</c>) — DirectShape nằm ĐÚNG vị trí
      ///     phần tử gốc trong model (world coordinates).
      /// </summary>
      Instance,

      /// <summary>
      ///     Geometry thô trong hệ toạ độ của family symbol
      ///     (<c>GeometryInstance.GetSymbolGeometry()</c>) — KHÔNG áp transform instance,
      ///     nên DirectShape nằm ở gốc toạ độ (0,0,0) theo định nghĩa family.
      /// </summary>
      Symbol
   }
}
