using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SuGarToolkit.WPF.Helpers;

public partial class TextBoxInputMonitor : IDisposable
{
    public TextBoxInputMonitor(TextBoxBase textBox)
    {
        TargetTextBox = textBox;
        TargetTextBox.TextChanged += OnTextBoxTextChanged;
        TextCompositionManager.AddPreviewTextInputStartHandler(TargetTextBox, OnTextCompositionStart);
        TextCompositionManager.AddPreviewTextInputHandler(TargetTextBox, OnTextCompositionComplete);
    }

    private void PrivateDispose()
    {
        TargetTextBox.TextChanged -= OnTextBoxTextChanged;
        TextCompositionManager.RemovePreviewTextInputStartHandler(TargetTextBox, OnTextCompositionStart);
        TextCompositionManager.RemovePreviewTextInputHandler(TargetTextBox, OnTextCompositionComplete);
    }

    #region 附加事件 TextReallyChanged

    public static void AddTextReallyChangedHandler(DependencyObject dependencyObject, RoutedEventHandler handler) => (dependencyObject as UIElement)?.AddHandler(TextReallyChangedEvent, handler);
    public static void RemoveTextReallyChangedHandler(DependencyObject dependencyObject, RoutedEventHandler handler) => (dependencyObject as UIElement)?.RemoveHandler(TextReallyChangedEvent, handler);

    public static readonly RoutedEvent TextReallyChangedEvent = EventManager.RegisterRoutedEvent(
        "TextReallyChanged",
        RoutingStrategy.Direct,
        typeof(RoutedEventHandler),
        typeof(TextBoxInputMonitor)
    );

    #endregion

    #region 附加事件 InputMethodEditingStart

    public static void AddInputMethodEditingStartHandler(DependencyObject dependencyObject, RoutedEventHandler handler) => (dependencyObject as UIElement)?.AddHandler(InputMethodEditingStartEvent, handler);
    public static void RemoveInputMethodEditingStartHandler(DependencyObject dependencyObject, RoutedEventHandler handler) => (dependencyObject as UIElement)?.RemoveHandler(InputMethodEditingStartEvent, handler);

    public static readonly RoutedEvent InputMethodEditingStartEvent = EventManager.RegisterRoutedEvent(
        "InputMethodEditingStart",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(TextBoxInputMonitor)
    );

    #endregion

    #region 附加事件 InputMethodEditingComplete

    public static void AddInputMethodEditingCompleteHandler(DependencyObject dependencyObject, RoutedEventHandler handler) => (dependencyObject as UIElement)?.AddHandler(InputMethodEditingCompleteEvent, handler);
    public static void RemoveInputMethodEditingCompleteHandler(DependencyObject dependencyObject, RoutedEventHandler handler) => (dependencyObject as UIElement)?.RemoveHandler(InputMethodEditingCompleteEvent, handler);

    public static readonly RoutedEvent InputMethodEditingCompleteEvent = EventManager.RegisterRoutedEvent(
        "InputMethodEditingComplete",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(TextBoxInputMonitor)
    );

    #endregion

    #region 附加属性 IsInputMethod

    public static bool GetIsInputMethodEditing(TextBoxBase target) => (bool) target.GetValue(IsInputMethodEditingProperty);
    private static void SetIsInputMethodEditing(TextBoxBase target, bool value) => target.SetValue(IsInputMethodEditingProperty, value);

    public static readonly DependencyProperty IsInputMethodEditingProperty = DependencyProperty.RegisterAttached(
        "IsInputMethodEditing",
        typeof(bool),
        typeof(TextBoxBase),
        new PropertyMetadata(default(bool))
    );

    #endregion

    public event EventHandler? TextReallyChanged;
    public event EventHandler? InputMethodEditingStart;
    public event EventHandler? InputMethodEditingComplete;

    public TextBoxBase TargetTextBox { get; }

    private bool _isTextComposing;

    private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isTextComposing)
            return;

        TextReallyChanged?.Invoke(this, EventArgs.Empty);
        TargetTextBox.RaiseEvent(new RoutedEventArgs(TextReallyChangedEvent));
    }

    private void OnTextCompositionStart(object sender, TextCompositionEventArgs e)
    {
        _isTextComposing = true;
        SetIsInputMethodEditing(TargetTextBox, true);
        InputMethodEditingStart?.Invoke(this, EventArgs.Empty);
        TargetTextBox.RaiseEvent(new RoutedEventArgs(InputMethodEditingStartEvent));
    }

    private void OnTextCompositionComplete(object sender, TextCompositionEventArgs e)
    {
        _isTextComposing = false;
        SetIsInputMethodEditing(TargetTextBox, false);
        InputMethodEditingComplete?.Invoke(this, EventArgs.Empty);
        TargetTextBox.RaiseEvent(new RoutedEventArgs(InputMethodEditingCompleteEvent));
    }

    #region IDisposable

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                PrivateDispose();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~TextBoxInputMonitor()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
