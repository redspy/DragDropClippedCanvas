using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DragDropClippedCanvas.Tests.TestUtilities
{
    /// <summary>
    /// A mock UIElement implementation for testing purposes
    /// </summary>
    public class MockElement : FrameworkElement
    {
        public event EventHandler<MouseEventArgs> MouseMoveEvent;
        public event EventHandler<MouseButtonEventArgs> MouseLeftButtonDownEvent;
        public event EventHandler<MouseButtonEventArgs> MouseLeftButtonUpEvent;
        public event EventHandler<MouseEventArgs> MouseEnterEvent;
        public event EventHandler<MouseEventArgs> MouseLeaveEvent;
        public event EventHandler<MouseWheelEventArgs> MouseWheelEvent;

        private double _width;
        private double _height;

        public MockElement(double width = 100, double height = 100)
        {
            _width = width;
            _height = height;
            this.Width = width;
            this.Height = height;
        }

        public new double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                base.Width = value;
            }
        }

        public new double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                base.Height = value;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(_width, _height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(_width, _height);
        }

        // Methods to simulate mouse events
        public void RaiseMouseMove(MouseEventArgs args)
        {
            MouseMoveEvent?.Invoke(this, args);
        }

        public void RaiseMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            MouseLeftButtonDownEvent?.Invoke(this, args);
        }

        public void RaiseMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            MouseLeftButtonUpEvent?.Invoke(this, args);
        }

        public void RaiseMouseEnter(MouseEventArgs args)
        {
            MouseEnterEvent?.Invoke(this, args);
        }

        public void RaiseMouseLeave(MouseEventArgs args)
        {
            MouseLeaveEvent?.Invoke(this, args);
        }

        public void RaiseMouseWheel(MouseWheelEventArgs args)
        {
            MouseWheelEvent?.Invoke(this, args);
        }

        // Handle mouse capture in tests using new instead of override
        private bool _isMouseCaptured = false;

        public new bool CaptureMouse()
        {
            _isMouseCaptured = true;
            return true;
        }

        public new void ReleaseMouseCapture()
        {
            _isMouseCaptured = false;
        }

        public bool IsMouseCaptured => _isMouseCaptured;
    }

    // Extend MouseEventArgs for testing
    public class MockMouseEventArgs : MouseEventArgs
    {
        public MockMouseEventArgs(Point position)
            : base(Mouse.PrimaryDevice, 0)
        {
            Position = position;
        }

        public Point Position { get; }

        public Point GetPosition(IInputElement relativeTo)
        {
            return Position;
        }
    }

    // Extend MouseButtonEventArgs for testing
    public class MockMouseButtonEventArgs : MouseButtonEventArgs
    {
        public MockMouseButtonEventArgs(Point position)
            : base(Mouse.PrimaryDevice, 0, System.Windows.Input.MouseButton.Left)
        {
            Position = position;
        }

        public Point Position { get; }

        public Point GetPosition(IInputElement relativeTo)
        {
            return Position;
        }
    }

    // Extend MouseWheelEventArgs for testing
    public class MockMouseWheelEventArgs : MouseWheelEventArgs
    {
        public MockMouseWheelEventArgs(Point position, int delta)
            : base(Mouse.PrimaryDevice, 0, delta)
        {
            Position = position;
        }

        public Point Position { get; }

        public Point GetPosition(IInputElement relativeTo)
        {
            return Position;
        }
    }
}