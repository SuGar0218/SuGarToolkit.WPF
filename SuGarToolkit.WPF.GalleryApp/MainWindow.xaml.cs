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
        TextBox textBox = (TextBox) sender;
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
}