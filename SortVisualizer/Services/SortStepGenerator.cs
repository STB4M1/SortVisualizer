using System;
using System.Collections.Generic;
using System.Text;

using SortVisualizer.Models;

namespace SortVisualizer.Services
{
    public static class SortStepGenerator
    {
        public static IEnumerable<SortStep> GenerateSteps(
            SortAlgorithmType algorithm,
            int[] values)
        {
            return algorithm switch
            {
                SortAlgorithmType.BubbleSort => BubbleSort(values),
                SortAlgorithmType.SelectionSort => SelectionSort(values),
                SortAlgorithmType.InsertionSort => InsertionSort(values),
                SortAlgorithmType.QuickSort => QuickSort(values),
                SortAlgorithmType.MergeSort => MergeSort(values),
                _ => BubbleSort(values)
            };
        }

        public static string[] GetPseudoCode(SortAlgorithmType algorithm)
        {
            return algorithm switch
            {
                SortAlgorithmType.BubbleSort =>
                [
                    "for i = 0 to n - 2",
                    "  for j = 0 to n - 2 - i",
                    "    compare arr[j] and arr[j + 1]",
                    "    if arr[j] > arr[j + 1]",
                    "      swap arr[j], arr[j + 1]",
                    "  arr[n - 1 - i] is fixed"
                ],

                SortAlgorithmType.SelectionSort =>
                [
                    "for i = 0 to n - 2",
                    "  minIndex = i",
                    "  for j = i + 1 to n - 1",
                    "    compare arr[j] and arr[minIndex]",
                    "    if arr[j] < arr[minIndex]",
                    "      minIndex = j",
                    "  swap arr[i], arr[minIndex]",
                    "  arr[i] is fixed"
                ],

                SortAlgorithmType.InsertionSort =>
                [
                    "for i = 1 to n - 1",
                    "  key = arr[i]",
                    "  j = i - 1",
                    "  while j >= 0 and arr[j] > key",
                    "    arr[j + 1] = arr[j]",
                    "    j--",
                    "  arr[j + 1] = key"
                ],

                SortAlgorithmType.QuickSort =>
                [
                    "quickSort(left, right)",
                    "  pivot = arr[right]",
                    "  partition array",
                    "  move smaller values to left",
                    "  move larger values to right",
                    "  place pivot at fixed position",
                    "  quickSort(left side)",
                    "  quickSort(right side)"
                ],

                SortAlgorithmType.MergeSort =>
                [
                    "mergeSort(left, right)",
                    "  divide array into two halves",
                    "  sort left half",
                    "  sort right half",
                    "  merge two sorted halves",
                    "  copy merged values back"
                ],

                _ => []
            };
        }

        private static IEnumerable<SortStep> BubbleSort(int[] source)
        {
            int[] arr = Copy(source);
            int n = arr.Length;
            HashSet<int> sorted = [];

            yield return Step(arr, "バブルソート開始", currentLine: 0);

            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;

                yield return Step(arr, $"外側ループ i = {i}", currentLine: 0, sortedIndices: sorted);

                for (int j = 0; j < n - 1 - i; j++)
                {
                    yield return Step(
                        arr,
                        $"arr[{j}] = {arr[j]} と arr[{j + 1}] = {arr[j + 1]} を比較",
                        compare1: j,
                        compare2: j + 1,
                        currentLine: 2,
                        sortedIndices: sorted);

                    if (arr[j] > arr[j + 1])
                    {
                        yield return Step(
                            arr,
                            $"{arr[j]} > {arr[j + 1]} なので交換する",
                            swap1: j,
                            swap2: j + 1,
                            currentLine: 4,
                            sortedIndices: sorted);

                        Swap(arr, j, j + 1);
                        swapped = true;

                        yield return Step(
                            arr,
                            "交換完了",
                            swap1: j,
                            swap2: j + 1,
                            currentLine: 4,
                            sortedIndices: sorted);
                    }
                }

                sorted.Add(n - 1 - i);

                yield return Step(
                    arr,
                    $"右端から {i + 1} 個が確定",
                    currentLine: 5,
                    sortedIndices: sorted);

                if (!swapped)
                    break;
            }

            for (int i = 0; i < n; i++)
                sorted.Add(i);

            yield return Step(arr, "ソート完了", currentLine: 5, sortedIndices: sorted);
        }

