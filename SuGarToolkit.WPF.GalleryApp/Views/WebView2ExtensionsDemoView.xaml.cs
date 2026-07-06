using SuGarToolkit.WPF.Extensions.WebView2;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SuGarToolkit.WPF.GalleryApp.Views;

public partial class WebView2ExtensionsDemoView : UserControl
{
    public WebView2ExtensionsDemoView()
    {
        InitializeComponent();
    }

    private void OnWebViewSelectionChanged(object sender, WebView2SelectionChangedRoutedEventArgs e)
    {
        Debug.WriteLine(e.SelectedText);
    }

    private void OnSelectAllButtonClick(object sender, RoutedEventArgs e)
    {
        webView.SelectAll();
    }

    private void OnUnselectAllButtonClick(object sender, RoutedEventArgs e)
    {
        webView.ClearSelection();
    }
}
