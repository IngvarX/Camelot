using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Camelot.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actual = System.Convert.ToInt32(value);
        var expected = System.Convert.ToInt32(parameter);

        return actual == expected;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}