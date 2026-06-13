namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Chế độ chọn tập cột để dim.
   /// </summary>
   public enum ScopeMode
   {
      /// <summary>Tất cả cột structural trong active view.</summary>
      ActiveView,

      /// <summary>Chỉ các cột user đã select (UIDocument.Selection).</summary>
      SelectedOnly
   }
}
