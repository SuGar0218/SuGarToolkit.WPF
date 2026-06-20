using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
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
        NameScope.SetNameScope(this, new NameScope());
        RegisterName(nameof(_popupClip), _popupClip);
        Storyboard.SetTargetName(_popupExpandAnimation, nameof(_popupClip));
        Storyboard.SetTargetProperty(_popupExpandAnimation, new PropertyPath(RectangleGeometry.RectProperty));
    }

    private Popup? _popup;
    private Border? _dropDownBorder;
    private FrameworkElement? _popupContentRoot;
    private readonly Storyboard _popupExpandStoryboard;
    private readonly RectAnimation _popupExpandAnimation;
    private readonly RectangleGeometry _popupClip;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _popup = GetTemplateChild("PART_Popup") as Popup;
        _popupContentRoot = GetTemplateChild("shadow") as FrameworkElement;
        _dropDownBorder = GetTemplateChild("dropDownBorder") as Border;
        _popupExpandStoryboard.Completed += OnPopupExpandStoryboardCompleted;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
        {
            Matrix transformToDevice = hwndSource.CompositionTarget.TransformToDevice;
            _dpiScaleX = transformToDevice.M11;
            _dpiScaleY = transformToDevice.M22;
        }
    }

    private void OnPopupExpandStoryboardCompleted(object? sender, EventArgs e)
    {
        if (_popupContentRoot != null)
        {
            _popupContentRoot.Clip = null;
        }
    }

    protected override void OnDropDownOpened(EventArgs e)
    {
        base.OnDropDownOpened(e);
        if (_popup is null)
            return;

        Visual? popupRootVisual = _dropDownBorder is null ? null : GetVisualTreeRoot(_dropDownBorder) as Visual;
        Visual? selectedVisual = SelectedIndex == -1 ? null : ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as Visual;
        Point selectedVisualPosition = selectedVisual?.TransformToVisual(popupRootVisual).Transform(ZeroPoint) ?? EmptyPoint;
        Point selfScreenPosition = PointToScreen(ZeroPoint);
        selfScreenPosition = ToDipPoint(selfScreenPosition);
        if (selectedVisual != null)
        {
            _popup.HorizontalOffset = selfScreenPosition.X;
            _popup.VerticalOffset = selfScreenPosition.Y - selectedVisualPosition.Y;
        }
        else
        {
            _popup.HorizontalOffset = selfScreenPosition.X;
            _popup.VerticalOffset = selfScreenPosition.Y;
        }
        if (_popupContentRoot != null)
        {
            if (_popupContentRoot.IsLoaded)
            {
                PrepareAnimation();
            }
            else
            {
                _popupContentRoot.Loaded += OnPopupContentRootLoaded;
            }
            void OnPopupContentRootLoaded(object sender, RoutedEventArgs args)
            {
                _popupContentRoot.Loaded -= OnPopupContentRootLoaded;
                PrepareAnimation();
            }
            void PrepareAnimation()
            {
                _popupContentRoot.Clip = _popupClip;
                _popupExpandAnimation.From = new Rect(
                    new Point(0, selectedVisualPosition.Y),
                    new Size(_popupContentRoot.RenderSize.Width, ActualHeight));
                _popupExpandAnimation.To = new Rect(
                    ZeroPoint,
                    _popupContentRoot.RenderSize);
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

    private Point ToDipPoint(Point point) => new Point
    {
        X = point.X / _dpiScaleX,
        Y = point.Y / _dpiScaleY
    };

    private Point ToPixelPoint(Point point) => new Point
    {
        X = point.X * _dpiScaleX,
        Y = point.Y * _dpiScaleY
    };

    private static readonly Point ZeroPoint = new Point(0, 0);
    private static readonly Point EmptyPoint = new Point(-1, -1);
}
