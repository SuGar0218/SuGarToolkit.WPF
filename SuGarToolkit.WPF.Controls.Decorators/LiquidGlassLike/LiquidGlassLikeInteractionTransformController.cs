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
                _pressTranform,
                _scaleTransform,
                _translateTransform
            ]
        };
        _scaleTransformResetXAnimation.EasingFunction = _easingFunction;
        _scaleTransformResetYAnimation.EasingFunction = _easingFunction;
        _translateTransformResetXAnimation.EasingFunction = _easingFunction;
        _translateTransformResetYAnimation.EasingFunction = _easingFunction;
        _pressTranformResetXAnimation.EasingFunction = _easingFunction;
        _pressTranformResetYAnimation.EasingFunction = _easingFunction;
        _pressTranformXAnimation.EasingFunction = _easingFunction;
        _pressTranformYAnimation.EasingFunction = _easingFunction;

        NameScope.SetNameScope(_target, new NameScope());
        _target.RegisterName(nameof(_scaleTransform), _scaleTransform);
        _target.RegisterName(nameof(_translateTransform), _translateTransform);
        _target.RegisterName(nameof(_pressTranform), _pressTranform);

        _resetStoryboard = new Storyboard
        {
            FillBehavior = FillBehavior.Stop,
            Children =
            [
                _scaleTransformResetXAnimation,
                _scaleTransformResetYAnimation,
                _translateTransformResetXAnimation,
                _translateTransformResetYAnimation,
                _pressTranformResetXAnimation,
                _pressTranformResetYAnimation
            ]
        };
        _resetStoryboard.Completed += OnResetStoryboardCompleted;
        Storyboard.SetTargetName(_scaleTransformResetXAnimation, nameof(_scaleTransform));
        Storyboard.SetTargetProperty(_scaleTransformResetXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
        Storyboard.SetTargetName(_scaleTransformResetYAnimation, nameof(_scaleTransform));
        Storyboard.SetTargetProperty(_scaleTransformResetYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));
        Storyboard.SetTargetName(_translateTransformResetXAnimation, nameof(_translateTransform));
        Storyboard.SetTargetProperty(_translateTransformResetXAnimation, new PropertyPath(TranslateTransform.XProperty));
        Storyboard.SetTargetName(_translateTransformResetYAnimation, nameof(_translateTransform));
        Storyboard.SetTargetProperty(_translateTransformResetYAnimation, new PropertyPath(TranslateTransform.YProperty));
        Storyboard.SetTargetName(_pressTranformResetXAnimation, nameof(_pressTranform));
        Storyboard.SetTargetProperty(_pressTranformResetXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
        Storyboard.SetTargetName(_pressTranformResetYAnimation, nameof(_pressTranform));
        Storyboard.SetTargetProperty(_pressTranformResetYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

        _pressStoryboard = new Storyboard
        {
            FillBehavior = FillBehavior.HoldEnd,
            Children =
            [
                _pressTranformXAnimation,
                _pressTranformYAnimation
            ]
        };
        Storyboard.SetTargetName(_pressTranformXAnimation, nameof(_pressTranform));
        Storyboard.SetTargetProperty(_pressTranformXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
        Storyboard.SetTargetName(_pressTranformYAnimation, nameof(_pressTranform));
        Storyboard.SetTargetProperty(_pressTranformYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

        ResetAnimationSeconds = 0.382;
        ExpandAnimationSeconds = 0.25;
    }

    private readonly FrameworkElement _target;
    private readonly TransformGroup _transformGroup;

    private readonly ScaleTransform _scaleTransform = new(1, 1);
    private readonly DoubleAnimation _scaleTransformResetXAnimation = new() { To = 1 };
    private readonly DoubleAnimation _scaleTransformResetYAnimation = new() { To = 1 };

    private readonly TranslateTransform _translateTransform = new(0, 0);
    private readonly DoubleAnimation _translateTransformResetXAnimation = new() { To = 0 };
    private readonly DoubleAnimation _translateTransformResetYAnimation = new() { To = 0 };

    private readonly ScaleTransform _pressTranform = new();
    private readonly DoubleAnimation _pressTranformXAnimation = new();
    private readonly DoubleAnimation _pressTranformYAnimation = new();
    private readonly DoubleAnimation _pressTranformResetXAnimation = new() { To = 1 };
    private readonly DoubleAnimation _pressTranformResetYAnimation = new() { To = 1 };

    private readonly IEasingFunction _easingFunction = new BackEase
    {
        EasingMode = EasingMode.EaseOut,
        Amplitude = 0.5
    };

    private readonly Storyboard _pressStoryboard;
    private readonly Storyboard _resetStoryboard;
    private readonly LiquidGlassLikeStretchCalculator _calculator = new();

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

    public double ExpandAnimationSeconds
    {
        get => field;
        set
        {
            field = value;
            OnExpandAnimationSecondsChanged();
        }
    }

    public void Begin()
    {
        _pressTranformXAnimation.To = _calculator.ExpandScale;
        _pressTranformYAnimation.To = _calculator.ExpandScale;
        _pressStoryboard.Begin(_target);
    }

    public void Reset()
    {
        _dragDelta = new Vector(0, 0);
        _resetStoryboard.Begin(_target);
    }

    private void OnTargetSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _calculator.OriginalSize = e.NewSize;
    }

    private void OnResetStoryboardCompleted(object? sender, EventArgs e)
    {
        _scaleTransform.ScaleX = 1;
        _scaleTransform.ScaleY = 1;
        _translateTransform.X = 0;
        _translateTransform.Y = 0;
    }

    private void OnResetAnimationSecondsChanged()
    {
        TimeSpan duration = TimeSpan.FromSeconds(ResetAnimationSeconds);
        _scaleTransformResetXAnimation.Duration = duration;
        _scaleTransformResetYAnimation.Duration = duration;
        _translateTransformResetXAnimation.Duration = duration;
        _translateTransformResetYAnimation.Duration = duration;
        _pressTranformResetXAnimation.Duration = duration;
        _pressTranformResetYAnimation.Duration = duration;
    }

    private void OnExpandAnimationSecondsChanged()
    {
        TimeSpan duration = TimeSpan.FromSeconds(ExpandAnimationSeconds);
        _pressTranformXAnimation.Duration = duration;
        _pressTranformYAnimation.Duration = duration;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnDragDeltaChanged()
    {
        _calculator.DragDelta = DragDelta;
        _calculator.Calculate();
        RefreshScaleTranform();
        RefreshTranslateTransform();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshScaleTranform()
    {
        _scaleTransform.ScaleX = 1 + _calculator.StretchX / _target.ActualWidth;
        _scaleTransform.ScaleY = 1 + _calculator.StretchY / _target.ActualHeight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshTranslateTransform()
    {
        _translateTransform.X = Math.Sign(DragDelta.X) * _calculator.OffsetX;
        _translateTransform.Y = Math.Sign(DragDelta.Y) * _calculator.OffsetY;
    }
}
