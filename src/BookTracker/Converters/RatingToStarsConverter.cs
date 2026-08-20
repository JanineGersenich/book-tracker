using System;
using System.Globalization;
using System.Windows.Data;

namespace BookTracker.Converters;

public class RatingToStarsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int rating || rating < 1)
        {
            return "–";
        }

        return new string('★', rating) + new string('☆', 5 - rating);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
