using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragDropClippedCanvas.Tests.TestUtilities
{
    /// <summary>
    /// Helper utilities for testing UIElements
    /// </summary>
    public static class UIElementHelper
    {
        /// <summary>
        /// Gets the Canvas.Left property value from an UIElement
        /// </summary>
        public static double GetCanvasLeft(UIElement element)
        {
            return Canvas.GetLeft(element);
        }

        /// <summary>
        /// Gets the Canvas.Top property value from an UIElement
        /// </summary>
        public static double GetCanvasTop(UIElement element)
        {
            return Canvas.GetTop(element);
        }

        /// <summary>
        /// Sets the Canvas.Left property value on an UIElement
        /// </summary>
        public static void SetCanvasLeft(UIElement element, double value)
        {
            Canvas.SetLeft(element, value);
        }

        /// <summary>
        /// Sets the Canvas.Top property value on an UIElement
        /// </summary>
        public static void SetCanvasTop(UIElement element, double value)
        {
            Canvas.SetTop(element, value);
        }

        /// <summary>
        /// Creates a Canvas with the specified size
        /// </summary>
        public static Canvas CreateCanvas(double width, double height)
        {
            var canvas = new Canvas
            {
                Width = width,
                Height = height
            };
            return canvas;
        }

        /// <summary>
        /// Creates a Rectangle with the specified size and color
        /// </summary>
        public static System.Windows.Shapes.Rectangle CreateRectangle(double width, double height, Brush fill = null)
        {
            return new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill ?? Brushes.Blue
            };
        }

        /// <summary>
        /// Adds an element to a Canvas at the specified position
        /// </summary>
        public static void AddToCanvas(Canvas canvas, UIElement element, double left, double top)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            canvas.Children.Add(element);
        }

        /// <summary>
        /// Tests if an element is contained within a canvas bounds
        /// </summary>
        public static bool IsElementWithinCanvas(Canvas canvas, UIElement element)
        {
            if (!(element is FrameworkElement frameworkElement))
                return false;

            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            double right = left + frameworkElement.Width;
            double bottom = top + frameworkElement.Height;

            return left >= 0 &&
                   top >= 0 &&
                   right <= canvas.Width &&
                   bottom <= canvas.Height;
        }
    }
}