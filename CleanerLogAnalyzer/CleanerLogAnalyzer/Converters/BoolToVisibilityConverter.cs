using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CleanerLogAnalyzer.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result;
            if (value is bool && (bool)value) result = Visibility.Visible;
            else result = Visibility.Collapsed;

            if (string.Equals(ConverterConstans.Inverse, parameter as string, StringComparison.OrdinalIgnoreCase))
                result = result == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value is Visibility && (Visibility)value == Visibility.Visible;

            if (string.Equals(ConverterConstans.Inverse, parameter as string, StringComparison.OrdinalIgnoreCase))
                result = !result;

            return result;
        }
    }
}
