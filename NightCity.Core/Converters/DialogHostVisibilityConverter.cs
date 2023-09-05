using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NightCity.Core.Converters
{
    public class DialogHostVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string DialogHostCategory = string.Empty;
            if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                DialogHostCategory = values[0].ToString();
            string DistCategory = parameter.ToString();
            if (DialogHostCategory == DistCategory && DistCategory != string.Empty)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
