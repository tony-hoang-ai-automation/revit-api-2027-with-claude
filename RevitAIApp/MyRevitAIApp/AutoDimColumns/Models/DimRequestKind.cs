namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Phân loại 1 yêu cầu dim. Quyết định service nào execute và Revit API call nào.
   /// </summary>
   public enum DimRequestKind
   {
      /// <summary>Self-dim cột vuông theo trục X (2 chấm: face trái → grid → face phải).</summary>
      SelfDimSquareX,

      /// <summary>Self-dim cột vuông theo trục Y (face dưới → grid → face trên).</summary>
      SelfDimSquareY,

      /// <summary>Cột vuông → grid X gần nhất (linear dim, 1 segment).</summary>
      ColumnToGridX,

      /// <summary>Cột vuông → grid Y gần nhất (linear dim, 1 segment).</summary>
      ColumnToGridY,

      /// <summary>Radial dim cho cột tròn (R&lt;bán-kính&gt;).</summary>
      RadialRound,

      /// <summary>Dim tổng grid-to-grid chu vi theo trục X (nhiều segment).</summary>
      PerimeterTotalX,

      /// <summary>Dim tổng grid-to-grid chu vi theo trục Y (nhiều segment).</summary>
      PerimeterTotalY
   }
}
