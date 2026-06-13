namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Row trong DataGrid preview của AutoDimColumnsView. Render từ <see cref="DimSpec"/>.
   ///     IsSkipped = true → row hiển thị màu xám / icon warning.
   /// </summary>
   public sealed record DimPreviewItem(
      int Index,
      string Column,
      string Kind,
      string Target,
      string DimStyle,
      bool IsSkipped,
      string Note);
}
