using System.Globalization;
using System.Windows.Data;

namespace WpfSynchronizationContext.Auxiliary.Converters
{
   /// <summary>
   /// Преобразует логическое значение в наименование цвета элемента, являющегося индикатором состояния подключения.
   /// </summary>
   [ValueConversion(typeof(bool), typeof(string))]
   public class BoolToFillColorConverter : IValueConverter
   {
      #region IValueConverter

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         => (bool)value ? "Green" : "Red";

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

      #endregion IValueConverter
   }
}
