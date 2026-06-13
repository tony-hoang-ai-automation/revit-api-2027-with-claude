using MyRevitAIApp.AutoDimColumns;
using MyRevitAIApp.ColumnRebarViewer;
using MyRevitAIApp.Commands;
using MyRevitAIApp.DuplicateSheet;
using MyRevitAIApp.GeometryToDirectShape;
using MyRevitAIApp.McpServer;
using MyRevitAIApp.ViewSheetCreator;
using Nice3point.Revit.Toolkit.External;
using Serilog;
using Serilog.Events;

namespace MyRevitAIApp
{
   /// <summary>
   ///     Application entry point
   /// </summary>
   [UsedImplicitly]
   public class Application : ExternalApplication
   {
      public override void OnStartup()
      {
         CreateLogger();
         CreateRibbon();
      }

      public override void OnShutdown()
      {
         Log.CloseAndFlush();
      }

      private void CreateRibbon()
      {
         var panel = Application.CreatePanel("Commands", "MyRevitAIApp");

         panel.AddPushButton<StartupCommand>("Execute")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<HelloWorldCommand>("Hello World")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<DocumentInfoCommand>("Document\nInfo")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<DuplicateSheetsCommand>("Duplicate\nSheets")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<AutoDimColumnsCommand>("Dim\nCột")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<CreateSheetsFromExcelCommand>("Create Sheets\nfrom Excel")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<GeometryToDirectShapeCommand>("Geometry\n→ DirectShape")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         panel.AddPushButton<ColumnRebarViewerCommand>("Ve Thep\nCot")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png");

         var mcpPanel = Application.CreatePanel("MCP Server", "MyRevitAIApp");

         mcpPanel.AddPushButton<McpServerCommand>("MCP\nServer")
             .SetImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon16.png")
             .SetLargeImage("/MyRevitAIApp;component/Resources/Icons/RibbonIcon32.png")
             .SetToolTip("Bật / Tắt MCP Server (port 8080) để AI điều khiển Revit.");
      }

      private static void CreateLogger()
      {
         const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

         Log.Logger = new LoggerConfiguration()
             .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
             .MinimumLevel.Debug()
             .CreateLogger();

         AppDomain.CurrentDomain.UnhandledException += (_, args) =>
         {
            var exception = (Exception)args.ExceptionObject;
            Log.Fatal(exception, "Domain unhandled exception");
         };
      }
   }
}