        private static IEnumerable<SortStep> SelectionSort(int[] source)
        {
            int[] arr = Copy(source);
            int n = arr.Length;
            HashSet<int> sorted = [];

            yield return Step(arr, "選択ソート開始", currentLine: 0);

            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;

                yield return Step(
                    arr,
                    $"未整列範囲の先頭 i = {i} を最小値候補にする",
                    compare1: i,
                    currentLine: 1,
                    sortedIndices: sorted);

                for (int j = i + 1; j < n; j++)
                {
                    yield return Step(
                        arr,
                        $"arr[{j}] = {arr[j]} と 現在の最小値 arr[{minIndex}] = {arr[minIndex]} を比較",
                        compare1: j,
                        compare2: minIndex,
                        currentLine: 3,
                        sortedIndices: sorted);

                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;

                        yield return Step(
                            arr,
                            $"最小値候補を arr[{j}] = {arr[j]} に更新",
                            compare1: j,
                            currentLine: 5,
                            sortedIndices: sorted);
                    }
                }

                yield return Step(
                    arr,
                    $"arr[{i}] と arr[{minIndex}] を交換",
                    swap1: i,
                    swap2: minIndex,
                    currentLine: 6,
                    sortedIndices: sorted);

                Swap(arr, i, minIndex);
                sorted.Add(i);

