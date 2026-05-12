using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SuGarToolkit.WPF.Controls.Windows;

public partial class WindowOverflowOverlay : Window
{
    static WindowOverflowOverlay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowOverflowOverlay), new FrameworkPropertyMetadata(typeof(WindowOverflowOverlay)));
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual(this);
        IntPtr handle = hwndSource.Handle;
        // Avoid the overlay getting focus
        SetWindowLong(handle, GWL_STYLE, new IntPtr(WS_CHILD | GetWindowLong(handle, GWL_STYLE).ToInt32()));
        SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | GetWindowLong(handle, GWL_EXSTYLE).ToInt32()));
    }

    private double _dpiScaleX = 1.0;
    private double _dpiScaleY = 1.0;

    public static WindowOverflowOverlay GetWindowOverflowOverlay(Window target) => (WindowOverflowOverlay) target.GetValue(WindowOverflowOverlayProperty);
    public static void SetWindowOverflowOverlay(Window target, WindowOverflowOverlay value) => target.SetValue(WindowOverflowOverlayProperty, value);

    public static readonly DependencyProperty WindowOverflowOverlayProperty = DependencyProperty.RegisterAttached(
        "WindowOverflowOverlay",
        typeof(WindowOverflowOverlay),
        typeof(Window),
        new PropertyMetadata(default(WindowOverflowOverlay), OnWindowOverflowOverlayChanged)
    );

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);
        Owner?.Activate();
    }

    private static void OnWindowOverflowOverlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Window target = (Window) d;
        if (e.OldValue is WindowOverflowOverlay oldOverlay)
        {
            oldOverlay.Owner = null;
            target.SourceInitialized -= OnTargetWindowSourceInitialized;
            target.Loaded -= OnTargetWindowLoaded;
            target.LocationChanged -= OnTargetWindowLocationChanged;
            target.SizeChanged -= OnTargetWindowSizeChanged;
            target.IsVisibleChanged -= OnTargetWindowIsVisibleChanged;
        }
        if (e.NewValue is WindowOverflowOverlay newOverlay)
        {
            target.Loaded += OnTargetWindowLoaded;
            target.LocationChanged += OnTargetWindowLocationChanged;
            target.SizeChanged += OnTargetWindowSizeChanged;
            if (target.IsVisible)
            {
                newOverlay.Owner = target;
            }
            else
            {
                target.IsVisibleChanged += OnTargetWindowIsVisibleChanged;
            }
            if (target.IsInitialized)
            {
                (double dpiScaleX, double dpiScaleY) = GetVisualDpiScale(target);
                newOverlay._dpiScaleX = dpiScaleX;
                newOverlay._dpiScaleY = dpiScaleY;
            }
            else
            {
                target.SourceInitialized += OnTargetWindowSourceInitialized;
            }
        }
    }

    private static void OnTargetWindowSourceInitialized(object? sender, EventArgs e)
    {
        if (sender is not Window target)
            return;

        WindowOverflowOverlay? overlay = GetWindowOverflowOverlay(target);
        if (overlay is null)
            return;

        HwndSource source = (HwndSource) PresentationSource.FromVisual(target);
        Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
        overlay._dpiScaleX = transformToDevice.M11;
        overlay._dpiScaleY = transformToDevice.M22;
    }

    private static void OnTargetWindowLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not Window target)
            return;

        WindowOverflowOverlay? overlay = GetWindowOverflowOverlay(target);
        if (overlay is null)
            return;

        overlay.Dispatcher.BeginInvoke(new Action(async () =>
        {
            RefreshOverlayLocation(overlay, target);
            overlay.Topmost = true;
            overlay.Show();
            await Task.Delay(100);
            overlay.Topmost = false;
        }));
    }

    private static void OnTargetWindowIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Window target)
            return;

        WindowOverflowOverlay? overlay = GetWindowOverflowOverlay(target);
        if (overlay is null)
            return;

        if (target.IsVisible)
        {
            overlay.Owner = target;
        }
    }

    private static void OnTargetWindowLocationChanged(object? sender, EventArgs e)
    {
        if (sender is not Window target)
            return;

        WindowOverflowOverlay overlay = GetWindowOverflowOverlay(target);
        if (overlay is null)
            return;

        RefreshOverlayLocation(overlay, target);
    }

    private static void OnTargetWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Window target = (Window) sender;
        WindowOverflowOverlay overlay = GetWindowOverflowOverlay(target);
        if (overlay is null)
            return;

        overlay.Width = target.ActualWidth - overlay.Margin.Left - overlay.Margin.Right;
        overlay.Height = target.ActualHeight - overlay.Margin.Top - overlay.Margin.Bottom;
    }

    private static void RefreshOverlayLocation(WindowOverflowOverlay overlay, Window target)
    {
        Point windowPixelLocation = target.PointToScreen(new Point(0, 0));
        if (windowPixelLocation.X == -32000 && windowPixelLocation.Y == -32000)
            return;

        overlay.Left = windowPixelLocation.X / overlay._dpiScaleX + overlay.Margin.Left;
        overlay.Top = windowPixelLocation.Y / overlay._dpiScaleY + overlay.Margin.Top;
    }

    private static (double, double) GetVisualDpiScale(Visual visual)
    {
        HwndSource source = (HwndSource) PresentationSource.FromVisual(visual);
        Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
        double dpiScaleX = transformToDevice.M11;
        double dpiScaleY = transformToDevice.M22;
        return (dpiScaleX, dpiScaleY);
    }

    private static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) => Environment.Is64BitProcess ? GetWindowLong64(hWnd, nIndex) : GetWindowLong32(hWnd, nIndex);

    private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) => Environment.Is64BitProcess ? SetWindowLong64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static partial IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static partial IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static partial IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static partial IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;
    private const int WS_CHILD = 0x40000000;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WM_MOUSEACTIVATE = 0x0021;
    private const int MA_NOACTIVATE = 0x0003;
}
