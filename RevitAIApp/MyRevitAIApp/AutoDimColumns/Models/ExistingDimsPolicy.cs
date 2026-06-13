namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Chính sách xử lý khi phát hiện dim cũ đã tồn tại trên cột target.
   /// </summary>
   public enum ExistingDimsPolicy
   {
      /// <summary>Bỏ qua cột đó, không tạo dim mới. Log warning.</summary>
      SkipColumn,

      /// <summary>Xoá dim cũ (overlap với refs target) rồi tạo lại. Default v1.</summary>
      DeleteThenRecreate,

      /// <summary>Luôn tạo dim mới, không quan tâm dim cũ. Có thể chồng chéo visual.</summary>
      AlwaysCreate
   }
}
