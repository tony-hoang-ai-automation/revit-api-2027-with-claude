using MyRevitAIApp.ColumnRebarViewer.Models;
using Serilog;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   /// <summary>
   ///     Đọc kích thước b, h, chiều cao cột từ FamilyInstance.
   ///     Ưu tiên family/type params → fallback BoundingBox.
   ///     Đơn vị Revit internal (feet) × 304.8 → mm.
   /// </summary>
   public sealed class ColumnGeometryReader : IColumnGeometryReader
   {
      private const double MmPerFoot = 304.8;
      private readonly ILogger _logger = Log.ForContext<ColumnGeometryReader>();

      public ColumnGeometry Read(FamilyInstance column, View activeView)
      {
         var shape = DetectShape(column, activeView);

         if (shape == RebarColumnShape.Round)
         {
            var dia = ReadParamMm(column, "Diameter", "d", "b") ?? BBoxWidthMm(column, activeView);
            var h = ReadColumnHeightMm(column) ?? BBoxHeightZMm(column);
            _logger.Debug("Column {Id}: Round dia={Dia:F0}mm h={H:F0}mm", column.Id, dia, h);
            return new ColumnGeometry(RebarColumnShape.Round, 0, 0, h, dia);
         }

         if (shape == RebarColumnShape.Unknown)
         {
            _logger.Warning("Column {Id}: Unknown shape", column.Id);
            return new ColumnGeometry(RebarColumnShape.Unknown, 0, 0, 0, 0);
         }

         var bMm = ReadParamMm(column, "b", "Width", "Ширина") ?? BBoxWidthMm(column, activeView);
         var hMm = ReadParamMm(column, "h", "Depth", "d", "Height") ?? BBoxDepthMm(column, activeView);
         var heightMm = ReadColumnHeightMm(column) ?? BBoxHeightZMm(column);

         _logger.Debug("Column {Id}: Rect b={B:F0} h={H:F0} height={Ht:F0}mm", column.Id, bMm, hMm, heightMm);
         return new ColumnGeometry(RebarColumnShape.Rectangular, bMm, hMm, heightMm, 0);
      }

      private static double? ReadParamMm(FamilyInstance fi, params string[] names)
      {
         foreach (var name in names)
         {
            var p = fi.LookupParameter(name) ?? fi.Symbol.LookupParameter(name);
            if (p is { StorageType: StorageType.Double } && p.HasValue)
               return p.AsDouble() * MmPerFoot;
         }
         return null;
      }

      private static double BBoxWidthMm(FamilyInstance fi, View view)
      {
         var bb = fi.get_BoundingBox(view);
         return bb != null ? (bb.Max.X - bb.Min.X) * MmPerFoot : 300;
      }

      private static double BBoxDepthMm(FamilyInstance fi, View view)
      {
         var bb = fi.get_BoundingBox(view);
         return bb != null ? (bb.Max.Y - bb.Min.Y) * MmPerFoot : 300;
      }

      private static double BBoxHeightZMm(FamilyInstance fi)
      {
         var bb = fi.get_BoundingBox(null);
         return bb != null ? (bb.Max.Z - bb.Min.Z) * MmPerFoot : 3000;
      }

      private static double? ReadColumnHeightMm(FamilyInstance fi)
      {
         // Ưu tiên level offsets (cột thẳng đứng)
         var baseLevel = fi.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM)?.AsElementId();
         var topLevel  = fi.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM)?.AsElementId();
         if (baseLevel != null && topLevel != null &&
             fi.Document.GetElement(baseLevel) is Level bl &&
             fi.Document.GetElement(topLevel)  is Level tl)
         {
            var baseOff = fi.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM)?.AsDouble() ?? 0;
            var topOff  = fi.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM)?.AsDouble()  ?? 0;
            var h = ((tl.Elevation + topOff) - (bl.Elevation + baseOff)) * MmPerFoot;
            if (h > 0) return h;
         }

         // Fallback: LocationCurve (cột nghiêng)
         if (fi.Location is LocationCurve lc)
            return lc.Curve.Length * MmPerFoot;

         return null;
      }

      private static RebarColumnShape DetectShape(FamilyInstance fi, View view)
      {
         var opts = new Options { ComputeReferences = false, View = view };
         var geom = fi.get_Geometry(opts);
         if (geom == null) return RebarColumnShape.Unknown;

         var hasCylindrical = false;
         var planarCount = 0;
         foreach (var go in geom)
            CollectFaces(go, ref hasCylindrical, ref planarCount);

         if (hasCylindrical) return RebarColumnShape.Round;
         if (planarCount >= 4) return RebarColumnShape.Rectangular;
         return RebarColumnShape.Unknown;
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
                  CollectFaces(inner, ref hasCylindrical, ref planarCount);
               break;
         }
      }
   }
}
