using System;
using System.Collections.Generic;
using System.Text;

using SortVisualizer.Models;
using System.Diagnostics;

namespace SortVisualizer.Services
{
    public static class SortBenchmarkService
    {
        public static int[] GenerateRandomArray(int size, int minValue = 0, int maxValue = 100000)
        {
            Random random = new();

            return Enumerable
                .Range(0, size)
                .Select(_ => random.Next(minValue, maxValue))
                .ToArray();
        }

        public static double MeasureMilliseconds(SortAlgorithmType algorithm, int[] source)
        {
            int[] arr = (int[])source.Clone();

            Stopwatch stopwatch = Stopwatch.StartNew();

            switch (algorithm)
            {
                case SortAlgorithmType.BubbleSort:
                    BubbleSort(arr);
                    break;

                case SortAlgorithmType.SelectionSort:
                    SelectionSort(arr);
                    break;

                case SortAlgorithmType.InsertionSort:
                    InsertionSort(arr);
                    break;

                case SortAlgorithmType.QuickSort:
                    QuickSort(arr);
                    break;

                case SortAlgorithmType.MergeSort:
                    MergeSort(arr);
                    break;
            }

            stopwatch.Stop();

            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public static int[] SortAndReturn(SortAlgorithmType algorithm, int[] source)
        {
            int[] arr = (int[])source.Clone();

            switch (algorithm)
            {
                case SortAlgorithmType.BubbleSort:
                    BubbleSort(arr);
                    break;

                case SortAlgorithmType.SelectionSort:
                    SelectionSort(arr);
                    break;

                case SortAlgorithmType.InsertionSort:
                    InsertionSort(arr);
                    break;

                case SortAlgorithmType.QuickSort:
                    QuickSort(arr);
                    break;

                case SortAlgorithmType.MergeSort:
                    MergeSort(arr);
                    break;
            }

            return arr;
        }

        public static bool IsSorted(int[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i] > arr[i + 1])
                    return false;
            }

            return true;
        }

        private static void BubbleSort(int[] arr)
        {
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;

                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(arr, j, j + 1);
                        swapped = true;
                    }
                }

                if (!swapped)
                    break;
            }
        }

        private static void SelectionSort(int[] arr)
        {
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }

                Swap(arr, i, minIndex);
            }
        }

        private static void InsertionSort(int[] arr)
        {
            int n = arr.Length;

            for (int i = 1; i < n; i++)
            {
                int key = arr[i];
                int j = i - 1;

                while (j >= 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }

                arr[j + 1] = key;
            }
        }

        private static void QuickSort(int[] arr)
        {
            QuickSortRecursive(arr, 0, arr.Length - 1);
        }

        private static void QuickSortRecursive(int[] arr, int left, int right)
        {
            if (left >= right)
                return;

            int pivotIndex = Partition(arr, left, right);

            QuickSortRecursive(arr, left, pivotIndex - 1);
            QuickSortRecursive(arr, pivotIndex + 1, right);
        }

        private static int Partition(int[] arr, int left, int right)
        {
            int pivot = arr[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (arr[j] <= pivot)
                {
                    i++;
                    Swap(arr, i, j);
                }
            }

            Swap(arr, i + 1, right);
            return i + 1;
        }

        private static void MergeSort(int[] arr)
        {
            if (arr.Length <= 1)
                return;

            int[] temp = new int[arr.Length];
            MergeSortRecursive(arr, temp, 0, arr.Length - 1);
        }

        private static void MergeSortRecursive(int[] arr, int[] temp, int left, int right)
        {
            if (left >= right)
                return;

            int mid = (left + right) / 2;

            MergeSortRecursive(arr, temp, left, mid);
            MergeSortRecursive(arr, temp, mid + 1, right);

            Merge(arr, temp, left, mid, right);
        }

        private static void Merge(int[] arr, int[] temp, int left, int mid, int right)
        {
            int i = left;
            int j = mid + 1;
            int k = left;

            while (i <= mid && j <= right)
            {
                if (arr[i] <= arr[j])
                {
                    temp[k] = arr[i];
                    i++;
                }
                else
                {
                    temp[k] = arr[j];
                    j++;
                }

                k++;
            }

            while (i <= mid)
            {
                temp[k] = arr[i];
                i++;
                k++;
            }

            while (j <= right)
            {
                temp[k] = arr[j];
                j++;
                k++;
            }

            for (int index = left; index <= right; index++)
            {
                arr[index] = temp[index];
            }
        }

        private static void Swap(int[] arr, int i, int j)
        {
            if (i == j)
                return;

            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }
}