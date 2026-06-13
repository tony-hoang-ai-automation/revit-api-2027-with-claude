using System.IO;
using System.Xml.Serialization;
using MyRevitAIApp.AutoDimColumns.Models;
using Serilog;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     XmlSerializer-based persistence (BCL — compat R23 net48 ↔ R27 net10).
   ///     File: %LocalAppData%\MyRevitAIApp\autoDimColumns.xml.
   ///     Hỏng file / IO error → return Default + log warning, KHÔNG throw.
   /// </summary>
   public sealed class SettingsStore : ISettingsStore
   {
      private static readonly XmlSerializer Serializer = new(typeof(SettingsDto));
      private readonly ILogger _logger = Log.ForContext<SettingsStore>();
      private readonly string _filePath;

      public SettingsStore()
      {
         var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyRevitAIApp");
         _filePath = Path.Combine(dir, "autoDimColumns.xml");
      }

      public DimensioningSettings Load()
      {
         try
         {
            if (!File.Exists(_filePath)) return DimensioningSettings.Default;
            using var fs = File.OpenRead(_filePath);
            var dto = (SettingsDto?)Serializer.Deserialize(fs);
            if (dto == null) return DimensioningSettings.Default;
            return dto.ToRecord();
         }
         catch (Exception ex)
         {
            _logger.Warning(ex, "Load settings failed → dùng Default");
            return DimensioningSettings.Default;
         }
      }

      public void Save(DimensioningSettings settings)
      {
         try
         {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir!);

            using var fs = File.Create(_filePath);
            Serializer.Serialize(fs, SettingsDto.From(settings));
         }
         catch (Exception ex)
         {
            _logger.Warning(ex, "Save settings failed");
         }
      }

      /// <summary>
      ///     Mutable DTO — bắt buộc cho XmlSerializer (yêu cầu parameterless ctor + public setter).
      ///     Record DimensioningSettings là immutable → cần convert.
      /// </summary>
      public sealed class SettingsDto
      {
         public long LinearDimTypeId { get; set; }
         public bool LinearDimTypeIdHasValue { get; set; }
         public long RadialDimTypeId { get; set; }
         public bool RadialDimTypeIdHasValue { get; set; }
         public double FaceOffsetMm { get; set; }
         public ScopeMode Scope { get; set; }
         public bool DimAxisX { get; set; }
         public bool DimAxisY { get; set; }
         public bool IncludeSelfDim { get; set; }
         public bool IncludeRadialDim { get; set; }
         public bool IncludePerimeterDim { get; set; }
         public ExistingDimsPolicy ExistingDims { get; set; }
         public double RotationToleranceDegrees { get; set; }
         public double OnGridToleranceMm { get; set; }

         public DimensioningSettings ToRecord() => new(
            LinearDimTypeId: LinearDimTypeIdHasValue ? LinearDimTypeId : null,
            RadialDimTypeId: RadialDimTypeIdHasValue ? RadialDimTypeId : null,
            FaceOffsetMm: FaceOffsetMm,
            Scope: Scope,
            DimAxisX: DimAxisX,
            DimAxisY: DimAxisY,
            IncludeSelfDim: IncludeSelfDim,
            IncludeRadialDim: IncludeRadialDim,
            IncludePerimeterDim: IncludePerimeterDim,
            ExistingDims: ExistingDims,
            RotationToleranceDegrees: RotationToleranceDegrees,
            OnGridToleranceMm: OnGridToleranceMm);

         public static SettingsDto From(DimensioningSettings s) => new()
         {
            LinearDimTypeId = s.LinearDimTypeId ?? 0,
            LinearDimTypeIdHasValue = s.LinearDimTypeId.HasValue,
            RadialDimTypeId = s.RadialDimTypeId ?? 0,
            RadialDimTypeIdHasValue = s.RadialDimTypeId.HasValue,
            FaceOffsetMm = s.FaceOffsetMm,
            Scope = s.Scope,
            DimAxisX = s.DimAxisX,
            DimAxisY = s.DimAxisY,
            IncludeSelfDim = s.IncludeSelfDim,
            IncludeRadialDim = s.IncludeRadialDim,
            IncludePerimeterDim = s.IncludePerimeterDim,
            ExistingDims = s.ExistingDims,
            RotationToleranceDegrees = s.RotationToleranceDegrees,
            OnGridToleranceMm = s.OnGridToleranceMm
         };
      }
   }
}
