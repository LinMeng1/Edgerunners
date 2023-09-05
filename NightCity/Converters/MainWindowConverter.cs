using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NightCity.Converters
{
    internal class MainWindowConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string Category = parameter.ToString();
            switch (Category)
            {
                case "BannerMessageCountText":
                    {
                        int bannerMessageCount = 0;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            bannerMessageCount = System.Convert.ToInt32(values[0]);
                        return bannerMessageCount > 1 ? $"{bannerMessageCount} Messages" : $"{bannerMessageCount} Message";
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
