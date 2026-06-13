namespace MyRevitAIApp.GeometryToDirectShape.Internals
{
   /// <summary>
   ///     Bridge BuiltInCategory ↔ ElementId qua các phiên bản Revit.
   ///     Constructor <c>ElementId(BuiltInCategory)</c> và property <c>ElementId.Value</c>
   ///     chỉ có từ Revit 2024 — gom #if vào 1 chỗ tránh rải rác trong service.
   /// </summary>
   internal static class CategoryIdHelper
   {
      /// <summary>ElementId của category Generic Models — category mặc định cho DirectShape demo.</summary>
      public static ElementId GenericModel()
      {
#if REVIT2024_OR_GREATER
         return new ElementId(BuiltInCategory.OST_GenericModel);
#else
         return new ElementId((int)BuiltInCategory.OST_GenericModel);
#endif
      }

      /// <summary>Đọc giá trị số của ElementId (long ở R24+, int ở R23).</summary>
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
