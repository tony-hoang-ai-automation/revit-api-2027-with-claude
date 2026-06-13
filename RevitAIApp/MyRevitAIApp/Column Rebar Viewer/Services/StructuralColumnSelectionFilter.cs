using Autodesk.Revit.UI.Selection;
using MyRevitAIApp.ColumnRebarViewer.Internals;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   /// <summary>
   ///     ISelectionFilter chỉ cho phép chọn cột kết cấu (OST_StructuralColumns + OST_Columns).
   /// </summary>
   public sealed class StructuralColumnSelectionFilter : ISelectionFilter
   {
      public bool AllowElement(Element elem)
      {
         if (elem is not FamilyInstance fi) return false;
         var cat = fi.Category?.Id?.GetIdValue();
         return cat == (long)BuiltInCategory.OST_StructuralColumns
             || cat == (long)BuiltInCategory.OST_Columns;
      }

      public bool AllowReference(Reference reference, XYZ position) => false;
   }
}
