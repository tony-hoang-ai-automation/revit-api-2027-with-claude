namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Kiểu bố trí thép dọc chủ trên tiết diện.
   ///     ByPerimeter: nhập tổng số thanh n, app chia đều quanh chu vi.
   ///     BySides: nhập số thanh theo cạnh b (nB) và cạnh h (nH), 4 góc dùng chung.
   /// </summary>
   public enum LongitudinalLayout
   {
      ByPerimeter,
      BySides
   }
}
