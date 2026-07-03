using System;
using System.Collections.Generic;
using System.Text;

namespace SortVisualizer.Models
{
    public class SortStep
    {
        public int[] Values { get; set; } = [];

        public int? CompareIndex1 { get; set; }
        public int? CompareIndex2 { get; set; }

        public int? SwapIndex1 { get; set; }
        public int? SwapIndex2 { get; set; }

        public int? PivotIndex { get; set; }

        public HashSet<int> SortedIndices { get; set; } = [];

        public string Message { get; set; } = "";

        public int CurrentCodeLine { get; set; }
    }
}