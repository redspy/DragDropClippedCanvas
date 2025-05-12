using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DragDropClippedCanvas.ViewModels;

namespace DragDropClippedCanvas.Tests.ViewModels
{
    [TestClass]
    public class ViewModelBaseTests
    {
        // ViewModelBase를 테스트하기 위한 간단한 구현체
        private class TestViewModel : ViewModelBase
        {
            private string _testProperty;
            public string TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value);
            }

            private int _testIntProperty;
            public int TestIntProperty
            {
                get => _testIntProperty;
                set => SetProperty(ref _testIntProperty, value);
            }
        }

        private TestViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _viewModel = new TestViewModel();
        }

        [TestMethod]
        public void Implements_INotifyPropertyChanged()
        {
            // Assert
            Assert.IsInstanceOfType(_viewModel, typeof(INotifyPropertyChanged));
        }

        [TestMethod]
        public void SetProperty_WhenValueChanges_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            bool eventRaised = false;
            string propertyName = null;

            _viewModel.PropertyChanged += (sender, e) =>
            {
                eventRaised = true;
                propertyName = e.PropertyName;
            };

            // Act
            _viewModel.TestProperty = "New Value";

            // Assert
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(nameof(_viewModel.TestProperty), propertyName);
            Assert.AreEqual("New Value", _viewModel.TestProperty);
        }

        [TestMethod]
        public void SetProperty_WhenValueDoesNotChange_ShouldNotRaisePropertyChangedEvent()
        {
            // Arrange
            _viewModel.TestProperty = "Initial Value";
            bool eventRaised = false;

            _viewModel.PropertyChanged += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act - set to the same value
            _viewModel.TestProperty = "Initial Value";

            // Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void SetProperty_WithDifferentTypes_ShouldWorkCorrectly()
        {
            // Arrange
            bool eventRaised = false;
            string propertyName = null;

            _viewModel.PropertyChanged += (sender, e) =>
            {
                eventRaised = true;
                propertyName = e.PropertyName;
            };

            // Act
            _viewModel.TestIntProperty = 42;

            // Assert
            Assert.IsTrue(eventRaised);
            Assert.AreEqual(nameof(_viewModel.TestIntProperty), propertyName);
            Assert.AreEqual(42, _viewModel.TestIntProperty);
        }
    }
}