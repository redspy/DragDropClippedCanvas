using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DragDropClippedCanvas;

namespace DragDropClippedCanvas.Tests.Commands
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void Constructor_WithNullExecute_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new RelayCommand(null));
        }

        [TestMethod]
        public void Execute_ShouldInvokeExecuteAction()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand(parameter => executed = true);

            // Act
            command.Execute(null);

            // Assert
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Execute_WithParameter_ShouldPassParameterToExecuteAction()
        {
            // Arrange
            object parameterPassed = null;
            var expectedParameter = "test";
            var command = new RelayCommand(parameter => parameterPassed = parameter);

            // Act
            command.Execute(expectedParameter);

            // Assert
            Assert.AreEqual(expectedParameter, parameterPassed);
        }

        [TestMethod]
        public void CanExecute_WithoutCanExecuteFunc_ShouldReturnTrue()
        {
            // Arrange
            var command = new RelayCommand(parameter => { });

            // Act
            bool result = command.CanExecute(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanExecute_WithCanExecuteFunc_ShouldInvokeCanExecuteFunc()
        {
            // Arrange
            bool canExecute = false;
            var command = new RelayCommand(
                parameter => { },
                parameter => canExecute
            );

            // Act & Assert
            Assert.IsFalse(command.CanExecute(null));

            // Change condition
            canExecute = true;
            Assert.IsTrue(command.CanExecute(null));
        }

        [TestMethod]
        public void CanExecute_WithParameter_ShouldPassParameterToCanExecuteFunc()
        {
            // Arrange
            object parameterPassed = null;
            var expectedParameter = "test";
            var command = new RelayCommand(
                parameter => { },
                parameter =>
                {
                    parameterPassed = parameter;
                    return true;
                }
            );

            // Act
            command.CanExecute(expectedParameter);

            // Assert
            Assert.AreEqual(expectedParameter, parameterPassed);
        }

        [TestMethod]
        public void CanExecuteChanged_ShouldBeRegisteredWithCommandManager()
        {
            // This test is more of a verification that the event is wired up to CommandManager
            // Since this is not easily testable, we'll just create the command and
            // ensure it doesn't throw an exception when we add and remove handlers

            // Arrange
            var command = new RelayCommand(parameter => { });
            EventHandler handler = (sender, e) => { };

            // Act & Assert - No exceptions should be thrown
            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;
        }
    }
}