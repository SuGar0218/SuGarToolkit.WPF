using CommunityToolkit.Mvvm.ComponentModel;

using SuGarToolkit.WPF.Controls.Windows;

using System.Windows;

namespace SuGarToolkit.WPF.GalleryApp.Views;

public partial class SampleDwmAttributesChromedWindow : Window
{
    public SampleDwmAttributesChromedWindow()
    {
        InitializeComponent();
        DataContext = _viewModel = new SampleDwmAttributeChromedWindowViewModel();
    }

    private readonly SampleDwmAttributeChromedWindowViewModel _viewModel;
}

internal partial class SampleDwmAttributeChromedWindowViewModel : ObservableObject
{
    public WindowSystemBackdrop[] SystemBackdrops { get; } = Enum.GetValues<WindowSystemBackdrop>();

    public WindowCornerRoundness[] CornerRoundnesses { get; } = Enum.GetValues<WindowCornerRoundness>();

    [ObservableProperty]
    public partial WindowSystemBackdrop SystemBackdrop { get; set; } = WindowSystemBackdrop.Mica;

    [ObservableProperty]
    public partial WindowCornerRoundness CornerRoundness { get; set; } = WindowCornerRoundness.Normal;
}