                yield return Step(
                    arr,
                    $"arr[{i}] の位置が確定",
                    currentLine: 7,
                    sortedIndices: sorted);
            }

            for (int i = 0; i < n; i++)
                sorted.Add(i);

            yield return Step(arr, "ソート完了", currentLine: 7, sortedIndices: sorted);
        }

        private static IEnumerable<SortStep> InsertionSort(int[] source)
        {
            int[] arr = Copy(source);
            int n = arr.Length;
            HashSet<int> sorted = [0];

            yield return Step(arr, "挿入ソート開始。左側を整列済み領域として扱う", currentLine: 0, sortedIndices: sorted);

            for (int i = 1; i < n; i++)
            {
                int key = arr[i];
                int j = i - 1;

                yield return Step(
                    arr,
                    $"key = arr[{i}] = {key}",
                    compare1: i,
                    currentLine: 1,
                    sortedIndices: sorted);

                while (j >= 0 && arr[j] > key)
                {
                    yield return Step(
                        arr,
                        $"arr[{j}] = {arr[j]} > key = {key} なので右へずらす",
                        compare1: j,
                        compare2: j + 1,
                        currentLine: 3,
                        sortedIndices: sorted);

                    arr[j + 1] = arr[j];

                    yield return Step(
                        arr,
                        $"arr[{j}] を arr[{j + 1}] にコピー",
                        swap1: j,
                        swap2: j + 1,
                        currentLine: 4,
                        sortedIndices: sorted);

                    j--;
                }

                arr[j + 1] = key;

                sorted.Clear();
                for (int k = 0; k <= i; k++)
                    sorted.Add(k);

                yield return Step(
                    arr,
                    $"key = {key} を arr[{j + 1}] に挿入",
                    swap1: j + 1,
                    currentLine: 6,
                    sortedIndices: sorted);
            }

            for (int i = 0; i < n; i++)
                sorted.Add(i);

            yield return Step(arr, "ソート完了", currentLine: 6, sortedIndices: sorted);
        }

        private static IEnumerable<SortStep> QuickSort(int[] source)
        {
            int[] arr = Copy(source);
            HashSet<int> sorted = [];

            yield return Step(arr, "クイックソート開始", currentLine: 0);

            foreach (var step in QuickSortRecursive(arr, 0, arr.Length - 1, sorted))
                yield return step;

            for (int i = 0; i < arr.Length; i++)
                sorted.Add(i);

            yield return Step(arr, "ソート完了", currentLine: 7, sortedIndices: sorted);
        }

        private static IEnumerable<SortStep> QuickSortRecursive(
            int[] arr,
            int left,
            int right,
            HashSet<int> sorted)
        {
            if (left > right)
                yield break;

            if (left == right)
            {
                sorted.Add(left);
                yield return Step(
                    arr,
                    $"arr[{left}] は1要素なので確定",
                    compare1: left,
                    currentLine: 5,
                    sortedIndices: sorted);
                yield break;
            }

            int pivot = arr[right];
            int pivotIndex = right;
            int i = left - 1;

            yield return Step(
                arr,
                $"pivot = arr[{right}] = {pivot}",
                pivot: pivotIndex,
                currentLine: 1,
                sortedIndices: sorted);

            for (int j = left; j < right; j++)
            {
                yield return Step(
                    arr,
                    $"arr[{j}] = {arr[j]} と pivot = {pivot} を比較",
                    compare1: j,
                    pivot: pivotIndex,
                    currentLine: 2,
                    sortedIndices: sorted);

                if (arr[j] <= pivot)
                {
                    i++;

                    yield return Step(
                        arr,
                        $"pivot以下なので arr[{i}] と arr[{j}] を交換",
                        swap1: i,
                        swap2: j,
                        pivot: pivotIndex,
                        currentLine: 3,
                        sortedIndices: sorted);

                    Swap(arr, i, j);

                    yield return Step(
                        arr,
                        "交換完了",
                        swap1: i,
                        swap2: j,
                        pivot: pivotIndex,
                        currentLine: 3,
                        sortedIndices: sorted);
                }
            }

            yield return Step(
                arr,
                $"pivot を正しい位置 arr[{i + 1}] へ移動",
                swap1: i + 1,
                swap2: right,
                pivot: pivotIndex,
                currentLine: 5,
                sortedIndices: sorted);

            Swap(arr, i + 1, right);

            int fixedPivotIndex = i + 1;
            sorted.Add(fixedPivotIndex);

            yield return Step(
                arr,
                $"pivot の位置 arr[{fixedPivotIndex}] が確定",
                pivot: fixedPivotIndex,
                currentLine: 5,
                sortedIndices: sorted);

            foreach (var step in QuickSortRecursive(arr, left, fixedPivotIndex - 1, sorted))
                yield return step;

            foreach (var step in QuickSortRecursive(arr, fixedPivotIndex + 1, right, sorted))
                yield return step;
        }

        private static IEnumerable<SortStep> MergeSort(int[] source)
        {
            int[] arr = Copy(source);
            HashSet<int> sorted = [];

            yield return Step(arr, "マージソート開始", currentLine: 0);

            foreach (var step in MergeSortRecursive(arr, 0, arr.Length - 1, sorted))
                yield return step;

            for (int i = 0; i < arr.Length; i++)
                sorted.Add(i);

            yield return Step(arr, "ソート完了", currentLine: 5, sortedIndices: sorted);
        }

        private static IEnumerable<SortStep> MergeSortRecursive(
            int[] arr,
            int left,
            int right,
            HashSet<int> sorted)
        {
            if (left >= right)
                yield break;

            int mid = (left + right) / 2;

            yield return Step(
                arr,
                $"範囲 [{left}, {right}] を [{left}, {mid}] と [{mid + 1}, {right}] に分割",
                compare1: left,
                compare2: right,
                currentLine: 1,
                sortedIndices: sorted);

            foreach (var step in MergeSortRecursive(arr, left, mid, sorted))
                yield return step;

            foreach (var step in MergeSortRecursive(arr, mid + 1, right, sorted))
                yield return step;

            int[] leftArray = arr[left..(mid + 1)];
            int[] rightArray = arr[(mid + 1)..(right + 1)];

            int i = 0;
            int j = 0;
            int k = left;

            yield return Step(
                arr,
                $"範囲 [{left}, {right}] をマージする",
                compare1: left,
                compare2: right,
                currentLine: 4,
                sortedIndices: sorted);

            while (i < leftArray.Length && j < rightArray.Length)
            {
                yield return Step(
                    arr,
                    $"{leftArray[i]} と {rightArray[j]} を比較して小さい方を入れる",
                    compare1: left + i,
                    compare2: mid + 1 + j,
                    currentLine: 4,
                    sortedIndices: sorted);

                if (leftArray[i] <= rightArray[j])
                {
                    arr[k] = leftArray[i];
                    i++;
                }
                else
                {
                    arr[k] = rightArray[j];
                    j++;
                }

                yield return Step(
                    arr,
                    $"arr[{k}] に値を配置",
                    swap1: k,
                    currentLine: 5,
                    sortedIndices: sorted);

                k++;
            }

            while (i < leftArray.Length)
            {
                arr[k] = leftArray[i];

                yield return Step(
                    arr,
                    $"左側の残り {leftArray[i]} を arr[{k}] に配置",
                    swap1: k,
                    currentLine: 5,
                    sortedIndices: sorted);

                i++;
                k++;
            }

            while (j < rightArray.Length)
            {
                arr[k] = rightArray[j];

                yield return Step(
                    arr,
                    $"右側の残り {rightArray[j]} を arr[{k}] に配置",
                    swap1: k,
                    currentLine: 5,
                    sortedIndices: sorted);

                j++;
                k++;
            }
        }

        private static SortStep Step(
            int[] arr,
            string message,
            int? compare1 = null,
            int? compare2 = null,
            int? swap1 = null,
            int? swap2 = null,
            int? pivot = null,
            int currentLine = 0,
            HashSet<int>? sortedIndices = null)
        {
            return new SortStep
            {
                Values = Copy(arr),
                Message = message,
                CompareIndex1 = compare1,
                CompareIndex2 = compare2,
                SwapIndex1 = swap1,
                SwapIndex2 = swap2,
                PivotIndex = pivot,
                CurrentCodeLine = currentLine,
                SortedIndices = sortedIndices is null
                    ? []
                    : new HashSet<int>(sortedIndices)
            };
        }

        private static int[] Copy(int[] source)
        {
            return (int[])source.Clone();
        }

        private static void Swap(int[] arr, int i, int j)
        {
            if (i == j) return;

            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }
}