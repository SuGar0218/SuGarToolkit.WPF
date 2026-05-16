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
            _controller = new LiquidGlassLikeInteractionTransformController(this);
            RenderTransform = _controller.Transform;
            RenderTransformOrigin = new Point(0.5, 0.5);
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), handledEventsToo: true);
            AddHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove), handledEventsToo: true);
            AddHandler(MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), handledEventsToo: true);
        }

        private readonly LiquidGlassLikeInteractionTransformController _controller;

        private bool _isMouseDown;
        private Point _mouseDownPosition;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            _mouseDownPosition = e.GetPosition(this);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;

            Vector dragDelta = e.GetPosition(this) - _mouseDownPosition;
            if (Math.Abs(dragDelta.X) <= double.Epsilon && Math.Abs(dragDelta.Y) <= double.Epsilon)
                return;

            CaptureMouse();
            _controller.DragDelta = dragDelta;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isMouseDown)
                return;

            if (IsMouseCaptured)
                ReleaseMouseCapture();

            _isMouseDown = false;
            _controller.Reset();
        }
    }
}
