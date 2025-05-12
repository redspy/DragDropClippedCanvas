using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DragDropClippedCanvas.Controls
{
    /// <summary>
    /// A UserControl with a draggable canvas that supports drag and drop of elements
    /// </summary>
    public partial class DraggableCanvas : UserControl
    {
        // Public events
        public event EventHandler<ElementDroppedEventArgs> ElementDropped;
        public event EventHandler<ElementDraggedEventArgs> ElementDragged;
        public event EventHandler<ElementResizedEventArgs> ElementResized;

        // Private fields
        private UIElement _draggedElement;
        private Point _dragStartPoint;
        private bool _isDragging;
        private UIElement _hoveredElement;

        #region Dependency Properties

        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(DraggableCanvas), new PropertyMetadata(1024.0));

        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(DraggableCanvas), new PropertyMetadata(768.0));

        public static readonly DependencyProperty AllowedElementTypesProperty =
            DependencyProperty.Register("AllowedElementTypes", typeof(List<Type>), typeof(DraggableCanvas), new PropertyMetadata(null));

        public static readonly DependencyProperty ClipElementsToCanvasProperty =
            DependencyProperty.Register("ClipElementsToCanvas", typeof(bool), typeof(DraggableCanvas), new PropertyMetadata(true));

        public static readonly DependencyProperty MaintainSquareAspectRatioProperty =
            DependencyProperty.Register("MaintainSquareAspectRatio", typeof(bool), typeof(DraggableCanvas), new PropertyMetadata(true));

        #endregion

        #region Public Properties

        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        public List<Type> AllowedElementTypes
        {
            get { return (List<Type>)GetValue(AllowedElementTypesProperty); }
            set { SetValue(AllowedElementTypesProperty, value); }
        }

        public bool ClipElementsToCanvas
        {
            get { return (bool)GetValue(ClipElementsToCanvasProperty); }
            set { SetValue(ClipElementsToCanvasProperty, value); }
        }

        public bool MaintainSquareAspectRatio
        {
            get { return (bool)GetValue(MaintainSquareAspectRatioProperty); }
            set { SetValue(MaintainSquareAspectRatioProperty, value); }
        }

        #endregion

        public DraggableCanvas()
        {
            InitializeComponent();

            // Initialize allowed element types if not set
            if (AllowedElementTypes == null)
            {
                AllowedElementTypes = new List<Type>
                {
                    typeof(Rectangle),
                    typeof(Ellipse),
                    typeof(TextBlock),
                    typeof(Button),
                    typeof(Image)
                };
            }

            // Set up mouse wheel event handler for the canvas
            MainCanvas.PreviewMouseWheel += MainCanvas_PreviewMouseWheel;
        }

        #region Canvas Event Handlers

        private void MainCanvas_Drop(object sender, DragEventArgs e)
        {
            // Handle external drops (e.g., from outside the canvas)
            if (e.Data.GetDataPresent("UIElement"))
            {
                var element = e.Data.GetData("UIElement") as UIElement;
                if (element != null && IsElementTypeAllowed(element.GetType()))
                {
                    Point dropPosition = e.GetPosition(MainCanvas);
                    AddElement(element, dropPosition);

                    // Raise the ElementDropped event
                    ElementDropped?.Invoke(this, new ElementDroppedEventArgs { Element = element, Position = dropPosition });
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Handle file drops (e.g., image files)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    try
                    {
                        // Create an image from the file
                        Image image = new Image();
                        image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(file));

                        Point dropPosition = e.GetPosition(MainCanvas);
                        AddElement(image, dropPosition);

                        // Raise the ElementDropped event
                        ElementDropped?.Invoke(this, new ElementDroppedEventArgs { Element = image, Position = dropPosition });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            e.Handled = true;
        }

        private void MainCanvas_DragOver(object sender, DragEventArgs e)
        {
            bool isValid = false;

            if (e.Data.GetDataPresent("UIElement"))
            {
                var element = e.Data.GetData("UIElement") as UIElement;
                isValid = element != null && IsElementTypeAllowed(element.GetType());
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Assuming files are allowed (for images)
                isValid = true;
            }

            e.Effects = isValid ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void MainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Check if there's a hovered element
            if (_hoveredElement != null && _hoveredElement is FrameworkElement frameworkElement)
            {
                // Get current width and height
                double currentWidth = frameworkElement.Width;
                double currentHeight = frameworkElement.Height;

                // Calculate new size based on wheel delta
                double delta = e.Delta > 0 ? 10 : -10;

                // Get current position
                double left = Canvas.GetLeft(_hoveredElement);
                double top = Canvas.GetTop(_hoveredElement);

                // Debug output for diagnostics
                // Console.WriteLine($"Before: Left={left}, Top={top}, Width={currentWidth}, Height={currentHeight}");

                // Calculate the center point of the element
                double centerX = left + (currentWidth / 2);
                double centerY = top + (currentHeight / 2);

                // If maintaining square aspect ratio, determine the size based on canvas constraints
                double newSize = currentWidth + delta;

                // Limit maximum size increase to +100 from original size
                if (frameworkElement.Tag is OriginalSize originalSize)
                {
                    newSize = Math.Min(originalSize.Width + 100, Math.Max(10, newSize));
                }

                // Always use the same value for width and height to maintain square aspect ratio
                double newWidth = newSize;
                double newHeight = newSize;

                // Calculate new position to maintain the center point
                double newLeft = centerX - (newWidth / 2);
                double newTop = centerY - (newHeight / 2);

                // Check boundaries and adjust position and size if needed
                AdjustSizeAndPositionForCanvas(ref newWidth, ref newHeight, ref newLeft, ref newTop, centerX, centerY);

                // Debug output for diagnostics
                // Console.WriteLine($"After: Left={newLeft}, Top={newTop}, Width={newWidth}, Height={newHeight}");

                // Update element size and position
                frameworkElement.Width = newWidth;
                frameworkElement.Height = newHeight;
                Canvas.SetLeft(_hoveredElement, newLeft);
                Canvas.SetTop(_hoveredElement, newTop);

                // Raise the ElementResized event
                ElementResized?.Invoke(this, new ElementResizedEventArgs
                {
                    Element = _hoveredElement,
                    Width = newWidth,
                    Height = newHeight,
                    Position = new Point(newLeft, newTop)
                });

                e.Handled = true;
            }
        }

        /// <summary>
        /// Adjusts the size and position of an element to ensure it stays within canvas boundaries
        /// while maintaining square aspect ratio
        /// </summary>
        private void AdjustSizeAndPositionForCanvas(ref double width, ref double height,
            ref double left, ref double top, double centerX, double centerY)
        {
            if (!ClipElementsToCanvas)
                return;

            // Safety check to ensure minimum size
            width = Math.Max(width, 10);
            height = Math.Max(height, 10);

            // Step 1: Check if we need to adjust position to keep the element within the canvas
            // Start with ensuring left position is valid
            if (left < 0)
            {
                left = 0;
            }

            // Ensure top position is valid
            if (top < 0)
            {
                top = 0;
            }

            // Step 2: Check if the element extends beyond the right or bottom edges
            bool rightEdgeViolation = (left + width) > CanvasWidth;
            bool bottomEdgeViolation = (top + height) > CanvasHeight;

            // Step 3: Calculate available space
            double availableWidth = rightEdgeViolation ?
                CanvasWidth - left : width;
            double availableHeight = bottomEdgeViolation ?
                CanvasHeight - top : height;

            // Step 4: Determine the maximum square size that fits, respecting the 10px minimum
            double maxSquareSize = Math.Max(10, Math.Min(availableWidth, availableHeight));

            // Step 5: Adjust size if needed
            if (width > maxSquareSize || height > maxSquareSize)
            {
                width = height = maxSquareSize;

                // Step 6: Recalculate position from center point to maintain centered scaling
                double adjustedLeft = centerX - (width / 2);
                double adjustedTop = centerY - (height / 2);

                // Step 7: Ensure adjusted position doesn't cause canvas overflow
                left = Math.Max(0, Math.Min(CanvasWidth - width, adjustedLeft));
                top = Math.Max(0, Math.Min(CanvasHeight - height, adjustedTop));
            }

            // Final boundary check
            if (left + width > CanvasWidth)
            {
                // If we're still over the edge, prioritize keeping the element within the canvas
                // by adjusting both position and size as needed
                if (width > CanvasWidth)
                {
                    // Element is wider than canvas, so set maximum size and position at left edge
                    width = height = Math.Min(width, CanvasWidth);
                    left = 0;
                }
                else
                {
                    // Move element back to stay within right boundary
                    left = CanvasWidth - width;
                }
            }

            if (top + height > CanvasHeight)
            {
                // Same logic for top/bottom
                if (height > CanvasHeight)
                {
                    height = width = Math.Min(height, CanvasHeight);
                    top = 0;
                }
                else
                {
                    top = CanvasHeight - height;
                }
            }
        }

        #endregion

        #region Element Dragging Methods

        /// <summary>
        /// Make an element draggable within the canvas
        /// </summary>
        public void MakeElementDraggable(UIElement element)
        {
            element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            element.MouseMove += Element_MouseMove;
            element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            element.MouseEnter += Element_MouseEnter;
            element.MouseLeave += Element_MouseLeave;

            // Store original size for size constraints
            if (element is FrameworkElement frameworkElement)
            {
                frameworkElement.Tag = new OriginalSize
                {
                    Width = frameworkElement.Width,
                    Height = frameworkElement.Height
                };
            }
        }

        private void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            _hoveredElement = sender as UIElement;
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_hoveredElement == sender)
            {
                _hoveredElement = null;
            }
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                _draggedElement = element;
                _dragStartPoint = e.GetPosition(MainCanvas);
                _isDragging = true;
                element.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                Point currentPoint = e.GetPosition(MainCanvas);
                double deltaX = currentPoint.X - _dragStartPoint.X;
                double deltaY = currentPoint.Y - _dragStartPoint.Y;

                double newLeft = Canvas.GetLeft(_draggedElement) + deltaX;
                double newTop = Canvas.GetTop(_draggedElement) + deltaY;

                if (ClipElementsToCanvas)
                {
                    // Get the width and height of the element
                    double width = 0;
                    double height = 0;

                    // Try to get the element's actual width/height
                    if (_draggedElement is FrameworkElement frameworkElement)
                    {
                        width = frameworkElement.ActualWidth > 0 ? frameworkElement.ActualWidth : frameworkElement.Width;
                        height = frameworkElement.ActualHeight > 0 ? frameworkElement.ActualHeight : frameworkElement.Height;
                    }

                    // Keep the element within the canvas boundaries
                    newLeft = Math.Max(0, Math.Min(CanvasWidth - width, newLeft));
                    newTop = Math.Max(0, Math.Min(CanvasHeight - height, newTop));
                }

                Canvas.SetLeft(_draggedElement, newLeft);
                Canvas.SetTop(_draggedElement, newTop);

                _dragStartPoint = currentPoint;

                // Raise the ElementDragged event
                ElementDragged?.Invoke(this, new ElementDraggedEventArgs
                {
                    Element = _draggedElement,
                    Position = new Point(newLeft, newTop),
                    Delta = new Vector(deltaX, deltaY)
                });

                e.Handled = true;
            }
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                _draggedElement.ReleaseMouseCapture();
                _isDragging = false;
                _draggedElement = null;
                e.Handled = true;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Add an element to the canvas at the specified position
        /// </summary>
        public void AddElement(UIElement element, Point position)
        {
            if (!IsElementTypeAllowed(element.GetType()))
                return;

            // Set the position on the canvas
            Canvas.SetLeft(element, position.X);
            Canvas.SetTop(element, position.Y);

            // Add the element to the canvas
            MainCanvas.Children.Add(element);

            // Make the element draggable
            MakeElementDraggable(element);
        }

        /// <summary>
        /// Remove all elements from the canvas
        /// </summary>
        public void ClearCanvas()
        {
            MainCanvas.Children.Clear();
        }

        /// <summary>
        /// Check if the element type is allowed to be dropped on the canvas
        /// </summary>
        private bool IsElementTypeAllowed(Type elementType)
        {
            if (AllowedElementTypes == null || AllowedElementTypes.Count == 0)
                return true;

            foreach (Type allowedType in AllowedElementTypes)
            {
                if (allowedType.IsAssignableFrom(elementType))
                    return true;
            }

            return false;
        }

        #endregion
    }

    #region Event Argument Classes

    public class ElementDroppedEventArgs : EventArgs
    {
        public UIElement Element { get; set; }
        public Point Position { get; set; }
    }

    public class ElementDraggedEventArgs : EventArgs
    {
        public UIElement Element { get; set; }
        public Point Position { get; set; }
        public Vector Delta { get; set; }
    }

    public class ElementResizedEventArgs : EventArgs
    {
        public UIElement Element { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Point Position { get; set; }
    }

    #endregion

    #region Helper Classes

    public class OriginalSize
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }

    #endregion
}