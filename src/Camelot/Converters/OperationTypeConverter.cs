using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Converters
{
    public class OperationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var actualOperationType = (OperationType) value;
            var expectedOperationType = (OperationType) parameter;

            return actualOperationType == expectedOperationType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}