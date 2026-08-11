using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace SuGarToolkit.WPF.Controls.Windows;

public class Win32SystemCaptionButtons
{
    public static bool GetCanMaximize(nint hwnd)
    {
        HWND handle = new(hwnd);
        int style = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        return (style & WS_MAXIMIZEBOX) != 0;
    }

    public static int SetCanMaximize(nint hwnd, bool value)
    {
        HWND handle = new(hwnd);
        int style = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        if (value)
        {
            style |= WS_MAXIMIZEBOX;
        }
        else
        {
            style &= ~WS_MAXIMIZEBOX;
        }
        return PInvoke.SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }

    public static bool GetCanMinimize(nint hwnd)
    {
        HWND handle = new(hwnd);
        int style = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        return (style & WS_MINIMIZEBOX) != 0;
    }

    public static int SetCanMinimize(nint hwnd, bool value)
    {
        HWND handle = new(hwnd);
        int style = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        if (value)
        {
            style |= WS_MINIMIZEBOX;
        }
        else
        {
            style &= ~WS_MINIMIZEBOX;
        }
        return PInvoke.SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }

    public static bool GetIsVisible(nint hwnd)
    {
        HWND handle = new(hwnd);
        int style = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        return (style & WS_SYSMENU) != 0;
    }

    public static int SetIsVisible(nint hwnd, bool value)
    {
        HWND handle = new(hwnd);
        int style = PInvoke.GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        if (value)
        {
            style |= WS_SYSMENU;
        }
        else
        {
            style &= ~WS_SYSMENU;
        }
        return PInvoke.SetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }

    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_SYSMENU = 0x00080000;
}
