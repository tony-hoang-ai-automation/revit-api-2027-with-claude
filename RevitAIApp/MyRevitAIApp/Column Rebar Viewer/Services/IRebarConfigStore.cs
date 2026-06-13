using MyRevitAIApp.ColumnRebarViewer.Models;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   public interface IRebarConfigStore
   {
      void Save(RebarConfigDto dto);
      RebarConfigDto? Load();
   }
}
