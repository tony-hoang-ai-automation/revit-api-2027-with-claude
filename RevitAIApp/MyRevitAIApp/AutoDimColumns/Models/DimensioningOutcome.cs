namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Kết quả của 1 lần chạy AutoDim. Trả về từ <see cref="Services.IDimensionExecutor"/>.
   ///     Created + Skipped + Failed = total DimSpec processed.
   /// </summary>
   public sealed record DimensioningOutcome(
      int Created,
      int Skipped,
      int Failed,
      int ExistingDimsDeleted,
      IReadOnlyList<string> Errors)
   {
      public static DimensioningOutcome Empty { get; } =
         new(0, 0, 0, 0, Array.Empty<string>());
   }
}
