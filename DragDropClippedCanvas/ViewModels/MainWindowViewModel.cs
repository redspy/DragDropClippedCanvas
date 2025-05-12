using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DragDropClippedCanvas.ViewModels
{
    /// <summary>
    /// ViewModel for the main window
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private double _canvasWidth = 1024;
        private double _canvasHeight = 768;
        private string _statusMessage = "Ready";

        /// <summary>
        /// Width of the draggable canvas
        /// </summary>
        public double CanvasWidth
        {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth, value);
        }

        /// <summary>
        /// Height of the draggable canvas
        /// </summary>
        public double CanvasHeight
        {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight, value);
        }

        /// <summary>
        /// Status message to display
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// Command to clear the canvas
        /// </summary>
        public RelayCommand ClearCanvasCommand { get; set; }

        /// <summary>
        /// Command to add a sample element to the canvas
        /// </summary>
        public RelayCommand AddSampleElementCommand { get; set; }

        /// <summary>
        /// Event raised when an element is dropped on the canvas
        /// </summary>
        public event EventHandler<ElementEventArgs> ElementDropped;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize commands
            ClearCanvasCommand = new RelayCommand(ExecuteClearCanvas, CanExecuteClearCanvas);
            AddSampleElementCommand = new RelayCommand(ExecuteAddSampleElement);
        }

        /// <summary>
        /// Execute the ClearCanvas command
        /// </summary>
        private void ExecuteClearCanvas(object parameter)
        {
            // This will be handled by the view
            StatusMessage = "Canvas cleared";
        }

        /// <summary>
        /// Determine if the ClearCanvas command can be executed
        /// </summary>
        private bool CanExecuteClearCanvas(object parameter)
        {
            // This would typically check if there are any elements on the canvas
            return true;
        }

        /// <summary>
        /// Execute the AddSampleElement command
        /// </summary>
        private void ExecuteAddSampleElement(object parameter)
        {
            // Create a sample element (a rectangle)
            var element = new System.Windows.Shapes.Rectangle
            {
                Width = 100,
                Height = 100,
                Fill = System.Windows.Media.Brushes.Blue
            };

            // Calculate a random position within the canvas
            Random random = new Random();
            double left = random.NextDouble() * (CanvasWidth - 100);
            double top = random.NextDouble() * (CanvasHeight - 100);

            // Raise the ElementDropped event with the element and position
            ElementDropped?.Invoke(this, new ElementEventArgs
            {
                Element = element,
                Position = new Point(left, top)
            });

            StatusMessage = "Sample element added";
        }
    }

    /// <summary>
    /// Event arguments for element events
    /// </summary>
    public class ElementEventArgs : EventArgs
    {
        /// <summary>
        /// The UI element
        /// </summary>
        public UIElement Element { get; set; }

        /// <summary>
        /// The position of the element
        /// </summary>
        public Point Position { get; set; }
    }
}