namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Trích xuất <see cref="Reference"/> mặt + centerline của 1 cột FamilyInstance.
   ///     Cột vuông: 4 face refs + 2 centerline refs (qua GetReferences API hoặc fallback geometry).
   ///     Cột tròn: cylindrical face ref cho NewRadialDimension.
   /// </summary>
   public interface IColumnReferenceExtractor
   {
      SquareColumnReferences? ExtractSquare(FamilyInstance inst, View view);
      RoundColumnReferences? ExtractRound(FamilyInstance inst, View view);

      public sealed class SquareColumnReferences
      {
         public SquareColumnReferences(Reference left, Reference right, Reference front, Reference back, Reference centerLeftRight, Reference centerFrontBack)
         {
            Left = left;
            Right = right;
            Front = front;
            Back = back;
            CenterLeftRight = centerLeftRight;
            CenterFrontBack = centerFrontBack;
         }

         public Reference Left { get; }
         public Reference Right { get; }
         public Reference Front { get; }
         public Reference Back { get; }
         /// <summary>Centerline plane chia Left/Right — vuông góc trục X — dùng cho SelfDimX.</summary>
         public Reference CenterLeftRight { get; }
         /// <summary>Centerline plane chia Front/Back — vuông góc trục Y — dùng cho SelfDimY.</summary>
         public Reference CenterFrontBack { get; }
      }

      public sealed class RoundColumnReferences
      {
         public RoundColumnReferences(Reference arc, XYZ origin)
         {
            Arc = arc;
            Origin = origin;
         }

         public Reference Arc { get; }
         public XYZ Origin { get; }
      }
   }
}
