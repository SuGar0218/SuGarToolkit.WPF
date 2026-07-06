namespace SuGarToolkit.WPF.Extensions.WebView2;

public class WebView2SelectionChangedEventArgs : EventArgs
{
    public WebView2SelectionChangedEventArgs(string selectedText)
    {
        SelectedText = selectedText;
    }

    public string SelectedText { get; set; }
}
