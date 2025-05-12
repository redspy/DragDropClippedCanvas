using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DragDropClippedCanvas
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private bool _isDragging;
        private Point _dragStartPoint;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void DraggableRectangle_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double delta = e.Delta > 0 ? 10 : -10;
            _viewModel.RectangleWidth = Math.Max(10, _viewModel.RectangleWidth + delta);
            _viewModel.RectangleHeight = Math.Max(10, _viewModel.RectangleHeight + delta);
            e.Handled = true;
        }

        private void DraggableRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(MainCanvas);
            DraggableRectangle.CaptureMouse();
            e.Handled = true;
        }

        private void DraggableRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(MainCanvas);
                double deltaX = currentPosition.X - _dragStartPoint.X;
                double deltaY = currentPosition.Y - _dragStartPoint.Y;
                _viewModel.RectangleLeft = Math.Max(0, Math.Min(MainCanvas.ActualWidth - _viewModel.RectangleWidth, _viewModel.RectangleLeft + deltaX));
                _viewModel.RectangleTop = Math.Max(0, Math.Min(MainCanvas.ActualHeight - _viewModel.RectangleHeight, _viewModel.RectangleTop + deltaY));
                _dragStartPoint = currentPosition;
                e.Handled = true;
            }
        }

        private void DraggableRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            DraggableRectangle.ReleaseMouseCapture();
            e.Handled = true;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private double _rectangleWidth = 100;
        private double _rectangleHeight = 100;
        private double _rectangleLeft = 0;
        private double _rectangleTop = 0;

        public double RectangleWidth
        {
            get { return _rectangleWidth; }
            set
            {
                if (_rectangleWidth != value)
                {
                    _rectangleWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double RectangleHeight
        {
            get { return _rectangleHeight; }
            set
            {
                if (_rectangleHeight != value)
                {
                    _rectangleHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double RectangleLeft
        {
            get { return _rectangleLeft; }
            set
            {
                if (_rectangleLeft != value)
                {
                    _rectangleLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public double RectangleTop
        {
            get { return _rectangleTop; }
            set
            {
                if (_rectangleTop != value)
                {
                    _rectangleTop = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
