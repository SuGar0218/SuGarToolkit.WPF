using System.Windows;
using System.Windows.Controls;

namespace SuGarToolkit.WPF.Controls.Layout;

public partial class HeaderBodyFooterView : ContentControl
{
    static HeaderBodyFooterView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderBodyFooterView), new FrameworkPropertyMetadata(typeof(HeaderBodyFooterView)));
    }
}
