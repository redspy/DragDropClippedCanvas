using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DragDropClippedCanvas.ViewModels;

namespace DragDropClippedCanvas.Tests.ViewModels
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private MainWindowViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _viewModel = new MainWindowViewModel();
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            Assert.AreEqual(1024, _viewModel.CanvasWidth);
            Assert.AreEqual(768, _viewModel.CanvasHeight);
            Assert.AreEqual("Ready", _viewModel.StatusMessage);
            Assert.IsNotNull(_viewModel.ClearCanvasCommand);
            Assert.IsNotNull(_viewModel.AddSampleElementCommand);
        }

        [TestMethod]
        public void CanvasWidth_WhenSet_ShouldUpdateProperty()
        {
            // Arrange
            double newWidth = 800;
            bool propertyChangedFired = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.CanvasWidth))
                    propertyChangedFired = true;
            };

            // Act
            _viewModel.CanvasWidth = newWidth;

            // Assert
            Assert.AreEqual(newWidth, _viewModel.CanvasWidth);
            Assert.IsTrue(propertyChangedFired);
        }

        [TestMethod]
        public void CanvasHeight_WhenSet_ShouldUpdateProperty()
        {
            // Arrange
            double newHeight = 600;
            bool propertyChangedFired = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.CanvasHeight))
                    propertyChangedFired = true;
            };

            // Act
            _viewModel.CanvasHeight = newHeight;

            // Assert
            Assert.AreEqual(newHeight, _viewModel.CanvasHeight);
            Assert.IsTrue(propertyChangedFired);
        }

        [TestMethod]
        public void StatusMessage_WhenSet_ShouldUpdateProperty()
        {
            // Arrange
            string newMessage = "Test message";
            bool propertyChangedFired = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.StatusMessage))
                    propertyChangedFired = true;
            };

            // Act
            _viewModel.StatusMessage = newMessage;

            // Assert
            Assert.AreEqual(newMessage, _viewModel.StatusMessage);
            Assert.IsTrue(propertyChangedFired);
        }

        [TestMethod]
        public void AddSampleElementCommand_WhenExecuted_ShouldRaiseElementDroppedEvent()
        {
            // Arrange
            bool eventFired = false;
            _viewModel.ElementDropped += (sender, e) =>
            {
                eventFired = true;
                Assert.IsNotNull(e.Element);
                Assert.IsInstanceOfType(e.Element, typeof(System.Windows.Shapes.Rectangle));
            };

            // Act
            _viewModel.AddSampleElementCommand.Execute(null);

            // Assert
            Assert.IsTrue(eventFired);
            Assert.AreEqual("Sample element added", _viewModel.StatusMessage);
        }

        [TestMethod]
        public void ClearCanvasCommand_WhenExecuted_ShouldUpdateStatusMessage()
        {
            // Act
            _viewModel.ClearCanvasCommand.Execute(null);

            // Assert
            Assert.AreEqual("Canvas cleared", _viewModel.StatusMessage);
        }

        [TestMethod]
        public void ClearCanvasCommand_CanExecute_ShouldReturnTrue()
        {
            // Act & Assert
            Assert.IsTrue(_viewModel.ClearCanvasCommand.CanExecute(null));
        }
    }
}