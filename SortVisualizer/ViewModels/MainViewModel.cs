using System;
using System.Collections.Generic;
using System.Text;

using SortVisualizer.Models;
using SortVisualizer.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SortVisualizer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Random _random = new();

        private int[] _currentValues = [];

        private SortAlgorithmType _selectedAlgorithm = SortAlgorithmType.BubbleSort;
        private int _arraySize = 30;
        private int _speedMs = 80;
        private string _message = "Shuffle を押して配列を生成してください";
        private int _compareCount;
        private int _swapCount;
        private bool _isRunning;

        public ObservableCollection<BarItem> Bars { get; } = [];
        public ObservableCollection<CodeLineItem> CodeLines { get; } = [];

        public Array Algorithms => Enum.GetValues(typeof(SortAlgorithmType));

        public SortAlgorithmType SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set
            {
                _selectedAlgorithm = value;
                OnPropertyChanged();
                LoadPseudoCode();
                ResetStats();
            }
        }

        public int ArraySize
        {
            get => _arraySize;
            set
            {
                _arraySize = value;
                OnPropertyChanged();
            }
        }

        public int SpeedMs
        {
            get => _speedMs;
            set
            {
                _speedMs = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public int CompareCount
        {
            get => _compareCount;
            set
            {
                _compareCount = value;
                OnPropertyChanged();
            }
        }

        public int SwapCount
        {
            get => _swapCount;
            set
            {
                _swapCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            LoadPseudoCode();
            GenerateRandomArray();
        }

        public void GenerateRandomArray()
        {
            _currentValues = Enumerable
                .Range(0, ArraySize)
                .Select(_ => _random.Next(10, 260))
                .ToArray();

            ResetStats();

            ApplyStep(new SortStep
            {
                Values = _currentValues,
                Message = "ランダム配列を生成しました"
            });
        }

        public List<SortStep> CreateSteps()
        {
            ResetStats();

            return SortStepGenerator
                .GenerateSteps(SelectedAlgorithm, _currentValues)
                .ToList();
        }

        public void ApplyStep(SortStep step)
        {
            Bars.Clear();

            int max = step.Values.Length == 0 ? 1 : step.Values.Max();
            double chartMaxHeight = 260.0;
            double barWidth = CalculateBarWidth(step.Values.Length);

            for (int i = 0; i < step.Values.Length; i++)
            {
                Brush fill = Brushes.LightGray;

                if (step.SortedIndices.Contains(i))
                    fill = Brushes.MediumSeaGreen;

                if (step.CompareIndex1 == i || step.CompareIndex2 == i)
                    fill = Brushes.Orange;

                if (step.SwapIndex1 == i || step.SwapIndex2 == i)
                    fill = Brushes.DodgerBlue;

                if (step.PivotIndex == i)
                    fill = Brushes.MediumPurple;

                Bars.Add(new BarItem
                {
                    Value = step.Values[i],
                    DisplayHeight = Math.Max(4, step.Values[i] / (double)max * chartMaxHeight),
                    Fill = fill,
                    BarWidth = barWidth
                });
            }

            if (step.CompareIndex1 is not null || step.CompareIndex2 is not null)
                CompareCount++;

            if (step.SwapIndex1 is not null || step.SwapIndex2 is not null)
                SwapCount++;

            Message = step.Message;
            HighlightCodeLine(step.CurrentCodeLine);
        }

        public void ResetStats()
        {
            CompareCount = 0;
            SwapCount = 0;
            Message = "";
        }

        private double CalculateBarWidth(int count)
        {
            if (count <= 0) return 16;

            double width = 760.0 / count;
            return Math.Clamp(width, 4, 22);
        }

        private void LoadPseudoCode()
        {
            CodeLines.Clear();

            string[] lines = SortStepGenerator.GetPseudoCode(SelectedAlgorithm);

            for (int i = 0; i < lines.Length; i++)
            {
                CodeLines.Add(new CodeLineItem
                {
                    LineNumber = i,
                    Text = lines[i]
                });
            }
        }

        private void HighlightCodeLine(int lineNumber)
        {
            foreach (var line in CodeLines)
            {
                line.IsCurrent = line.LineNumber == lineNumber;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}