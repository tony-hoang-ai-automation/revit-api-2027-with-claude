using MyRevitAIApp.AutoDimColumns.Models;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Default <see cref="IDimensionPlanner"/>. Pure logic, no Revit API.
   ///     Algorithm: xem plan §Sơ đồ thuật toán.
   /// </summary>
   public sealed class DimensionPlanner : IDimensionPlanner
   {
      private const double MmPerFoot = 304.8;
      private const double DegToRad = Math.PI / 180.0;

      public IReadOnlyList<DimSpec> Plan(
         IReadOnlyList<ColumnSnapshot> columns,
         IReadOnlyList<GridSnapshot> grids,
         DimensioningSettings settings)
      {
         var specs = new List<DimSpec>();
         var rotToleranceRad = settings.RotationToleranceDegrees * DegToRad;
         var onGridToleranceFeet = settings.OnGridToleranceMm / MmPerFoot;

         var verticalGrids = grids.Where(g => g.Axis == GridAxis.Vertical).OrderBy(g => g.Position).ToList();
         var horizontalGrids = grids.Where(g => g.Axis == GridAxis.Horizontal).OrderBy(g => g.Position).ToList();

         foreach (var col in columns)
         {
            if (col.Shape == ColumnShape.Other)
            {
               specs.Add(SkipSpec(col, "Hình học không hỗ trợ (không phải vuông/tròn)"));
               continue;
            }

            if (col.Shape == ColumnShape.Square && IsRotated(col.Rotation, rotToleranceRad))
            {
               specs.Add(SkipSpec(col, $"Cột xoay {RadToDeg(col.Rotation):F1}° — bỏ qua (v1 chỉ orthogonal)"));
               continue;
            }

            if (col.Shape == ColumnShape.Square && settings.IncludeSelfDim)
            {
               if (settings.DimAxisX) specs.Add(SquareSelfSpec(col, DimRequestKind.SelfDimSquareX, settings));
               if (settings.DimAxisY) specs.Add(SquareSelfSpec(col, DimRequestKind.SelfDimSquareY, settings));
            }

            if (col.Shape == ColumnShape.Round && settings.IncludeRadialDim)
            {
               specs.Add(RadialSpec(col, settings));
            }

            if (settings.DimAxisX)
            {
               var nearestX = FindNearest(verticalGrids, col.X);
               if (nearestX != null && Math.Abs(col.X - nearestX.Position) > onGridToleranceFeet)
               {
                  specs.Add(GridSpec(col, DimRequestKind.ColumnToGridX, nearestX, settings));
               }
            }

            if (settings.DimAxisY)
            {
               var nearestY = FindNearest(horizontalGrids, col.Y);
               if (nearestY != null && Math.Abs(col.Y - nearestY.Position) > onGridToleranceFeet)
               {
                  specs.Add(GridSpec(col, DimRequestKind.ColumnToGridY, nearestY, settings));
               }
            }
         }

         if (settings.IncludePerimeterDim)
         {
            if (settings.DimAxisX && verticalGrids.Count >= 2)
            {
               specs.Add(PerimeterSpec(DimRequestKind.PerimeterTotalX, verticalGrids, settings));
            }
            if (settings.DimAxisY && horizontalGrids.Count >= 2)
            {
               specs.Add(PerimeterSpec(DimRequestKind.PerimeterTotalY, horizontalGrids, settings));
            }
         }

         return specs;
      }

      private static bool IsRotated(double rotationRad, double toleranceRad)
      {
         return Math.Abs(rotationRad) > toleranceRad;
      }

      private static double RadToDeg(double rad) => rad * 180.0 / Math.PI;

      private static GridSnapshot? FindNearest(IReadOnlyList<GridSnapshot> sorted, double coord)
      {
         if (sorted.Count == 0) return null;
         GridSnapshot? best = null;
         var bestDist = double.MaxValue;
         foreach (var g in sorted)
         {
            var d = Math.Abs(g.Position - coord);
            if (d < bestDist)
            {
               bestDist = d;
               best = g;
            }
         }
         return best;
      }

      private static DimSpec SkipSpec(ColumnSnapshot col, string reason) => new(
         DimRequestKind.SelfDimSquareX,
         col.Id,
         FormatColumnLabel(col),
         Array.Empty<long>(),
         "—",
         0.0,
         reason);

      private static DimSpec SquareSelfSpec(ColumnSnapshot col, DimRequestKind kind, DimensioningSettings settings) => new(
         kind,
         col.Id,
         FormatColumnLabel(col),
         Array.Empty<long>(),
         kind == DimRequestKind.SelfDimSquareX ? "self-X" : "self-Y",
         settings.FaceOffsetMm / MmPerFoot,
         null);

      private static DimSpec RadialSpec(ColumnSnapshot col, DimensioningSettings settings) => new(
         DimRequestKind.RadialRound,
         col.Id,
         FormatColumnLabel(col),
         Array.Empty<long>(),
         $"R{col.Radius * MmPerFoot:F0}",
         settings.FaceOffsetMm / MmPerFoot,
         null);

      private static DimSpec GridSpec(ColumnSnapshot col, DimRequestKind kind, GridSnapshot grid, DimensioningSettings settings) => new(
         kind,
         col.Id,
         FormatColumnLabel(col),
         new[] { grid.Id },
         $"Grid {grid.Name}",
         settings.FaceOffsetMm / MmPerFoot,
         null);

      private static DimSpec PerimeterSpec(DimRequestKind kind, IReadOnlyList<GridSnapshot> orderedGrids, DimensioningSettings settings) => new(
         kind,
         null,
         "—",
         orderedGrids.Select(g => g.Id).ToList(),
         $"{orderedGrids[0].Name} → {orderedGrids[orderedGrids.Count - 1].Name} ({orderedGrids.Count} grids)",
         settings.FaceOffsetMm / MmPerFoot * 2.0,
         null);

      private static string FormatColumnLabel(ColumnSnapshot col) =>
         $"{col.FamilySymbolName} [{col.Id}]";
   }
}
