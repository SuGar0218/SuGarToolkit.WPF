using SuGarToolkit.WPF.GalleryApp.Views;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SuGarToolkit.WPF.GalleryApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TextBox_TextReallyChanged(object sender, EventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Debug.WriteLine($"TextReallyChanged: {textBox.Text}");
    }

    private void TextBox_InputMethodEditingStart(object sender, EventArgs e)
    {
        Debug.WriteLine($"InputMethodEditingStart");
    }

    private void TextBox_InputMethodEditingComplete(object sender, EventArgs e)
    {
        Debug.WriteLine($"TextBox_InputMethodEditingComplete");
    }

    private void OnCapsuleButtonClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(this, "Try to drag move this button", "CapsuleButton");
    }

    private void OnShowOverflowWindowButtonClick(object sender, RoutedEventArgs e)
    {
        new SampleOverflowWindow().Show();
    }

    private void OnWebView2ExnteionsButtonClick(object sender, RoutedEventArgs e)
    {
        new Window
        {
            Width = 1024,
            Height = 768,
            Content = new WebView2ExtensionsDemoView()
        }
        .Show();
    }

    private void OnShowHeaderBodyFooterViewButtonClick(object sender, RoutedEventArgs e)
    {
        new Window
        {
            Width = 480,
            Height = 320,
            Content = new HeaderBodyFooterViewDemoView()
        }
        .Show();
    }

    private void OnDwmWindowAttribute1ButtonClick(object sender, RoutedEventArgs e)
    {
        new SampleDwmAttributesWindow().Show();
    }

    private void OnDwmWindowAttribute2ButtonClick(object sender, RoutedEventArgs e)
    {
        new SampleDwmAttributesChromedWindow().Show();
    }
}