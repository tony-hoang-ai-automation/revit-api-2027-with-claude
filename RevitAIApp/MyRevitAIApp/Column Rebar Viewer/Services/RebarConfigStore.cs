using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using MyRevitAIApp.ColumnRebarViewer.Models;
using Serilog;

namespace MyRevitAIApp.ColumnRebarViewer.Services
{
   /// <summary>
   ///     Lưu/đọc cấu hình thép ra file JSON qua SaveFileDialog/OpenFileDialog.
   /// </summary>
   public sealed class RebarConfigStore : IRebarConfigStore
   {
      private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
      private readonly ILogger _logger = Log.ForContext<RebarConfigStore>();

      public void Save(RebarConfigDto dto)
      {
         var dlg = new SaveFileDialog
         {
            Title = "Lưu cấu hình thép cột",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = "json",
            FileName = "rebar-config"
         };
         if (dlg.ShowDialog() != true) return;

         try
         {
            var json = JsonSerializer.Serialize(dto, JsonOpts);
            File.WriteAllText(dlg.FileName, json);
            _logger.Information("Rebar config saved to {Path}", dlg.FileName);
         }
         catch (Exception ex)
         {
            _logger.Warning(ex, "Failed to save rebar config");
         }
      }

      public RebarConfigDto? Load()
      {
         var dlg = new OpenFileDialog
         {
            Title = "Mở cấu hình thép cột",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = "json"
         };
         if (dlg.ShowDialog() != true) return null;

         try
         {
            var json = File.ReadAllText(dlg.FileName);
            var dto = JsonSerializer.Deserialize<RebarConfigDto>(json);
            _logger.Information("Rebar config loaded from {Path}", dlg.FileName);
            return dto;
         }
         catch (Exception ex)
         {
            _logger.Warning(ex, "Failed to load rebar config from {Path}", dlg.FileName);
            return null;
         }
      }
   }
}
