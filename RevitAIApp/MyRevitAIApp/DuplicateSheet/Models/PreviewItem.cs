namespace MyRevitAIApp.DuplicateSheet.Models
{
   /// <summary>
   ///     Row trong DataGrid preview (bottom panel). Tính từ source + NamingRule
   ///     mỗi lần user nhấn Refresh Preview. <c>HasCollision=true</c> → render warning.
   /// </summary>
   public sealed record PreviewItem(
      string SourceNumber,
      string SourceName,
      string NewNumber,
      string NewName,
      bool HasCollision,
      bool IsBlockingError,
      string? StatusMessage);
}
