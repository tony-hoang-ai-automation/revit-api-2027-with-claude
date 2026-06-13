using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace MyRevitAIApp.Commands
{
   /// <summary>
   ///     First Revit add-in — displays a Hello World message.
   /// </summary>
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class HelloWorldCommand : ExternalCommand
   {
      public override void Execute()
      {
         TaskDialog.Show("Hello World", "Hello, Revit!");
      }
   }
}
