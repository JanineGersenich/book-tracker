using System;
using System.Globalization;
using System.Windows.Data;
using BookTracker.Models;

namespace BookTracker.Converters;

public class StatusToGermanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            ReadingStatus.ToRead => "Ungelesen",
            ReadingStatus.Reading => "Wird gelesen",
            ReadingStatus.Finished => "Gelesen",
            ReadingStatus.Abandoned => "Abgebrochen",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
