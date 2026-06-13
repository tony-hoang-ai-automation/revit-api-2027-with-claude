using MyRevitAIApp.AutoDimColumns.Internals;
using MyRevitAIApp.AutoDimColumns.Models;
using Serilog;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Collect columns từ active view qua FilteredElementCollector (OST_StructuralColumns + OST_Columns).
   ///     Phân loại shape qua geometry probe: có CylindricalFace → Round, else Square,
   ///     else (compound) → Other (sẽ bị planner skip).
   /// </summary>
   public sealed class ColumnCollector : IColumnCollector
   {
      private readonly ILogger _logger = Log.ForContext<ColumnCollector>();

      public IColumnCollector.CollectResult Collect(Document doc, View view, ScopeMode scope, ICollection<ElementId>? selectedIds)
      {
         var collector = new FilteredElementCollector(doc, view.Id)
            .WherePasses(new ElementMulticategoryFilter(new[]
            {
               BuiltInCategory.OST_StructuralColumns,
               BuiltInCategory.OST_Columns
            }))
            .WhereElementIsNotElementType();

         var instances = collector.Cast<FamilyInstance>().ToList();

         if (scope == ScopeMode.SelectedOnly && selectedIds != null && selectedIds.Count > 0)
         {
            var selectedSet = new HashSet<long>(selectedIds.Select(id => id.GetIdValue()));
            instances = instances.Where(i => selectedSet.Contains(i.Id.GetIdValue())).ToList();
         }

         var snapshots = new List<ColumnSnapshot>(instances.Count);
         var lookup = new Dictionary<long, FamilyInstance>(instances.Count);

         foreach (var inst in instances)
         {
            var snapshot = TryBuildSnapshot(inst, view);
            if (snapshot == null) continue;
            snapshots.Add(snapshot);
            lookup[snapshot.Id] = inst;
         }

         _logger.Information("Collected {Count} columns from view {ViewName} (scope={Scope})",
            snapshots.Count, view.Name, scope);

         return new IColumnCollector.CollectResult(snapshots, lookup);
      }

      private ColumnSnapshot? TryBuildSnapshot(FamilyInstance inst, View view)
      {
         if (inst.Location is not LocationPoint loc) return null;

         var shape = DetectShape(inst, view);
         var bbox = inst.get_BoundingBox(view);
         var width = 0.0;
         var height = 0.0;
         if (bbox != null)
         {
            width = bbox.Max.X - bbox.Min.X;
            height = bbox.Max.Y - bbox.Min.Y;
         }

         var radius = shape == ColumnShape.Round
            ? Math.Max(width, height) / 2.0
            : 0.0;

         return new ColumnSnapshot(
            Id: inst.Id.GetIdValue(),
            FamilySymbolName: inst.Symbol.Name,
            Shape: shape,
            X: loc.Point.X,
            Y: loc.Point.Y,
            Width: width,
            Height: height,
            Radius: radius,
            Rotation: loc.Rotation);
      }

      private static ColumnShape DetectShape(FamilyInstance inst, View view)
      {
         var opts = new Options { ComputeReferences = false, View = view };
         var geom = inst.get_Geometry(opts);
         if (geom == null) return ColumnShape.Other;

         var hasCylindrical = false;
         var planarCount = 0;
         foreach (var go in geom)
         {
            CollectFaces(go, ref hasCylindrical, ref planarCount);
         }

         if (hasCylindrical) return ColumnShape.Round;
         if (planarCount >= 4) return ColumnShape.Square;
         return ColumnShape.Other;
      }

      private static void CollectFaces(GeometryObject go, ref bool hasCylindrical, ref int planarCount)
      {
         switch (go)
         {
            case Solid solid:
               foreach (Face face in solid.Faces)
               {
                  if (face is CylindricalFace) hasCylindrical = true;
                  else if (face is PlanarFace) planarCount++;
               }
               break;
            case GeometryInstance gi:
               foreach (var inner in gi.GetInstanceGeometry())
               {
                  CollectFaces(inner, ref hasCylindrical, ref planarCount);
               }
               break;
         }
      }
   }
}
