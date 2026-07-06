using Microsoft.Web.WebView2.Wpf;

using System.Runtime.CompilerServices;
using System.Windows;

namespace SuGarToolkit.WPF.Extensions.WebView2;

public class WebView2SelectionChangedListener : DependencyObject
{
    public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
        "SelectionChanged",
        RoutingStrategy.Bubble,
        typeof(WebView2SelectionChangedRoutedEventHandler),
        typeof(WebView2SelectionChangedListener)
    );

    public static void AddSelectionChangedHandler(UIElement target, WebView2SelectionChangedRoutedEventHandler handler)
    {
        if (target is IWebView2)
        {
            target.AddHandler(SelectionChangedEvent, handler);
        }
    }

    public static void RemoveSelectionChangedHandler(UIElement target, WebView2SelectionChangedRoutedEventHandler handler)
    {
        if (target is IWebView2)
        {
            target.RemoveHandler(SelectionChangedEvent, handler);
        }
    }

    public static string? GetSelectedText(IWebView2 target) => (string?)(target as UIElement)?.GetValue(SelectedTextProperty);
    private static void SetSelectedText(IWebView2 target, string? value) => (target as UIElement)?.SetValue(SelectedTextProperty, value);

    public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.RegisterAttached(
        "SelectedText",
        typeof(string),
        typeof(WebView2SelectionChangedListener),
        new PropertyMetadata(default(string))
    );

    public static bool GetIsEnabled(IWebView2 target) => (bool)((target as UIElement)?.GetValue(IsEnabledProperty) ?? false);
    public static void SetIsEnabled(IWebView2 target, bool value) => (target as UIElement)?.SetValue(IsEnabledProperty, value);

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(WebView2SelectionChangedListener),
        new PropertyMetadata(default(bool), OnIsEnabledChanged)
    );

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IWebView2 webView2 = (IWebView2)d;
        if (!_listeners.TryGetValue(webView2, out WebView2SelectionChangedListenerCore? listener))
        {
            listener = new WebView2SelectionChangedListenerCore(webView2);
            listener.SelectionChanged += OnListenerSelectionChanged;
            _listeners.Add(webView2, listener);
        }
        if (e.NewValue is true)
        {
            listener!.Enable();
        }
        else
        {
            listener!.Disable();
        }
    }

    private static void OnListenerSelectionChanged(object? sender, WebView2SelectionChangedEventArgs e)
    {
        WebView2SelectionChangedListenerCore listenerCore = (WebView2SelectionChangedListenerCore)sender!;
        SetSelectedText(listenerCore.WebView2, e.SelectedText);
        (listenerCore.WebView2 as UIElement)?.RaiseEvent(new WebView2SelectionChangedRoutedEventArgs(e.SelectedText, SelectionChangedEvent));
    }

    private static readonly ConditionalWeakTable<IWebView2, WebView2SelectionChangedListenerCore> _listeners = new ConditionalWeakTable<IWebView2, WebView2SelectionChangedListenerCore>();
}
