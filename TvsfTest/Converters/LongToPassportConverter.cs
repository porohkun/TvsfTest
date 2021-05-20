using System;
using System.Globalization;
using System.Windows.Data;

namespace TvsfTest
{
    public class LongToPassportConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is long longValue))
                return "";
            if (longValue < 0)
                return "";
            var stringValue = longValue.ToString();
            if (stringValue.Length < 4)
                return stringValue;
            return $"{stringValue.Substring(0, 4)} {stringValue.Substring(4, stringValue.Length - 4)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string stringValue))
                return 0;
            var result = 0l;
            foreach (var c in stringValue)
                if (char.IsDigit(c))
                    result = result * 10 + c - 48;
            return result;
        }
    }
}
