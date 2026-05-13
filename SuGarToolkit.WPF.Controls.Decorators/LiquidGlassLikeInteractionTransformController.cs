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
        _transformGroup = new TransformGroup
        {
            Children =
            [
                _scaleTransform,
                _translateTransform
            ]
        };
        _scaleTransfromResetXAnimation.EasingFunction = _easingFunction;
        _scaleTransfromResetYAnimation.EasingFunction = _easingFunction;
        _translateTransformResetXAnimation.EasingFunction = _easingFunction;
        _translateTransformResetYAnimation.EasingFunction = _easingFunction;
        _resetStoryboard.Children =
        [
            _scaleTransfromResetXAnimation,
            _scaleTransfromResetYAnimation,
            _translateTransformResetXAnimation,
            _translateTransformResetYAnimation
        ];
        _resetStoryboard.FillBehavior = FillBehavior.Stop;
        _resetStoryboard.Completed += OnResetStoryboardCompleted;
        NameScope.SetNameScope(_target, new NameScope());
        _target.RegisterName(nameof(_scaleTransform), _scaleTransform);
        _target.RegisterName(nameof(_translateTransform), _translateTransform);
        Storyboard.SetTargetName(_scaleTransfromResetXAnimation, nameof(_scaleTransform));
        Storyboard.SetTargetProperty(_scaleTransfromResetXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
        Storyboard.SetTargetName(_scaleTransfromResetYAnimation, nameof(_scaleTransform));
        Storyboard.SetTargetProperty(_scaleTransfromResetYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));
        Storyboard.SetTargetName(_translateTransformResetXAnimation, nameof(_translateTransform));
        Storyboard.SetTargetProperty(_translateTransformResetXAnimation, new PropertyPath(TranslateTransform.XProperty));
        Storyboard.SetTargetName(_translateTransformResetYAnimation, nameof(_translateTransform));
        Storyboard.SetTargetProperty(_translateTransformResetYAnimation, new PropertyPath(TranslateTransform.YProperty));
        ResetAnimationSeconds = 0.382;
    }

    private readonly FrameworkElement _target;
    private readonly TransformGroup _transformGroup;
    private readonly ScaleTransform _scaleTransform = new ScaleTransform(1, 1);
    private readonly DoubleAnimation _scaleTransfromResetXAnimation = new DoubleAnimation(1, default);
    private readonly DoubleAnimation _scaleTransfromResetYAnimation = new DoubleAnimation(1, default);
    private readonly TranslateTransform _translateTransform = new TranslateTransform(0, 0);
    private readonly DoubleAnimation _translateTransformResetXAnimation = new DoubleAnimation(0, default);
    private readonly DoubleAnimation _translateTransformResetYAnimation = new DoubleAnimation(0, default);
    private readonly IEasingFunction _easingFunction = new BackEase
    {
        EasingMode = EasingMode.EaseOut,
        Amplitude = 0.5
    };

    private readonly Storyboard _resetStoryboard = new();

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

    private void OnResetStoryboardCompleted(object? sender, EventArgs e)
    {
        _scaleTransform.ScaleX = 1;
        _scaleTransform.ScaleY = 1;
        _translateTransform.X = 0;
        _translateTransform.Y = 0;
    }

    private void OnResetAnimationSecondsChanged()
    {
        _scaleTransfromResetXAnimation.Duration = TimeSpan.FromSeconds(ResetAnimationSeconds);
        _scaleTransfromResetYAnimation.Duration = TimeSpan.FromSeconds(ResetAnimationSeconds);
        _translateTransformResetXAnimation.Duration = TimeSpan.FromSeconds(ResetAnimationSeconds);
        _translateTransformResetYAnimation.Duration = TimeSpan.FromSeconds(ResetAnimationSeconds);
    }

    private void OnDragDeltaChanged()
    {
        RefreshScaleTranform();
        RefreshTranslateTransform();
    }

    private static readonly double k = 618;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshScaleTranform()
    {
        double absDeltaX = Math.Abs(DragDelta.X);
        double absDeltaY = Math.Abs(DragDelta.Y);
        _scaleTransform.ScaleX = 2 - k / (absDeltaX + k) - 0.5 * absDeltaY / (absDeltaY + k);
        _scaleTransform.ScaleY = 2 - k / (absDeltaY + k) - 0.5 * absDeltaX / (absDeltaX + k);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshTranslateTransform()
    {
        double absDeltaX = Math.Abs(DragDelta.X);
        double absDeltaY = Math.Abs(DragDelta.Y);
        _translateTransform.X = (_target.ActualWidth * 0.5 + 16) * absDeltaX / (absDeltaX + k);
        _translateTransform.Y = (_target.ActualHeight * 0.5 + 16) * absDeltaY / (absDeltaY + k);
        if (DragDelta.X < 0)
        {
            _translateTransform.X = -_translateTransform.X;
        }
        if (DragDelta.Y < 0)
        {
            _translateTransform.Y = -_translateTransform.Y;
        }
    }
}
