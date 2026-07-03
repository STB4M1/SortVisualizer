using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SortVisualizer.Models
{
    public class BenchmarkResult : INotifyPropertyChanged
    {
        private string _algorithmName = "";
        private int _dataSize;
        private double _elapsedMilliseconds;
        private double _chartWidth;
        private bool _isCompleted;
        private string _status = "";

        public string AlgorithmName
        {
            get => _algorithmName;
            set
            {
                _algorithmName = value;
                OnPropertyChanged();
            }
        }

        public int DataSize
        {
            get => _dataSize;
            set
            {
                _dataSize = value;
                OnPropertyChanged();
            }
        }

        public double ElapsedMilliseconds
        {
            get => _elapsedMilliseconds;
            set
            {
                _elapsedMilliseconds = value;
                OnPropertyChanged();
            }
        }

        public double ChartWidth
        {
            get => _chartWidth;
            set
            {
                _chartWidth = value;
                OnPropertyChanged();
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}