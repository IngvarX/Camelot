using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Camelot.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var actualOperationType = System.Convert.ToInt32(value);
            var expectedOperationType = System.Convert.ToInt32(parameter);

            return actualOperationType == expectedOperationType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}