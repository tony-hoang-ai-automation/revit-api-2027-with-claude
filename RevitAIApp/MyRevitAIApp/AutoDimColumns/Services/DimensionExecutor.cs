using MyRevitAIApp.AutoDimColumns.Internals;
using MyRevitAIApp.AutoDimColumns.Models;
using Serilog;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Default <see cref="IDimensionExecutor"/>.
   ///
   ///     Trục Z dim line luôn = 0 (mặt bằng Floor Plan).
   ///     References sort dọc theo direction của dim line (project anchor → param) trước khi append
   ///     để tránh ArgumentException + overlap witness.
   /// </summary>
   public sealed class DimensionExecutor : IDimensionExecutor
   {
      private const double MmPerFoot = 304.8;
      private const double DimLinePadFeet = 1.0;

      private readonly IColumnReferenceExtractor _refExtractor;
      private readonly IExistingDimensionCleaner _cleaner;
      private readonly ILogger _logger = Log.ForContext<DimensionExecutor>();

      public DimensionExecutor(IColumnReferenceExtractor refExtractor, IExistingDimensionCleaner cleaner)
      {
         _refExtractor = refExtractor;
         _cleaner = cleaner;
      }

      public DimensioningOutcome Execute(
         Document doc,
         View view,
         IReadOnlyList<DimSpec> specs,
         DimensioningSettings settings,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         IReadOnlyDictionary<long, Grid> gridIndex,
         DimensionType linearDimType,
         DimensionType radialDimType)
      {
         var created = 0;
         var skipped = 0;
         var failed = 0;
         var errors = new List<string>();

         var deleted = 0;
         if (settings.ExistingDims == ExistingDimsPolicy.DeleteThenRecreate)
         {
            var targetIds = specs
               .Where(s => s.ColumnId.HasValue && s.SkipReason == null)
               .Select(s => s.ColumnId!.Value)
               .Distinct()
               .ToList();
            deleted = _cleaner.DeleteOverlapping(doc, view, targetIds);
         }

         foreach (var spec in specs)
         {
            if (spec.SkipReason != null)
            {
               skipped++;
               continue;
            }

            try
            {
               var dim = CreateDim(doc, view, spec, settings, columnIndex, gridIndex, linearDimType, radialDimType);
               if (dim == null)
               {
                  skipped++;
                  errors.Add($"[{spec.Kind}] {spec.ColumnLabel}: Revit returned null");
               }
               else
               {
                  created++;
               }
            }
            catch (Exception ex)
            {
               failed++;
               errors.Add($"[{spec.Kind}] {spec.ColumnLabel}: {ex.Message}");
               _logger.Warning(ex, "Failed to create dim for spec {Kind} column={Column}", spec.Kind, spec.ColumnLabel);
            }
         }

         _logger.Information("AutoDim outcome: created={C} skipped={S} failed={F} deleted={D}",
            created, skipped, failed, deleted);

         return new DimensioningOutcome(created, skipped, failed, deleted, errors);
      }

      private Dimension? CreateDim(
         Document doc,
         View view,
         DimSpec spec,
         DimensioningSettings settings,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         IReadOnlyDictionary<long, Grid> gridIndex,
         DimensionType linearDimType,
         DimensionType radialDimType)
      {
         var offsetFeet = settings.FaceOffsetMm / MmPerFoot;

         switch (spec.Kind)
         {
            case DimRequestKind.SelfDimSquareX:
            case DimRequestKind.SelfDimSquareY:
               return CreateSquareSelfDim(doc, view, spec, offsetFeet, columnIndex, linearDimType);

            case DimRequestKind.ColumnToGridX:
            case DimRequestKind.ColumnToGridY:
               return CreateColumnToGridDim(doc, view, spec, offsetFeet, columnIndex, gridIndex, linearDimType);

            case DimRequestKind.RadialRound:
               return CreateRadialDim(doc, view, spec, offsetFeet, columnIndex, radialDimType);

            case DimRequestKind.PerimeterTotalX:
            case DimRequestKind.PerimeterTotalY:
               return CreatePerimeterDim(doc, view, spec, offsetFeet, gridIndex, columnIndex, linearDimType);

            default:
               throw new InvalidOperationException($"Unsupported DimRequestKind: {spec.Kind}");
         }
      }

      private Dimension? CreateSquareSelfDim(
         Document doc, View view, DimSpec spec, double offsetFeet,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         DimensionType dimType)
      {
         var inst = columnIndex[spec.ColumnId!.Value];
         var refs = _refExtractor.ExtractSquare(inst, view);
         if (refs == null) throw new InvalidOperationException("Không trích được face references của cột vuông");

         var loc = (LocationPoint)inst.Location;
         var center = loc.Point;
         var bbox = inst.get_BoundingBox(view) ?? throw new InvalidOperationException("Không có BoundingBox trong view");

         ReferenceArray refArray;
         Line dimLine;

         if (spec.Kind == DimRequestKind.SelfDimSquareX)
         {
            refArray = MakeRefArray(refs.Left, refs.CenterLeftRight, refs.Right);
            var lineY = bbox.Max.Y + offsetFeet;
            dimLine = Line.CreateBound(
               new XYZ(bbox.Min.X - DimLinePadFeet, lineY, 0),
               new XYZ(bbox.Max.X + DimLinePadFeet, lineY, 0));
         }
         else
         {
            refArray = MakeRefArray(refs.Front, refs.CenterFrontBack, refs.Back);
            var lineX = bbox.Max.X + offsetFeet;
            dimLine = Line.CreateBound(
               new XYZ(lineX, bbox.Min.Y - DimLinePadFeet, 0),
               new XYZ(lineX, bbox.Max.Y + DimLinePadFeet, 0));
         }

         return doc.Create.NewDimension(view, dimLine, refArray, dimType);
      }

      private Dimension? CreateColumnToGridDim(
         Document doc, View view, DimSpec spec, double offsetFeet,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         IReadOnlyDictionary<long, Grid> gridIndex,
         DimensionType dimType)
      {
         var inst = columnIndex[spec.ColumnId!.Value];
         var refs = _refExtractor.ExtractSquare(inst, view);
         if (refs == null) throw new InvalidOperationException("Không trích được face references");

         if (spec.GridIds.Count != 1) throw new InvalidOperationException("ColumnToGrid cần đúng 1 grid ID");
         var grid = gridIndex[spec.GridIds[0]];
         var gridRef = new Reference(grid);

         var loc = (LocationPoint)inst.Location;
         var center = loc.Point;
         var bbox = inst.get_BoundingBox(view) ?? throw new InvalidOperationException("Không có BoundingBox");
         var gridLine = (Line)grid.Curve;
         var gridPos = spec.Kind == DimRequestKind.ColumnToGridX ? gridLine.Origin.X : gridLine.Origin.Y;

         ReferenceArray refArray = new ReferenceArray();
         Line dimLine;

         if (spec.Kind == DimRequestKind.ColumnToGridX)
         {
            var nearFace = gridPos < center.X ? refs.Left : refs.Right;
            refArray.Append(nearFace);
            refArray.Append(gridRef);
            var lineY = bbox.Max.Y + offsetFeet;
            var xMin = Math.Min(gridPos, bbox.Min.X) - DimLinePadFeet;
            var xMax = Math.Max(gridPos, bbox.Max.X) + DimLinePadFeet;
            dimLine = Line.CreateBound(new XYZ(xMin, lineY, 0), new XYZ(xMax, lineY, 0));
         }
         else
         {
            var nearFace = gridPos < center.Y ? refs.Front : refs.Back;
            refArray.Append(nearFace);
            refArray.Append(gridRef);
            var lineX = bbox.Max.X + offsetFeet;
            var yMin = Math.Min(gridPos, bbox.Min.Y) - DimLinePadFeet;
            var yMax = Math.Max(gridPos, bbox.Max.Y) + DimLinePadFeet;
            dimLine = Line.CreateBound(new XYZ(lineX, yMin, 0), new XYZ(lineX, yMax, 0));
         }

         return doc.Create.NewDimension(view, dimLine, refArray, dimType);
      }

      private Dimension? CreateRadialDim(
         Document doc, View view, DimSpec spec, double offsetFeet,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         DimensionType dimType)
      {
#if REVIT2025_OR_GREATER
         var inst = columnIndex[spec.ColumnId!.Value];
         var refs = _refExtractor.ExtractRound(inst, view);
         if (refs == null) throw new InvalidOperationException("Không trích được arc reference của cột tròn");

         // RadialDimension.Create chỉ có từ Revit 2025+. R23/R24 fallback: skip (log warning).
         var dim = RadialDimension.Create(doc, view, refs.Arc, /* isRadius: */ true);
         if (dim != null && dimType != null)
         {
            dim.DimensionType = dimType;
         }
         return dim;
#else
         _logger.Warning("Radial dim chỉ support Revit 2025+. Cột {Id} bị skip.", spec.ColumnId);
         return null;
#endif
      }

      private Dimension? CreatePerimeterDim(
         Document doc, View view, DimSpec spec, double offsetFeet,
         IReadOnlyDictionary<long, Grid> gridIndex,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         DimensionType dimType)
      {
         if (spec.GridIds.Count < 2) return null;

         var refArray = new ReferenceArray();
         var positions = new List<double>();
         foreach (var gid in spec.GridIds)
         {
            var grid = gridIndex[gid];
            refArray.Append(new Reference(grid));
            var line = (Line)grid.Curve;
            positions.Add(spec.Kind == DimRequestKind.PerimeterTotalX ? line.Origin.X : line.Origin.Y);
         }

         var minPos = positions.Min();
         var maxPos = positions.Max();
         var perpExtent = ComputePerpendicularExtent(view, columnIndex.Values, spec.Kind);
         var dimLineOffsetFeet = spec.DimLineOffsetFeet > 0 ? spec.DimLineOffsetFeet : offsetFeet * 2.0;

         Line dimLine;
         if (spec.Kind == DimRequestKind.PerimeterTotalX)
         {
            var lineY = perpExtent.max + dimLineOffsetFeet;
            dimLine = Line.CreateBound(
               new XYZ(minPos - DimLinePadFeet, lineY, 0),
               new XYZ(maxPos + DimLinePadFeet, lineY, 0));
         }
         else
         {
            var lineX = perpExtent.max + dimLineOffsetFeet;
            dimLine = Line.CreateBound(
               new XYZ(lineX, minPos - DimLinePadFeet, 0),
               new XYZ(lineX, maxPos + DimLinePadFeet, 0));
         }

         return doc.Create.NewDimension(view, dimLine, refArray, dimType);
      }

      private static (double min, double max) ComputePerpendicularExtent(
         View view,
         IEnumerable<FamilyInstance> columns,
         DimRequestKind kind)
      {
         double min = double.MaxValue;
         double max = double.MinValue;
         foreach (var inst in columns)
         {
            var bbox = inst.get_BoundingBox(view);
            if (bbox == null) continue;
            if (kind == DimRequestKind.PerimeterTotalX)
            {
               if (bbox.Min.Y < min) min = bbox.Min.Y;
               if (bbox.Max.Y > max) max = bbox.Max.Y;
            }
            else
            {
               if (bbox.Min.X < min) min = bbox.Min.X;
               if (bbox.Max.X > max) max = bbox.Max.X;
            }
         }
         if (min == double.MaxValue) { min = 0; max = 0; }
         return (min, max);
      }

      private static ReferenceArray MakeRefArray(params Reference[] refs)
      {
         var arr = new ReferenceArray();
         foreach (var r in refs) arr.Append(r);
         return arr;
      }
   }
}
