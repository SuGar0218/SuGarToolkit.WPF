using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SuGarToolkit.WPF.Helpers.Converters;

public class DoubleToThicknessConverter : IValueConverter
{
    public static Thickness Convert(double value, ThicknessSides sides)
    {
        ThicknessSides[] allSides = [ThicknessSides.Left, ThicknessSides.Top, ThicknessSides.Right, ThicknessSides.Bottom];
        double[] result = new double[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = sides.HasFlag(allSides[i]) ? value : 0;
        }
        return new Thickness(result[0], result[1], result[2], result[3]);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is double doubleValue)
        {
            switch (parameter)
            {
                case ThicknessSides sides:
                    return Convert(doubleValue, sides);
                case int intValue:
                    return Convert(doubleValue, intValue.ToThicknessSides());
                default:
                    return Convert(doubleValue, ThicknessSides.Left | ThicknessSides.Top | ThicknessSides.Right | ThicknessSides.Bottom);
            }
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotSupportedException();
    }
}
