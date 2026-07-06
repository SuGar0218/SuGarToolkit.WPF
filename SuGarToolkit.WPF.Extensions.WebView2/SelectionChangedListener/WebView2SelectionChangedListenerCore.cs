using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace SuGarToolkit.WPF.Extensions.WebView2;

public class WebView2SelectionChangedListenerCore
{
    public WebView2SelectionChangedListenerCore(IWebView2 webView2)
    {
        WebView2 = webView2;
    }

    public event EventHandler<WebView2SelectionChangedEventArgs>? SelectionChanged;

    public IWebView2 WebView2 { get; }

    private string? _scriptId;

    public void Enable()
    {
        if (WebView2.CoreWebView2 is null)
        {
            WebView2.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
            return;
        }
        _ = AttachAfterInitialized();
    }

    public void Disable()
    {
        if (WebView2.CoreWebView2 is null)
            return;

        _ = DetachAfterInitialized();
    }

    private void OnCoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        WebView2.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;
        _ = AttachAfterInitialized();
    }

    private async Task AttachAfterInitialized()
    {
        if (_scriptId != null)
            return;

        WebView2.WebMessageReceived += OnWebMessageReceived;
        Task jsTask = WebView2.ExecuteScriptAsync(js_AddSelectionChangedEventListener);
        _scriptId = await WebView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js_AddSelectionChangedEventListener);
        await jsTask;
    }

    private async Task DetachAfterInitialized()
    {
        if (_scriptId is null)
            return;

        WebView2.WebMessageReceived -= OnWebMessageReceived;
        Task jsTask = WebView2.ExecuteScriptAsync(js_RemoveSelectionChangedEventListener);
        WebView2.CoreWebView2.RemoveScriptToExecuteOnDocumentCreated(_scriptId);
        _scriptId = null;
        await jsTask;
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        Dictionary<string, string>? message = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(e.WebMessageAsJson);
        if (message is null)
            return;

        if (!message.TryGetValue("type", out string? type))
            return;

        if (type != "SelectionChanged")
            return;

        if (!message.TryGetValue("text", out string? text))
            return;

        text = text?
            .Replace(@"\r\n", Environment.NewLine)
            .Replace(@"\r", Environment.NewLine)
            .Replace(@"\n", Environment.NewLine) ?? string.Empty;

        SelectionChanged?.Invoke(this, new WebView2SelectionChangedEventArgs(text));
    }

    private const string js_AddSelectionChangedEventListener = @"
            if (typeof window.onSelectionChange === 'undefined') {
                window.onSelectionChange = function() {
                    const text = window.getSelection().toString();
                    window.chrome.webview.postMessage({ type: 'SelectionChanged', text: text });
                };
            }

            document.addEventListener('selectionchange', onSelectionChange);
        ";

    private const string js_RemoveSelectionChangedEventListener = @"
            document.removeEventListener('selectionchange', onSelectionChange);
        ";
}
