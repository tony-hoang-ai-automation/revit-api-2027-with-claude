using MyRevitAIApp.DuplicateSheet.Models;

namespace MyRevitAIApp.DuplicateSheet.Services
{
   /// <summary>
   ///     Pure-logic engine biến đổi (srcNumber, srcName) → (newNumber, newName)
   ///     theo <see cref="NamingRule"/>. Không phụ thuộc Revit API → xUnit testable.
   /// </summary>
   public interface INamingRuleEngine
   {
      /// <summary>
      ///     Apply rule cho 1 source sheet trong batch.
      /// </summary>
      /// <param name="srcNumber">Sheet number gốc (vd "A101").</param>
      /// <param name="srcName">Sheet name gốc (vd "Ground Floor").</param>
      /// <param name="indexInBatch">Vị trí 0-based của sheet trong batch (cho increment).</param>
      /// <param name="rule">Cấu hình rule.</param>
      /// <param name="takenNumbers">Tập số sheet đã dùng (document hiện có + sheet đã sinh trong batch trước đó).</param>
      /// <returns>Tuple (newNumber unique, newName, hasCollision flag).</returns>
      (string newNumber, string newName, bool hasCollision) Apply(
         string srcNumber,
         string srcName,
         int indexInBatch,
         NamingRule rule,
         IReadOnlyCollection<string> takenNumbers);
   }
}
