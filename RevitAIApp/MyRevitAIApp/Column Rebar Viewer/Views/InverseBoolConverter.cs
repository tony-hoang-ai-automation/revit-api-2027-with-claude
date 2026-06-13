using System.Globalization;
using System.Windows.Data;
using Visibility = System.Windows.Visibility;

namespace MyRevitAIApp.ColumnRebarViewer.Views
{
   /// <summary>true → Collapsed, false → Visible (ngược BooleanToVisibilityConverter).</summary>
   [ValueConversion(typeof(bool), typeof(Visibility))]
   public sealed class InverseBoolToVisibilityConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         => value is true ? Visibility.Collapsed : Visibility.Visible;

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         => value is Visibility.Collapsed;
   }

   /// <summary>true → false và ngược lại.</summary>
   [ValueConversion(typeof(bool), typeof(bool))]
   public sealed class InverseBoolConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         => value is not true;

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         => value is not true;
   }
}
