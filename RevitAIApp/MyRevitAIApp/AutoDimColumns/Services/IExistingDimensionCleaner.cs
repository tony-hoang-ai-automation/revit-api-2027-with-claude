namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Tìm và xoá Dimension cũ trong view tham chiếu đến các target column IDs.
   ///     Caller phải wrap trong Transaction (modifies document).
   /// </summary>
   public interface IExistingDimensionCleaner
   {
      /// <summary>
      ///     Đếm số Dimension trong view có Reference trỏ đến bất kỳ ID nào trong <paramref name="targetColumnIds"/>.
      ///     KHÔNG modify document — dùng cho preview / confirm dialog.
      /// </summary>
      int CountOverlapping(Document doc, View view, IEnumerable<long> targetColumnIds);

      /// <summary>
      ///     Xoá Dimension overlapping với targetColumnIds. Phải gọi trong Transaction.
      /// </summary>
      int DeleteOverlapping(Document doc, View view, IEnumerable<long> targetColumnIds);
   }
}
