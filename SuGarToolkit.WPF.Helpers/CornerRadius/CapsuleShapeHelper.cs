using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SuGarToolkit.WPF.Helpers;

public class CapsuleShapeHelper
{
    public static void ShapeControlIntoCapsule(Control control)
    {
        if (VisualTreeHelper.GetChildrenCount(control) > 0 && VisualTreeHelper.GetChild(control, 0) is Border border)
        {
            ShapeBorderIntoCapsule(border);
        }
    }

    public static void ShapeBorderIntoCapsule(Border border)
    {
        border.CornerRadius = new CornerRadius(Math.Min(border.ActualWidth, border.ActualHeight) / 2);
    }

    private static void ShapeFrameworkElementIntoCapsule(FrameworkElement element)
    {
        if (element is Border border)
        {
            ShapeBorderIntoCapsule(border);
            return;
        }
        if (element is Control control)
        {
            ShapeControlIntoCapsule(control);
            return;
        }
    }

    public static bool GetShapeIntoCapsule(Control target) => (bool) target.GetValue(ShapeIntoCapsuleProperty);
    public static void SetShapeIntoCapsule(Control target, bool value) => target.SetValue(ShapeIntoCapsuleProperty, value);

    public static readonly DependencyProperty ShapeIntoCapsuleProperty = DependencyProperty.RegisterAttached(
        "ShapeIntoCapsule",
        typeof(bool),
        typeof(FrameworkElement),
        new PropertyMetadata(default(bool), OnShapeIntoCapsuleChanged)
    );

    private static void OnShapeIntoCapsuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement target = (FrameworkElement) d;
        ShapeFrameworkElementIntoCapsule(target);
        if ((bool) e.NewValue)
        {
            target.SizeChanged += OnSizeChanged;
        }
        else
        {
            target.SizeChanged -= OnSizeChanged;
        }
    }

    private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        FrameworkElement target = (FrameworkElement) sender;
        ShapeFrameworkElementIntoCapsule(target);
    }
}
