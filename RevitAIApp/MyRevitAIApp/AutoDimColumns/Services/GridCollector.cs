using MyRevitAIApp.AutoDimColumns.Internals;
using MyRevitAIApp.AutoDimColumns.Models;
using Serilog;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Default <see cref="IGridCollector"/>. Trục axis xác định bằng dot product với X/Y unit vectors:
   ///     |dir.X| &gt; |dir.Y| → grid chạy ngang (Horizontal) → Position = origin.Y.
   ///     Reverse → Vertical → Position = origin.X.
   /// </summary>
   public sealed class GridCollector : IGridCollector
   {
      private const double AxisAlignedTolerance = 0.01;

      private readonly ILogger _logger = Log.ForContext<GridCollector>();

      public IGridCollector.CollectResult Collect(Document doc, View view)
      {
         var grids = new FilteredElementCollector(doc, view.Id)
            .OfClass(typeof(Grid))
            .Cast<Grid>()
            .ToList();

         var snapshots = new List<GridSnapshot>(grids.Count);
         var lookup = new Dictionary<long, Grid>(grids.Count);

         foreach (var g in grids)
         {
            var snap = TryBuildSnapshot(g);
            if (snap == null) continue;
            snapshots.Add(snap);
            lookup[snap.Id] = g;
         }

         _logger.Information("Collected {Count} grids from view {ViewName}", snapshots.Count, view.Name);
         return new IGridCollector.CollectResult(snapshots, lookup);
      }

      private static GridSnapshot? TryBuildSnapshot(Grid g)
      {
         if (g.Curve is not Line line) return null;

         var dir = line.Direction;
         var absX = Math.Abs(dir.X);
         var absY = Math.Abs(dir.Y);

         GridAxis axis;
         double position;
         if (absX > absY && absY < AxisAlignedTolerance)
         {
            axis = GridAxis.Horizontal;
            position = line.Origin.Y;
         }
         else if (absY > absX && absX < AxisAlignedTolerance)
         {
            axis = GridAxis.Vertical;
            position = line.Origin.X;
         }
         else
         {
            axis = GridAxis.Other;
            position = 0;
         }

         return new GridSnapshot(g.Id.GetIdValue(), g.Name, axis, position);
      }
   }
}
