using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SortVisualizer.Models;
using SortVisualizer.ViewModels;

namespace SortVisualizer
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        private List<SortStep> _steps = [];
        private int _stepIndex;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            StopRunning();
            _viewModel.GenerateRandomArray();
            _steps.Clear();
            _stepIndex = 0;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsRunning)
                return;

            if (_steps.Count == 0 || _stepIndex >= _steps.Count)
            {
                _steps = _viewModel.CreateSteps();
                _stepIndex = 0;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _viewModel.IsRunning = true;

            try
            {
                while (_stepIndex < _steps.Count)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    _viewModel.ApplyStep(_steps[_stepIndex]);
                    _stepIndex++;

                    await Task.Delay(_viewModel.SpeedMs, _cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Pause時はここに入る。何もしなくてOK。
            }
            finally
            {
                _viewModel.IsRunning = false;
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            StopRunning();
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            StopRunning();

            if (_steps.Count == 0 || _stepIndex >= _steps.Count)
            {
                _steps = _viewModel.CreateSteps();
                _stepIndex = 0;
            }

            if (_stepIndex < _steps.Count)
            {
                _viewModel.ApplyStep(_steps[_stepIndex]);
                _stepIndex++;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            StopRunning();

            _steps.Clear();
            _stepIndex = 0;

            _viewModel.GenerateRandomArray();
        }

        private void StopRunning()
        {
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            _viewModel.IsRunning = false;
        }
    }
}