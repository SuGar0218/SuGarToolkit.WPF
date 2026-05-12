using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuGarToolkit.WPF.Controls.Decorators
{
    public class LiquidGlassLikeInteractionDecorator : Decorator
    {
        static LiquidGlassLikeInteractionDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiquidGlassLikeInteractionDecorator), new FrameworkPropertyMetadata(typeof(LiquidGlassLikeInteractionDecorator)));
        }

        public LiquidGlassLikeInteractionDecorator()
        {
            _controller = new LiquidGlassLikeInteractionTransformController(this)
            {
                ResetAnimationSeconds = 0.382
            };
            _controller.TransformOriginChanged += OnControllerTransformOriginChanged;
            RenderTransform = _controller.Transform;
            RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private readonly LiquidGlassLikeInteractionTransformController _controller;

        private void OnControllerTransformOriginChanged(object? sender, EventArgs e)
        {
            RenderTransformOrigin = _controller.TransformOrigin;
        }

        private bool _isMouseDown;
        private Point _mouseDownPosition;

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            _isMouseDown = true;
            _mouseDownPosition = e.GetPosition(Window.GetWindow(this));
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (!_isMouseDown)
                return;

            _controller.DragDelta = e.GetPosition(Window.GetWindow(this)) - _mouseDownPosition;
            if (Math.Abs(_controller.DragDelta.X) > double.Epsilon || Math.Abs(_controller.DragDelta.Y) > double.Epsilon)
            {
                CaptureMouse();
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (!_isMouseDown)
                return;

            if (IsMouseCaptured)
                ReleaseMouseCapture();

            _isMouseDown = false;
            _controller.Reset();
        }
    }
}
