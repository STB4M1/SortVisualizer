using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SortVisualizer.Models
{
    public class BarItem : INotifyPropertyChanged
    {
        private int _value;
        private double _displayHeight;
        private Brush _fill = Brushes.LightGray;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public double DisplayHeight
        {
            get => _displayHeight;
            set
            {
                _displayHeight = value;
                OnPropertyChanged();
            }
        }

        public Brush Fill
        {
            get => _fill;
            set
            {
                _fill = value;
                OnPropertyChanged();
            }
        }

        public double BarWidth { get; set; } = 16;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}