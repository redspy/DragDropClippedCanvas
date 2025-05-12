using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DragDropClippedCanvas.Controls;
using DragDropClippedCanvas.ViewModels;

namespace DragDropClippedCanvas
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the view model from resources
            _viewModel = (MainWindowViewModel)FindResource("MainViewModel");

            // Subscribe to view model events
            _viewModel.ElementDropped += ViewModel_ElementDropped;

            // Subscribe to draggable canvas events
            DraggableCanvas.ElementDropped += DraggableCanvas_ElementDropped;
            DraggableCanvas.ElementDragged += DraggableCanvas_ElementDragged;
            DraggableCanvas.ElementResized += DraggableCanvas_ElementResized;

            // Set up command handlers
            _viewModel.ClearCanvasCommand = new RelayCommand(param => DraggableCanvas.ClearCanvas());

            // Add a sample element to demonstrate the functionality
            var element = new Rectangle
            {
                Width = 100,
                Height = 100,
                Fill = Brushes.Blue
            };

            DraggableCanvas.AddElement(element, new Point(50, 50));
            _viewModel.StatusMessage = "Canvas ready. You can drag the rectangle or add more elements. Hover an element and use mouse wheel to resize.";
        }

        private void ViewModel_ElementDropped(object sender, ElementEventArgs e)
        {
            // Add the element from the view model to the canvas
            DraggableCanvas.AddElement(e.Element, e.Position);
        }

        private void DraggableCanvas_ElementDropped(object sender, ElementDroppedEventArgs e)
        {
            // Update status message
            _viewModel.StatusMessage = $"Element dropped at ({e.Position.X:F0}, {e.Position.Y:F0})";
        }

        private void DraggableCanvas_ElementDragged(object sender, ElementDraggedEventArgs e)
        {
            // Update status message (only occasionally to avoid too many updates)
            if (DateTime.Now.Millisecond % 100 == 0)
            {
                _viewModel.StatusMessage = $"Element dragged to ({e.Position.X:F0}, {e.Position.Y:F0})";
            }
        }

        private void DraggableCanvas_ElementResized(object sender, ElementResizedEventArgs e)
        {
            // Update status message
            _viewModel.StatusMessage = $"Element resized to {e.Width:F0}x{e.Height:F0} at ({e.Position.X:F0}, {e.Position.Y:F0})";
        }
    }
}
