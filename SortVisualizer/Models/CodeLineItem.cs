using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SortVisualizer.Models
{
    public class CodeLineItem : INotifyPropertyChanged
    {
        private bool _isCurrent;

        public int LineNumber { get; set; }
        public string Text { get; set; } = "";

        public bool IsCurrent
        {
            get => _isCurrent;
            set
            {
                _isCurrent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Background));
            }
        }

        public Brush Background => IsCurrent ? Brushes.Gold : Brushes.Transparent;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}