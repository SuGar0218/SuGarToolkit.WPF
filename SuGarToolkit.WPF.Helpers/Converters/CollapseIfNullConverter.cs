using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SuGarToolkit.WPF.Helpers.Converters;

public class CollapseIfNullConverter : IValueConverter
{
    public static Visibility Convert(object? value) => value is null ? Visibility.Collapsed : Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
