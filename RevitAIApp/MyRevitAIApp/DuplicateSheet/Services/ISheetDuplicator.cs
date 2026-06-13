using MyRevitAIApp.DuplicateSheet.Models;

namespace MyRevitAIApp.DuplicateSheet.Services
{
   /// <summary>
   ///     Duplicate 1 source <see cref="ViewSheet"/> sang sheet mới với nội dung config.
   ///     Caller phải wrap call trong 1 <see cref="Transaction"/> (atomic batch).
   /// </summary>
   public interface ISheetDuplicator
   {
      DuplicationOutcome DuplicateOne(
         Document doc,
         ViewSheet source,
         string newNumber,
         string newName,
         DuplicateMode viewMode,
         SheetContentOptions content);
   }
}
