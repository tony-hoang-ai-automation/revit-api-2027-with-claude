namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Phân loại cột theo hình học: vuông (rectangular box), tròn (cylindrical),
   ///     hoặc Other (không hỗ trợ trong v1 — slanted, L-shape, custom families).
   /// </summary>
   public enum ColumnShape
   {
      Square,
      Round,
      Other
   }
}
