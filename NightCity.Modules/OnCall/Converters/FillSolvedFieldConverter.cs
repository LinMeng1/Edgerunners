using NightCity.Core.Models.Standard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace OnCall.Converters
{
    internal class FillSolvedFieldConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string Category = parameter.ToString();
            switch (Category)
            {
                case "Collection":
                    {
                        string value1 = string.Empty;
                        string value2 = string.Empty;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            value1 = values[0].ToString();
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null)
                            value2 = values[1].ToString();
                        if (value1 == string.Empty && value2 == string.Empty)
                            return Visibility.Collapsed;
                        else
                            return Visibility.Visible;
                    }
                case "Label":
                    {
                        string value = string.Empty;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            value = values[0].ToString();
                        if (value == string.Empty)
                            return Visibility.Collapsed;
                        else
                            return Visibility.Visible;
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
