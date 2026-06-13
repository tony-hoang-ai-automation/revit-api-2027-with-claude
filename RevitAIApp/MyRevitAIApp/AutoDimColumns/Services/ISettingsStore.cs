using MyRevitAIApp.AutoDimColumns.Models;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Persist <see cref="DimensioningSettings"/> qua filesystem giữa các lần mở dialog.
   ///     File location: %LocalAppData%\MyRevitAIApp\autoDimColumns.xml
   /// </summary>
   public interface ISettingsStore
   {
      DimensioningSettings Load();
      void Save(DimensioningSettings settings);
   }
}
