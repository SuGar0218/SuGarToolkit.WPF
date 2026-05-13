using System.Windows;
using System.Windows.Controls.Primitives;

namespace SuGarToolkit.WPF.Helpers;

public partial class TextBoxInputMonitor
{
    public static bool GetIsEnabled(TextBoxBase target) => (bool) target.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(TextBoxBase target, bool value) => target.SetValue(IsEnabledProperty, value);

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(TextBoxBase),
        new PropertyMetadata(default(bool), OnIsEnabledChanged)
    );

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBoxBase textBox = (TextBoxBase) d;
        if ((bool) e.NewValue)
        {
            _monitors.Add(textBox, new TextBoxInputMonitor(textBox));
        }
        else if (_monitors.TryGetValue(textBox, out var monitor))
        {
            monitor.Dispose();
            _monitors.Remove(textBox);
        }
    }

    private static readonly Dictionary<TextBoxBase, TextBoxInputMonitor> _monitors = [];
}
