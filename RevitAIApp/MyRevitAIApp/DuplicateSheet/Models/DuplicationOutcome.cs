namespace MyRevitAIApp.DuplicateSheet.Models
{
   /// <summary>
   ///     Kết quả duplicate 1 source sheet, trả về từ <c>ISheetDuplicator.DuplicateOne</c>.
   ///     <c>Success=false</c> → call-site phải rollback transaction.
   /// </summary>
   public sealed record DuplicationOutcome(
      ElementId SourceSheetId,
      ElementId? NewSheetId,
      string NewNumber,
      string NewName,
      int ViewportsCopied,
      int LegendsPlaced,
      int SchedulesPlaced,
      int AnnotationsCopied,
      bool Success,
      string? ErrorMessage)
   {
      public static DuplicationOutcome Failed(ElementId sourceId, string newNumber, string newName, string error) =>
         new(sourceId, null, newNumber, newName, 0, 0, 0, 0, false, error);
   }
}
