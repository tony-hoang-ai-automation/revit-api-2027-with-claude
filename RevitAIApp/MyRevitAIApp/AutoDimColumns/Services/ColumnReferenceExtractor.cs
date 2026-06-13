using Serilog;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Primary path: <c>FamilyInstance.GetReferences(FamilyInstanceReferenceType.X)</c>.
   ///     Fallback: geometry traversal với <c>Options.ComputeReferences=true</c> rồi phân loại face
   ///     bằng normal vector (.X dominant → Left/Right; .Y dominant → Front/Back).
   /// </summary>
   public sealed class ColumnReferenceExtractor : IColumnReferenceExtractor
   {
      private const double NormalDotEps = 0.9;

      private readonly ILogger _logger = Log.ForContext<ColumnReferenceExtractor>();

      public IColumnReferenceExtractor.SquareColumnReferences? ExtractSquare(FamilyInstance inst, View view)
      {
         var left = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.Left));
         var right = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.Right));
         var front = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.Front));
         var back = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.Back));
         var centerLR = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.CenterLeftRight));
         var centerFB = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.CenterFrontBack));

         if (left != null && right != null && front != null && back != null && centerLR != null && centerFB != null)
         {
            return new IColumnReferenceExtractor.SquareColumnReferences(left, right, front, back, centerLR, centerFB);
         }

         _logger.Debug("Primary refs incomplete for column {Id}, fallback to geometry traversal", inst.Id);
         return ExtractSquareFromGeometry(inst, view);
      }

      public IColumnReferenceExtractor.RoundColumnReferences? ExtractRound(FamilyInstance inst, View view)
      {
         if (inst.Location is not LocationPoint loc) return null;

         var opts = new Options { ComputeReferences = true, IncludeNonVisibleObjects = false, View = view };
         var arcRef = FindCylindricalFaceRef(inst.get_Geometry(opts));
         if (arcRef == null)
         {
            _logger.Warning("Không tìm thấy CylindricalFace cho cột tròn {Id}", inst.Id);
            return null;
         }

         return new IColumnReferenceExtractor.RoundColumnReferences(arcRef, loc.Point);
      }

      private static Reference? FindCylindricalFaceRef(GeometryElement? geom)
      {
         if (geom == null) return null;
         foreach (var go in geom)
         {
            var found = TraverseForCylindrical(go);
            if (found != null) return found;
         }
         return null;
      }

      private static Reference? TraverseForCylindrical(GeometryObject go)
      {
         switch (go)
         {
            case Solid solid:
               foreach (Face face in solid.Faces)
               {
                  if (face is CylindricalFace && face.Reference != null)
                  {
                     return face.Reference;
                  }
               }
               break;
            case GeometryInstance gi:
               foreach (var inner in gi.GetInstanceGeometry())
               {
                  var found = TraverseForCylindrical(inner);
                  if (found != null) return found;
               }
               break;
         }
         return null;
      }

      private IColumnReferenceExtractor.SquareColumnReferences? ExtractSquareFromGeometry(FamilyInstance inst, View view)
      {
         var opts = new Options { ComputeReferences = true, IncludeNonVisibleObjects = false, View = view };
         var geom = inst.get_Geometry(opts);
         if (geom == null) return null;

         Reference? leftRef = null;
         Reference? rightRef = null;
         Reference? frontRef = null;
         Reference? backRef = null;
         double leftX = double.MaxValue;
         double rightX = double.MinValue;
         double frontY = double.MaxValue;
         double backY = double.MinValue;

         foreach (var go in geom)
         {
            CollectPlanarFaces(go, inst.Id, view,
               ref leftRef, ref rightRef, ref frontRef, ref backRef,
               ref leftX, ref rightX, ref frontY, ref backY);
         }

         var centerLR = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.CenterLeftRight));
         var centerFB = FirstOrNull(inst.GetReferences(FamilyInstanceReferenceType.CenterFrontBack));

         if (leftRef == null || rightRef == null || frontRef == null || backRef == null || centerLR == null || centerFB == null)
         {
            return null;
         }

         return new IColumnReferenceExtractor.SquareColumnReferences(leftRef, rightRef, frontRef, backRef, centerLR, centerFB);
      }

      private static void CollectPlanarFaces(
         GeometryObject go,
         ElementId instanceId,
         View view,
         ref Reference? leftRef,
         ref Reference? rightRef,
         ref Reference? frontRef,
         ref Reference? backRef,
         ref double leftX,
         ref double rightX,
         ref double frontY,
         ref double backY)
      {
         switch (go)
         {
            case Solid solid:
               foreach (Face face in solid.Faces)
               {
                  if (face is not PlanarFace pf || pf.Reference == null) continue;
                  var n = pf.FaceNormal;
                  var origin = pf.Origin;

                  if (n.X < -NormalDotEps && origin.X < leftX) { leftRef = pf.Reference; leftX = origin.X; }
                  else if (n.X > NormalDotEps && origin.X > rightX) { rightRef = pf.Reference; rightX = origin.X; }
                  else if (n.Y < -NormalDotEps && origin.Y < frontY) { frontRef = pf.Reference; frontY = origin.Y; }
                  else if (n.Y > NormalDotEps && origin.Y > backY) { backRef = pf.Reference; backY = origin.Y; }
               }
               break;
            case GeometryInstance gi:
               foreach (var inner in gi.GetInstanceGeometry())
               {
                  CollectPlanarFaces(inner, instanceId, view,
                     ref leftRef, ref rightRef, ref frontRef, ref backRef,
                     ref leftX, ref rightX, ref frontY, ref backY);
               }
               break;
         }
      }

      private static Reference? FirstOrNull(IList<Reference>? refs)
      {
         if (refs == null || refs.Count == 0) return null;
         return refs[0];
      }
   }
}
