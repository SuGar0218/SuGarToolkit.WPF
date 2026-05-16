using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SuGarToolkit.WPF.Controls.Decorators;

public class LiquidGlassLikeInteractionTransformController
{
    public LiquidGlassLikeInteractionTransformController(FrameworkElement target)
    {
        _target = target;
        _target.SizeChanged += OnTargetSizeChanged;
        _transformGroup = new TransformGroup
        {
            Children =
            [
                _scaleTransform,
                _translateTransform
            ]
        };
        _scaleTransformResetXAnimation.EasingFunction = _easingFunction;
        _scaleTransformResetYAnimation.EasingFunction = _easingFunction;
        _translateTransformResetXAnimation.EasingFunction = _easingFunction;
        _translateTransformResetYAnimation.EasingFunction = _easingFunction;
        _resetStoryboard.Children =
        [
            _scaleTransformResetXAnimation,
            _scaleTransformResetYAnimation,
            _translateTransformResetXAnimation,
            _translateTransformResetYAnimation
        ];
        _resetStoryboard.FillBehavior = FillBehavior.Stop;
        _resetStoryboard.Completed += OnResetStoryboardCompleted;
        NameScope.SetNameScope(_target, new NameScope());
        _target.RegisterName(nameof(_scaleTransform), _scaleTransform);
        _target.RegisterName(nameof(_translateTransform), _translateTransform);
        Storyboard.SetTargetName(_scaleTransformResetXAnimation, nameof(_scaleTransform));
        Storyboard.SetTargetProperty(_scaleTransformResetXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
        Storyboard.SetTargetName(_scaleTransformResetYAnimation, nameof(_scaleTransform));
        Storyboard.SetTargetProperty(_scaleTransformResetYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));
        Storyboard.SetTargetName(_translateTransformResetXAnimation, nameof(_translateTransform));
        Storyboard.SetTargetProperty(_translateTransformResetXAnimation, new PropertyPath(TranslateTransform.XProperty));
        Storyboard.SetTargetName(_translateTransformResetYAnimation, nameof(_translateTransform));
        Storyboard.SetTargetProperty(_translateTransformResetYAnimation, new PropertyPath(TranslateTransform.YProperty));
        ResetAnimationSeconds = 0.382;
    }

    private readonly FrameworkElement _target;
    private readonly TransformGroup _transformGroup;
    private readonly ScaleTransform _scaleTransform = new ScaleTransform(1, 1);
    private readonly DoubleAnimation _scaleTransformResetXAnimation = new DoubleAnimation(1, default);
    private readonly DoubleAnimation _scaleTransformResetYAnimation = new DoubleAnimation(1, default);
    private readonly TranslateTransform _translateTransform = new TranslateTransform(0, 0);
    private readonly DoubleAnimation _translateTransformResetXAnimation = new DoubleAnimation(0, default);
    private readonly DoubleAnimation _translateTransformResetYAnimation = new DoubleAnimation(0, default);
    private readonly IEasingFunction _easingFunction = new BackEase
    {
        EasingMode = EasingMode.EaseOut,
        Amplitude = 0.5
    };
    private readonly Storyboard _resetStoryboard = new();
    private readonly LiquidGlassLikeStretchCalculator _stretchCalculator = new();

    public Transform Transform => _transformGroup;

    private Vector _dragDelta;

    public Vector DragDelta
    {
        get => _dragDelta;
        set
        {
            _dragDelta = value;
            OnDragDeltaChanged();
        }
    }

    public double ResetAnimationSeconds
    {
        get => field;
        set
        {
            field = value;
            OnResetAnimationSecondsChanged();
        }
    }

    public void Reset()
    {
        if (DragDelta.X == 0 && DragDelta.Y == 0)
            return;

        _dragDelta = new Vector(0, 0);
        _resetStoryboard.Begin(_target);
    }

    private void OnTargetSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _stretchCalculator.OriginalSize = e.NewSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnResetStoryboardCompleted(object? sender, EventArgs e)
    {
        _scaleTransform.ScaleX = 1;
        _scaleTransform.ScaleY = 1;
        _translateTransform.X = 0;
        _translateTransform.Y = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnResetAnimationSecondsChanged()
    {
        TimeSpan duration = TimeSpan.FromSeconds(ResetAnimationSeconds);
        _scaleTransformResetXAnimation.Duration = duration;
        _scaleTransformResetYAnimation.Duration = duration;
        _translateTransformResetXAnimation.Duration = duration;
        _translateTransformResetYAnimation.Duration = duration;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnDragDeltaChanged()
    {
        _stretchCalculator.DragDelta = DragDelta;
        _stretchCalculator.Calculate();
        RefreshScaleTranform();
        RefreshTranslateTransform();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshScaleTranform()
    {
        _scaleTransform.ScaleX = 1 + _stretchCalculator.StretchX / _target.ActualWidth;
        _scaleTransform.ScaleY = 1 + _stretchCalculator.StretchY / _target.ActualHeight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshTranslateTransform()
    {
        _translateTransform.X = Math.Sign(DragDelta.X) * _stretchCalculator.OffsetX;
        _translateTransform.Y = Math.Sign(DragDelta.Y) * _stretchCalculator.OffsetY;
    }
}
