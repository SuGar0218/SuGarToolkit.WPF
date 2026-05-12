using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuGarToolkit.WPF.GalleryApp.Controls;

[TemplatePart(Name = nameof(PART_RootBorder), Type = typeof(Border))]
public class CapsuleButton : Button
{
    static CapsuleButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CapsuleButton), new FrameworkPropertyMetadata(typeof(CapsuleButton)));
    }

    private Border? PART_RootBorder;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_RootBorder = GetTemplateChild(nameof(PART_RootBorder)) as Border;
        if (PART_RootBorder != null)
        {
            PART_RootBorder.SizeChanged += OnRootBorderSizeChanged;
        }
    }

    private void OnRootBorderSizeChanged(object sender, SizeChangedEventArgs e)
    {
        PART_RootBorder?.CornerRadius = new CornerRadius(Math.Min(e.NewSize.Width, e.NewSize.Height) / 2);
    }
}
