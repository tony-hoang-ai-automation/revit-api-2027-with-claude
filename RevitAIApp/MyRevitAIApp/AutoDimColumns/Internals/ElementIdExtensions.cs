namespace MyRevitAIApp.AutoDimColumns.Internals
{
   /// <summary>
   ///     Bridge long ↔ ElementId qua các phiên bản Revit:
   ///     - R23 (Revit 2023): ElementId(int), IntegerValue
   ///     - R24+ (Revit 2024+): ElementId(long), Value
   ///     Toàn bộ feature AutoDimColumns dùng long làm key → tránh #if rải rác.
   /// </summary>
   internal static class ElementIdExtensions
   {
      public static ElementId ToElementId(this long id)
      {
#if REVIT2024_OR_GREATER
         return new ElementId(id);
#else
         return new ElementId((int)id);
#endif
      }

      public static long GetIdValue(this ElementId id)
      {
#if REVIT2024_OR_GREATER
         return id.Value;
#else
         return id.IntegerValue;
#endif
      }
   }
}
