using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NightCity.Converters
{
    internal class BannerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string Category = parameter.ToString();
            switch (Category)
            {
                case "MessageBackground":
                    {
                        string Urgency = null;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            Urgency = values[0].ToString();
                        if (Urgency == null)
                            return new SolidColorBrush(Colors.White);
                        int priority = 0;
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null)
                            priority = System.Convert.ToInt32(values[1]);
                        switch (Urgency)
                        {
                            case "Inform":
                                if (priority < 25)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3E5FC"));
                                else if (priority < 50)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FC3F7"));
                                else if (priority < 75)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#039BE5"));
                                else
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0277BD"));
                            case "Plan":
                                if (priority < 25)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF9C4"));
                                else if (priority < 50)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF176"));
                                else if (priority < 75)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDD835"));
                                else
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9A825"));
                            case "Execute":
                                if (priority < 25)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCDD2"));
                                else if (priority < 50)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E57373"));
                                else if (priority < 75)
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53935"));
                                else
                                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828"));
                            default:
                                return new SolidColorBrush(Colors.White);
                        }
                    }
                case "MessageForeground":
                    {
                        string Urgency = null;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            Urgency = values[0].ToString();
                        if (Urgency == null)
                            return new SolidColorBrush(Colors.Black);
                        int priority = 0;
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null)
                            priority = System.Convert.ToInt32(values[1]);
                        switch (Urgency)
                        {
                            case "Inform":
                                if (priority < 50)
                                    return new SolidColorBrush(Colors.Black);
                                else
                                    return new SolidColorBrush(Colors.White);
                            case "Plan":
                                return new SolidColorBrush(Colors.Black);
                            case "Execute":
                                if (priority < 25)
                                    return new SolidColorBrush(Colors.Black);
                                else
                                    return new SolidColorBrush(Colors.White);
                            default:
                                return new SolidColorBrush(Colors.White);
                        }                    
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
