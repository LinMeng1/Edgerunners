using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Lock.Converters
{
    public class EnabledTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool Enabled = false;
            if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                Enabled = (bool)values[0];
            string Dist = parameter.ToString();

            string Text;
            SolidColorBrush Foreground;

            if (Enabled)
            {
                Text = "Enabled";
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#388E3C"));
            }
            else
            {
                Text = "Disabled";
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
            }

            if (Dist == "Text")
                return Text;
            else if (Dist == "Foreground")
                return Foreground;
            else
                return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
