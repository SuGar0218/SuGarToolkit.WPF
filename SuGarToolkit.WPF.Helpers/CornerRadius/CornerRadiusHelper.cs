using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SuGarToolkit.WPF.Helpers;

public class CornerRadiusHelper
{
    public static CornerRadius GetCornerRadius(Control target) => (CornerRadius) target.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Control target, CornerRadius value) => target.SetValue(CornerRadiusProperty, value);

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(Control),
        new PropertyMetadata(default(CornerRadius), OnCornerRadiusChanged)
    );

    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Control control)
            return;

        if (!control.IsLoaded)
        {
            control.Loaded += OnLoaded;
            void OnLoaded(object sender, RoutedEventArgs args)
            {
                control.Loaded -= OnLoaded;
                if (VisualTreeHelper.GetChildrenCount(control) > 0 && VisualTreeHelper.GetChild(control, 0) is Border border)
                {
                    border.CornerRadius = (CornerRadius) e.NewValue;
                }
            }
        }
        else if (VisualTreeHelper.GetChildrenCount(control) > 0 && VisualTreeHelper.GetChild(control, 0) is Border border)
        {
            border.CornerRadius = (CornerRadius) e.NewValue;
        }
    }
}
