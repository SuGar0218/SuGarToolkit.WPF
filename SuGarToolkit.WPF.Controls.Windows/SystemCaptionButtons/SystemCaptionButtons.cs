using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;

namespace SuGarToolkit.WPF.Controls.Windows;

public class SystemCaptionButtons
{
    public static bool GetCanMinimize(Window target) => (bool)target.GetValue(CanMinimizeProperty);
    public static void SetCanMinimize(Window target, bool value) => target.SetValue(CanMinimizeProperty, value);

    public static readonly DependencyProperty CanMinimizeProperty = DependencyProperty.RegisterAttached(
        "CanMinimize",
        typeof(bool),
        typeof(SystemCaptionButtons),
        new PropertyMetadata(true, OnCanMinimizeChanged)
    );

    private static void OnCanMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Window window = (Window)d;
        bool can = (bool)e.NewValue;
        nint handle = RetrieveWindowHandle(window);
        if (handle != nint.Zero)
        {
            Win32SystemCaptionButtons.SetCanMinimize(handle, can);
            return;
        }
        ExecuteOnceAfterSourceInitialized(window, () =>
        {
            Win32SystemCaptionButtons.SetCanMinimize(handle, can);
        });
    }

    public static bool GetCanMaximize(Window target) => (bool)target.GetValue(CanMaximizeProperty);
    public static void SetCanMaximize(Window target, bool value) => target.SetValue(CanMaximizeProperty, value);

    public static readonly DependencyProperty CanMaximizeProperty = DependencyProperty.RegisterAttached(
        "CanMaximize",
        typeof(bool),
        typeof(SystemCaptionButtons),
        new PropertyMetadata(true, OnCanMaximizeChanged)
    );

    private static void OnCanMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Window window = (Window)d;
        bool can = (bool)e.NewValue;
        nint handle = RetrieveWindowHandle(window);
        if (handle != nint.Zero)
        {
            Win32SystemCaptionButtons.SetCanMaximize(handle, can);
            return;
        }
        ExecuteOnceAfterSourceInitialized(window, () =>
        {
            Win32SystemCaptionButtons.SetCanMaximize(handle, can);
        });
    }

    public static bool GetIsVisible(Window target) => (bool)target.GetValue(IsVisibleProperty);
    public static void SetIsVisible(Window target, bool value) => target.SetValue(IsVisibleProperty, value);

    public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
        "IsVisible",
        typeof(bool),
        typeof(SystemCaptionButtons),
        new PropertyMetadata(true, OnIsVisibleChanged)
    );

    private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Window window = (Window)d;
        bool visible = (bool)e.NewValue;
        nint handle = RetrieveWindowHandle(window);
        if (handle != nint.Zero)
        {
            Win32SystemCaptionButtons.SetIsVisible(handle, visible);
            return;
        }
        ExecuteOnceAfterSourceInitialized(window, () =>
        {
            Win32SystemCaptionButtons.SetIsVisible(handle, visible);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nint RetrieveWindowHandle(Window window) => new WindowInteropHelper(window).Handle;

    private static void ExecuteOnceAfterSourceInitialized(Window window, Action action)
    {
        window.SourceInitialized += ExecuteActionOnSourceInitialized;
        void ExecuteActionOnSourceInitialized(object? sender, EventArgs e)
        {
            window.SourceInitialized -= ExecuteActionOnSourceInitialized;
            action.Invoke();
        }
    }
}
