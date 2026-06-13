namespace MyRevitAIApp.DuplicateSheet.Models
{
   /// <summary>
   ///     Row trong DataGrid sheet list. <c>IsSelected</c> 2-way binding với checkbox.
   ///     Number/Name là display fields, không cần observable (load 1 lần lúc init).
   /// </summary>
   public sealed partial class SheetItem : ObservableObject
   {
      [ObservableProperty]
      private bool _isSelected;

      public SheetItem(ElementId id, string number, string name)
      {
         Id = id;
         Number = number;
         Name = name;
      }

      public ElementId Id { get; }

      public string Number { get; }

      public string Name { get; }
   }
}
