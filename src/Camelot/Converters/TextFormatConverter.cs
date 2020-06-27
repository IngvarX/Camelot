using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Camelot.Converters
{
    public class TextFormatConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var format = parameter as string;
            return !string.IsNullOrEmpty(format) ? string.Format(culture, format, values.ToArray()) : null;
        }
    }
}
