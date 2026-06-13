namespace MyRevitAIApp.DuplicateSheet.Models
{
   /// <summary>
   ///     View duplication mode applied to non-legend views on the source sheet.
   ///     Maps 1:1 to <see cref="Autodesk.Revit.DB.ViewDuplicateOption"/>.
   /// </summary>
   public enum DuplicateMode
   {
      /// <summary>Plain view duplicate without annotations.</summary>
      Duplicate,

      /// <summary>Duplicate with view-specific annotations (dimensions, tags) preserved.</summary>
      WithDetailing,

      /// <summary>Create a dependent view of the source view.</summary>
      AsDependent
   }
}
