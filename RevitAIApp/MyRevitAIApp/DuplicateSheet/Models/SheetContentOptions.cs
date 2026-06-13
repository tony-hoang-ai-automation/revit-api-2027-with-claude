namespace MyRevitAIApp.DuplicateSheet.Models
{
   /// <summary>
   ///     Flags chọn loại content cần copy từ source sheet sang sheet mới.
   ///     TitleBlock luôn được tạo bởi <c>ViewSheet.Create</c>; flag CopyTitleBlock
   ///     chỉ điều khiển việc align position để khớp source.
   /// </summary>
   public sealed record SheetContentOptions(
      bool CopyTitleBlock,
      bool IncludeLegends,
      bool IncludeSchedules,
      bool CopySheetAnnotations)
   {
      public static SheetContentOptions All { get; } = new(true, true, true, true);
   }
}
