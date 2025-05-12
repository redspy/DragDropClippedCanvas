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
                double newWidth = Math.Max(10, currentWidth + delta);
                double newHeight = Math.Max(10, currentHeight + delta);

                // Limit maximum size increase to +100 from original size
                if (frameworkElement.Tag is OriginalSize originalSize)
                {
                    newWidth = Math.Min(originalSize.Width + 100, Math.Max(10, newWidth));
                    newHeight = Math.Min(originalSize.Height + 100, Math.Max(10, newHeight));
                }

                // Get current position
                double left = Canvas.GetLeft(_hoveredElement);
                double top = Canvas.GetTop(_hoveredElement);

                // Calculate the center point of the element
                double centerX = left + (currentWidth / 2);
                double centerY = top + (currentHeight / 2);

                // Calculate new position to maintain the center point
                double newLeft = centerX - (newWidth / 2);
                double newTop = centerY - (newHeight / 2);

                // Ensure the element stays within canvas boundaries
                if (ClipElementsToCanvas)
                {
                    // Check if the new dimensions and position will fit within the canvas
                    if (newLeft < 0)
                    {
                        newLeft = 0;
                        // May need to adjust width to fit
                        newWidth = Math.Min(newWidth, 2 * centerX);
                    }

                    if (newTop < 0)
                    {
                        newTop = 0;
                        // May need to adjust height to fit
                        newHeight = Math.Min(newHeight, 2 * centerY);
                    }

                    if (newLeft + newWidth > CanvasWidth)
                    {
                        // Adjust width first
                        newWidth = CanvasWidth - newLeft;

                        // If width is too small, adjust position and width
                        if (newWidth < 10)
                        {
                            newWidth = 10;
                            newLeft = Math.Max(0, CanvasWidth - newWidth);
                        }
                    }

                    if (newTop + newHeight > CanvasHeight)
                    {
                        // Adjust height first
                        newHeight = CanvasHeight - newTop;

                        // If height is too small, adjust position and height
                        if (newHeight < 10)
                        {
                            newHeight = 10;
                            newTop = Math.Max(0, CanvasHeight - newHeight);
                        }
                    }
                }

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