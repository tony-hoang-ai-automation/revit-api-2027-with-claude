namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Cấu hình của 1 lần chạy AutoDim. Persist qua JSON (ISettingsStore).
   ///     Đơn vị FaceOffset theo mm (UI nhập mm, executor convert sang feet).
   ///     RotationToleranceDegrees / OnGridToleranceMm là epsilon — không expose UI v1.
   /// </summary>
   public sealed record DimensioningSettings(
      long? LinearDimTypeId,
      long? RadialDimTypeId,
      double FaceOffsetMm,
      ScopeMode Scope,
      bool DimAxisX,
      bool DimAxisY,
      bool IncludeSelfDim,
      bool IncludeRadialDim,
      bool IncludePerimeterDim,
      ExistingDimsPolicy ExistingDims,
      double RotationToleranceDegrees,
      double OnGridToleranceMm)
   {
      public static DimensioningSettings Default { get; } = new(
         LinearDimTypeId: null,
         RadialDimTypeId: null,
         FaceOffsetMm: 500.0,
         Scope: ScopeMode.ActiveView,
         DimAxisX: true,
         DimAxisY: true,
         IncludeSelfDim: true,
         IncludeRadialDim: true,
         IncludePerimeterDim: true,
         ExistingDims: ExistingDimsPolicy.DeleteThenRecreate,
         RotationToleranceDegrees: 1.0,
         OnGridToleranceMm: 5.0);
   }
}
