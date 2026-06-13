using MyRevitAIApp.GeometryToDirectShape.Internals;
using MyRevitAIApp.GeometryToDirectShape.Models;

namespace MyRevitAIApp.GeometryToDirectShape
{
   /// <summary>
   ///     Trích các Solid từ một phần tử Revit (theo mode Instance/Symbol) rồi dựng DirectShape
   ///     vào model. Toàn bộ mutation gói trong 1 transaction.
   /// </summary>
   public sealed class DirectShapeBuilderService
   {
      private const string AppId = "MyRevitAIApp.GeometryToDirectShape";
      private const double FeetToMm = 304.8;
      private const double CubicFeetToCubicMeters = 0.0283168466;
      private const double MinVolume = 1e-9;

      /// <summary>
      ///     Tạo 1 DirectShape từ geometry của <paramref name="source"/> theo <paramref name="mode"/>.
      ///     Trả về <see cref="DirectShapeCreationResult"/> kể cả khi không trích được solid nào.
      /// </summary>
      public DirectShapeCreationResult Create(Document doc, Element source, GeometrySourceMode mode)
      {
         var solids = ExtractSolids(source, mode, out var hadInstance);
         if (solids.Count == 0)
         {
            return new DirectShapeCreationResult(mode, 0, 0, 0, 0, 0, 0,
               "Không tìm thấy Solid hợp lệ trong phần tử.");
         }

         ElementId newId;
         using (var t = new Transaction(doc, $"Tạo DirectShape ({mode})"))
         {
            t.Start();

            var directShape = DirectShape.CreateElement(doc, CategoryIdHelper.GenericModel());
            directShape.ApplicationId = AppId;
            directShape.ApplicationDataId = Guid.NewGuid().ToString();
            directShape.Name = $"GeometryDemo_{mode}";
            directShape.SetShape(solids.Cast<GeometryObject>().ToList());

            newId = directShape.Id;
            t.Commit();
         }

         var totalVolumeFt3 = solids.Sum(s => s.Volume);
         var created = doc.GetElement(newId);
         var bbox = created?.get_BoundingBox(null);
         var center = bbox != null ? (bbox.Min + bbox.Max) * 0.5 : XYZ.Zero;

         var note = hadInstance
            ? "OK"
            : "Phần tử không phải Family Instance — Instance & Symbol cho kết quả giống nhau.";

         return new DirectShapeCreationResult(
            Mode: mode,
            SolidCount: solids.Count,
            VolumeCubicMeters: totalVolumeFt3 * CubicFeetToCubicMeters,
            CenterXmm: center.X * FeetToMm,
            CenterYmm: center.Y * FeetToMm,
            CenterZmm: center.Z * FeetToMm,
            DirectShapeId: newId.GetIdValue(),
            Note: note);
      }

      /// <summary>
      ///     Duyệt GeometryElement, gom các Solid không suy biến. Với mỗi GeometryInstance,
      ///     lấy geometry theo mode: Symbol = chưa áp transform (toạ độ family),
      ///     Instance = đã áp transform (toạ độ model).
      /// </summary>
      private static IReadOnlyList<Solid> ExtractSolids(Element source, GeometrySourceMode mode, out bool hadInstance)
      {
         var options = new Options
         {
            ComputeReferences = false,
            IncludeNonVisibleObjects = false,
            DetailLevel = ViewDetailLevel.Fine
         };

         var solids = new List<Solid>();
         hadInstance = false;

         var geometry = source.get_Geometry(options);
         if (geometry == null) return solids;

         Collect(geometry, mode, solids, ref hadInstance);
         return solids;
      }

      private static void Collect(GeometryElement geometry, GeometrySourceMode mode, List<Solid> solids, ref bool hadInstance)
      {
         foreach (var geometryObject in geometry)
         {
            switch (geometryObject)
            {
               case Solid solid when solid.Volume > MinVolume && solid.Faces.Size > 0:
                  solids.Add(solid);
                  break;

               case GeometryInstance instance:
                  hadInstance = true;
                  var nested = mode == GeometrySourceMode.Symbol
                     ? instance.GetSymbolGeometry()
                     : instance.GetInstanceGeometry();
                  if (nested != null)
                     Collect(nested, mode, solids, ref hadInstance);
                  break;
            }
         }
      }
   }
}
