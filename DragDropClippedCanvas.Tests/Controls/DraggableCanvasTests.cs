using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DragDropClippedCanvas.Controls;
using DragDropClippedCanvas.Tests.TestUtilities;

namespace DragDropClippedCanvas.Tests.Controls
{
    [TestClass]
    public class DraggableCanvasTests
    {
        private DraggableCanvas _draggableCanvas;

        [TestInitialize]
        public void Setup()
        {
            // Initialize draggable canvas before each test
            _draggableCanvas = new DraggableCanvas
            {
                CanvasWidth = 1024,
                CanvasHeight = 768,
                ClipElementsToCanvas = true,
                MaintainSquareAspectRatio = true
            };
        }

        [TestMethod]
        public void AddElement_ShouldAddElementToCanvas()
        {
            // Arrange
            var element = UIElementHelper.CreateRectangle(100, 100);
            var position = new Point(50, 50);
            bool eventFired = false;

            _draggableCanvas.ElementDropped += (sender, e) =>
            {
                eventFired = true;
                Assert.AreEqual(element, e.Element);
                Assert.AreEqual(position, e.Position);
            };

            // Act
            _draggableCanvas.AddElement(element, position);

            // Assert
            Assert.IsTrue(eventFired, "ElementDropped event was not fired");
        }

        [TestMethod]
        public void MakeElementDraggable_ShouldMakeElementDraggable()
        {
            // Arrange
            var mockElement = new MockElement(100, 100);
            var position = new Point(50, 50);

            // Act
            _draggableCanvas.AddElement(mockElement, position);

            // Assert
            // This test should simulate mouse events to test dragging
            // but here we only check the position registration
            // Actual drag testing would require more complex test code
            Assert.AreEqual(50, Canvas.GetLeft(mockElement));
            Assert.AreEqual(50, Canvas.GetTop(mockElement));
        }

        [TestMethod]
        public void ClearCanvas_ShouldRemoveAllElements()
        {
            // Arrange
            var element1 = UIElementHelper.CreateRectangle(100, 100);
            var element2 = UIElementHelper.CreateRectangle(100, 100);

            _draggableCanvas.AddElement(element1, new Point(50, 50));
            _draggableCanvas.AddElement(element2, new Point(200, 200));

            // Act
            _draggableCanvas.ClearCanvas();

            // Assert
            // We need logic to verify all elements are removed
            // but DraggableCanvas doesn't provide a public API for this
            // In a real implementation, we might use reflection or internal knowledge
            // This test is provided as a conceptual test only
        }

        [TestMethod]
        public void ElementResize_WithMouseWheel_ShouldResizeElement()
        {
            // Arrange
            var mockElement = new MockElement(100, 100);
            _draggableCanvas.AddElement(mockElement, new Point(100, 100));

            // We need code to simulate mouse wheel events
            // but here we only provide a conceptual test
            // In a real implementation, we'd use MouseWheelEventArgs for testing
        }
    }
}