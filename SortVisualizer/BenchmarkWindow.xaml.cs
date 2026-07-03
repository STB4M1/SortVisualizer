using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SortVisualizer.Models;
using SortVisualizer.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SortVisualizer
{
    public partial class BenchmarkWindow : Window, INotifyPropertyChanged
    {
        private int[] _baseArray = [];

        private string _dataPreviewText = "";

        public ObservableCollection<BenchmarkResult> Results { get; } = [];
        public ObservableCollection<DataValueItem> DataValues { get; } = [];

        public string DataPreviewText
        {
            get => _dataPreviewText;
            set
            {
                _dataPreviewText = value;
                OnPropertyChanged();
            }
        }

        public BenchmarkWindow()
        {
            InitializeComponent();
            DataContext = this;

            InitializeResults();
            GenerateDataFromCurrentSettings();
        }

        private void InitializeResults()
        {
            Results.Clear();

            foreach (SortAlgorithmType algorithm in Enum.GetValues(typeof(SortAlgorithmType)))
            {
                Results.Add(new BenchmarkResult
                {
                    AlgorithmName = algorithm.ToString(),
                    DataSize = 0,
                    ElapsedMilliseconds = 0,
                    ChartWidth = 0,
                    Status = "Not Run"
                });
            }
        }

        private void GenerateDataButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateDataFromCurrentSettings();
            ClearBenchmarkValues();

            MessageTextBlock.Text = "新しいデータを生成しました。";
        }

        private void GenerateDataFromCurrentSettings()
        {
            if (!TryGetSettings(out int dataSize, out _))
                return;

            _baseArray = SortBenchmarkService.GenerateRandomArray(dataSize);

            UpdateDataView();
        }
        private void UpdateDataView()
        {
            UpdateDataView(_baseArray);
        }

        private void UpdateDataView(int[] values)
        {
            DataValues.Clear();

            for (int i = 0; i < values.Length; i++)
            {
                DataValues.Add(new DataValueItem
                {
                    Index = i,
                    Value = values[i]
                });
            }

            DataPreviewText = string.Join(", ", values);
        }

        private async void RunAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetSettings(out int dataSize, out int repeat))
                return;

            EnsureBaseArray(dataSize);

            ClearBenchmarkValues();

            MessageTextBlock.Text = "全アルゴリズムを実行中...";

            SortAlgorithmType? lastAlgorithm = null;

            foreach (var result in Results)
            {
                await RunBenchmarkAsync(result, _baseArray, repeat);
                UpdateChartWidths();

                lastAlgorithm = Enum.Parse<SortAlgorithmType>(result.AlgorithmName);
            }

            if (lastAlgorithm is not null)
            {
                ShowSortedData(lastAlgorithm.Value);
            }

            MessageTextBlock.Text = "全アルゴリズムの実行が完了しました。Actual Data にソート結果を表示しました。";
        }

        private async void RunSingleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetSettings(out int dataSize, out int repeat))
                return;

            EnsureBaseArray(dataSize);

            if (sender is not Button button)
                return;

            if (button.DataContext is not BenchmarkResult result)
                return;

            MessageTextBlock.Text = $"{result.AlgorithmName} を実行中...";

            await RunBenchmarkAsync(result, _baseArray, repeat);

            UpdateChartWidths();

            SortAlgorithmType algorithm =
                Enum.Parse<SortAlgorithmType>(result.AlgorithmName);

            ShowSortedData(algorithm);

            MessageTextBlock.Text = $"{result.AlgorithmName} の実行が完了しました。Actual Data にソート結果を表示しました。";
        }

        private async Task RunBenchmarkAsync(BenchmarkResult result, int[] baseArray, int repeat)
        {
            result.Status = "Running...";
            result.DataSize = baseArray.Length;
            result.ElapsedMilliseconds = 0;
            result.ChartWidth = 0;
            result.IsCompleted = false;

            await Task.Run(() =>
            {
                SortAlgorithmType algorithm =
                    Enum.Parse<SortAlgorithmType>(result.AlgorithmName);

                double totalMs = 0;

                for (int i = 0; i < repeat; i++)
                {
                    totalMs += SortBenchmarkService.MeasureMilliseconds(
                        algorithm,
                        baseArray);
                }

                double averageMs = totalMs / repeat;

                Dispatcher.Invoke(() =>
                {
                    result.ElapsedMilliseconds = averageMs;
                    result.Status = "Completed";
                    result.IsCompleted = true;
                });
            });
        }

        private void EnsureBaseArray(int dataSize)
        {
            if (_baseArray.Length != dataSize)
            {
                _baseArray = SortBenchmarkService.GenerateRandomArray(dataSize);
                UpdateDataView();
                ClearBenchmarkValues();
            }
        }

        private void ClearBenchmarkValues()
        {
            foreach (var result in Results)
            {
                result.DataSize = 0;
                result.ElapsedMilliseconds = 0;
                result.ChartWidth = 0;
                result.Status = "Not Run";
                result.IsCompleted = false;
            }
        }

        private void UpdateChartWidths()
        {
            double maxMs = Results.Max(r => r.ElapsedMilliseconds);

            if (maxMs <= 0)
            {
                foreach (var result in Results)
                    result.ChartWidth = 0;

                return;
            }

            double maxWidth = 330.0;

            foreach (var result in Results)
            {
                result.ChartWidth = result.ElapsedMilliseconds / maxMs * maxWidth;
            }
        }

        private bool TryGetSettings(out int dataSize, out int repeat)
        {
            dataSize = 0;
            repeat = 0;

            if (!int.TryParse(DataSizeTextBox.Text, out dataSize))
            {
                MessageBox.Show("Data Size には整数を入力してください。");
                return false;
            }

            if (!int.TryParse(RepeatTextBox.Text, out repeat))
            {
                MessageBox.Show("Repeat には整数を入力してください。");
                return false;
            }

            if (dataSize <= 0)
            {
                MessageBox.Show("Data Size は1以上にしてください。");
                return false;
            }

            if (repeat <= 0)
            {
                MessageBox.Show("Repeat は1以上にしてください。");
                return false;
            }

            return true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeResults();

            _baseArray = [];
            DataValues.Clear();
            DataPreviewText = "";

            MessageTextBlock.Text = "結果とデータをクリアしました。";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShowSortedData(SortAlgorithmType algorithm)
        {
            if (_baseArray.Length == 0)
                return;

            int[] sortedArray = SortBenchmarkService.SortAndReturn(algorithm, _baseArray);

            UpdateDataView(sortedArray);
        }
    }
}