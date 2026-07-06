using System.Windows;

namespace SuGarToolkit.WPF.Extensions.WebView2;

public class WebView2SelectionChangedRoutedEventArgs : RoutedEventArgs
{
    public WebView2SelectionChangedRoutedEventArgs(string selectedText, RoutedEvent routedEvent) : base(routedEvent)
    {
        SelectedText = selectedText;
    }

    public string SelectedText { get; set; }
}
