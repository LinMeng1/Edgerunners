using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NightCity.Core.Converters
{
    public class ObjectIsNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolVlue = false;
            if (value != DependencyProperty.UnsetValue && value != null)
                boolVlue = true;
            if (boolVlue) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
