using System;
using System.Windows;
using DragDropClippedCanvas.ViewModels;
using DragDropClippedCanvas.Controls;

namespace DragDropClippedCanvas.Tests.TestUtilities
{
    /// <summary>
    /// Helper class to create various event args for testing
    /// </summary>
    public static class TestEventArgs
    {
        /// <summary>
        /// Creates an ElementDroppedEventArgs with the specified element and position
        /// </summary>
        public static ElementDroppedEventArgs CreateElementDroppedEventArgs(UIElement element, Point position)
        {
            return new ElementDroppedEventArgs
            {
                Element = element,
                Position = position
            };
        }

        /// <summary>
        /// Creates an ElementDraggedEventArgs with the specified element, position and delta
        /// </summary>
        public static ElementDraggedEventArgs CreateElementDraggedEventArgs(UIElement element, Point position, Vector delta)
        {
            return new ElementDraggedEventArgs
            {
                Element = element,
                Position = position,
                Delta = delta
            };
        }

        /// <summary>
        /// Creates an ElementResizedEventArgs with the specified element, width, height and position
        /// </summary>
        public static ElementResizedEventArgs CreateElementResizedEventArgs(UIElement element, double width, double height, Point position)
        {
            return new ElementResizedEventArgs
            {
                Element = element,
                Width = width,
                Height = height,
                Position = position
            };
        }

        /// <summary>
        /// Creates an ElementEventArgs with the specified element and position
        /// </summary>
        public static ElementEventArgs CreateElementEventArgs(UIElement element, Point position)
        {
            return new ElementEventArgs
            {
                Element = element,
                Position = position
            };
        }
    }
}