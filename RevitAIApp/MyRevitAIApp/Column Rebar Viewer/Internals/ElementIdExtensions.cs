namespace MyRevitAIApp.ColumnRebarViewer.Internals
{
   /// <summary>
   ///     Bridge long ↔ ElementId qua các phiên bản Revit:
   ///     - R23 (Revit 2023): ElementId(int), IntegerValue
   ///     - R24+ (Revit 2024+): ElementId(long), Value
   /// </summary>
   internal static class ElementIdExtensions
   {
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
