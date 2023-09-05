using NightCity.Core.Models.Standard;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NightCity.Converters
{
    internal class ConnectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string Category = parameter.ToString();
            switch (Category)
            {
                case "IsClusterTopic":
                    {
                        MqttTopic selectedTopic = null;
                        if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                            selectedTopic = (MqttTopic)values[0];
                        if (selectedTopic != null && selectedTopic.Origin == null)
                            return true;
                        else
                            return false;
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
