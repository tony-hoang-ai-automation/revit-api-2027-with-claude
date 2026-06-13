namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     DTO lưu/đọc cấu hình thép cột ra file JSON (System.Text.Json).
   ///     Toàn bộ field là kiểu primitive/string để serialize ổn định cross-version.
   /// </summary>
   public sealed record RebarConfigDto
   {
      public double SectionB { get; init; }
      public double SectionH { get; init; }
      public double ColumnHeightMm { get; init; }

      public int LongBarDiameter { get; init; }
      public int LongBarCount { get; init; }
      public string Layout { get; init; } = nameof(LongitudinalLayout.ByPerimeter);
      public int BarsAlongB { get; init; }
      public int BarsAlongH { get; init; }

      public int StirrupDiameter { get; init; }
      public double StirrupSpacing { get; init; }
      public double ConfinementSpacing { get; init; }
      public double ConfinementLength { get; init; }
      public bool AutoTcvn { get; init; }

      public double CoverMm { get; init; }
      public string Concrete { get; init; } = nameof(ConcreteGrade.B25);
      public string Steel { get; init; } = nameof(SteelGroup.CB400);

      public double LneoMm { get; init; }
      public double LnoiMm { get; init; }
   }
}
