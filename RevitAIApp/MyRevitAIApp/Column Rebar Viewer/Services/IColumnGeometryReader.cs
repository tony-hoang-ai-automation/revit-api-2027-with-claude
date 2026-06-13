using MyRevitAIApp.ColumnRebarViewer.Models;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   public interface IColumnGeometryReader
   {
      ColumnGeometry Read(FamilyInstance column, View activeView);
   }
}
