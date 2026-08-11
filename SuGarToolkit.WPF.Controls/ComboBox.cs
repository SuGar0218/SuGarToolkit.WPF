using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SuGarToolkit.WPF.Controls;

public class ComboBox : System.Windows.Controls.ComboBox
{
    static ComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
    }

    public ComboBox()
    {
        Loaded += OnLoaded;
        _popupExpandAnimation = new RectAnimation
        {
            Duration = TimeSpan.FromSeconds(0.382),
            EasingFunction = new PowerEase
            {
                Power = 6,
                EasingMode = EasingMode.EaseOut
            }
        };
        _popupExpandStoryboard = new Storyboard
        {
            Children = [_popupExpandAnimation]
        };
        _popupClip = new RectangleGeometry();
        Storyboard.SetTargetName(_popupExpandAnimation, nameof(_popupClip));
        Storyboard.SetTargetProperty(_popupExpandAnimation, new PropertyPath(RectangleGeometry.RectProperty));
    }

    private Popup? PART_Popup;
    private readonly Storyboard _popupExpandStoryboard;
    private readonly RectAnimation _popupExpandAnimation;
    private readonly RectangleGeometry _popupClip;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_Popup = GetTemplateChild("PART_Popup") as Popup;
        _popupExpandStoryboard.Completed += OnPopupExpandStoryboardCompleted;

        if (NameScope.GetNameScope(this) is null)
        {
            NameScope.SetNameScope(this, new NameScope());
        }
        RegisterName(nameof(_popupClip), _popupClip);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DpiScale dpiScale = VisualTreeHelper.GetDpi(this);
        _dpiScaleX = dpiScale.DpiScaleX;
        _dpiScaleY = dpiScale.DpiScaleY;
    }

    private void OnPopupExpandStoryboardCompleted(object? sender, EventArgs e)
    {
        PART_Popup?.Child?.Clip = null;
    }

    protected override void OnDropDownOpened(EventArgs e)
    {
        base.OnDropDownOpened(e);
        if (PART_Popup is null)
            return;

        Visual? popupRootVisual = PART_Popup.Child is null ? null : GetVisualTreeRoot(PART_Popup.Child) as Visual;
        Visual? selectedVisual = SelectedIndex == -1 ? null : ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as Visual;
        Point selectedVisualPosition = selectedVisual?.TransformToVisual(popupRootVisual).Transform(ZeroPoint) ?? EmptyPoint;
        Point selfScreenPosition = PointToScreen(ZeroPoint);
        selfScreenPosition = ToDipPoint(selfScreenPosition);
        if (selectedVisual != null)
        {
            PART_Popup.HorizontalOffset = selfScreenPosition.X;
            PART_Popup.VerticalOffset = selfScreenPosition.Y - selectedVisualPosition.Y;

            // If an UIElement inside popup is selected, move it to the center of ComboBox.
            if (SelectedIndex != -1 && selectedVisual is UIElement selectedElement)
            {
                double selectedItemHeight = selectedElement.RenderSize.Height;
                if (selectedElement is FrameworkElement frameworkElement)
                {
                    selectedItemHeight += frameworkElement.Margin.Top + frameworkElement.Margin.Bottom;
                }
                PART_Popup.VerticalOffset += (ActualHeight - selectedItemHeight) / 2;
            }
        }
        else
        {
            PART_Popup.HorizontalOffset = selfScreenPosition.X;
            PART_Popup.VerticalOffset = selfScreenPosition.Y;
        }
        if (PART_Popup.Child is FrameworkElement popupContentRoot)
        {
            if (popupContentRoot.IsLoaded)
            {
                PrepareAnimation();
            }
            else
            {
                popupContentRoot.Loaded += OnPopupContentRootLoaded;
            }
            void OnPopupContentRootLoaded(object sender, RoutedEventArgs args)
            {
                popupContentRoot.Loaded -= OnPopupContentRootLoaded;
                PrepareAnimation();
            }
            void PrepareAnimation()
            {
                popupContentRoot.Clip = _popupClip;
                _popupExpandAnimation.From = new Rect(
                    new Point(0, selectedVisualPosition.Y),
                    new Size(popupContentRoot.RenderSize.Width, ActualHeight));
                _popupExpandAnimation.To = new Rect(
                    ZeroPoint,
                    popupContentRoot.RenderSize);
                _popupExpandStoryboard.Begin(this);
            }
        }
    }

    private static DependencyObject GetVisualTreeRoot(DependencyObject obj)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(obj);
        while (parent != null)
        {
            obj = parent;
            parent = VisualTreeHelper.GetParent(obj);
        }
        return obj;
    }

    private double _dpiScaleX = 1.0;
    private double _dpiScaleY = 1.0;

    private Point ToDipPoint(Point point) => new()
    {
        X = point.X / _dpiScaleX,
        Y = point.Y / _dpiScaleY
    };

    private Point ToPixelPoint(Point point) => new()
    {
        X = point.X * _dpiScaleX,
        Y = point.Y * _dpiScaleY
    };

    private static readonly Point ZeroPoint = new(0, 0);
    private static readonly Point EmptyPoint = new(-1, -1);
}
