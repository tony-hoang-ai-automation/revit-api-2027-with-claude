namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Danh mục đường kính thép phổ biến (mm) theo TCVN và phân loại vật liệu.
   /// </summary>
   public static class BarCatalog
   {
      /// <summary>Đường kính thép dọc chủ (mm).</summary>
      public static readonly int[] LongitudinalDiameters = { 12, 14, 16, 18, 20, 22, 25, 28, 32 };

      /// <summary>Đường kính thép đai (mm).</summary>
      public static readonly int[] StirrupDiameters = { 6, 8, 10, 12 };
   }

   /// <summary>Cấp độ bền bê tông theo TCVN 5574.</summary>
   public enum ConcreteGrade
   {
      B15,
      B20,
      B25,
      B30,
      B35,
      B40
   }

   /// <summary>Nhóm cốt thép theo TCVN 5574.</summary>
   public enum SteelGroup
   {
      CB240,
      CB300,
      CB400,
      CB500
   }
}
