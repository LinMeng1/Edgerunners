using System;
using System.Globalization;
using System.Windows.Data;

namespace NightCity.Core.Converters
{
    public class WrapTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string text = value.ToString();
            text = text.Replace(@"\n", "\n");
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
