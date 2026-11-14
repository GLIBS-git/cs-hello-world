using System.Diagnostics;

namespace Exercises
{
    internal class SortingTest
    {
        public static void starter()
        {
            //SortingTest.BrutalForce(100000);
            //SortingTest.BrutalForce2(100000);
            //SortingTest.BrutalForce3(100000);
            //SortingTest.Splitting(100000);
            SortingTest.Splitting(100000000);
            //SortingTest.Test();
        }
        public static void Test()
        {
            int[] array_1 = { 3, 2, 1 };
            int[] array_2 = { 6, 5, 4 };
            int[] array_3 = array_1.Concat(array_2).ToArray();
            int[] array_4 = array_1.Take(2).ToArray();
            int[] array_5 = array_1.Take(2).ToArray();
        }
        public static void BrutalForce(int _elements)
        {
            int[] sourceArray = SortingTest.GetTestArray(_elements);
            int loops = 0;
            int swaps = 0;
            int iterations = 0;
            bool anySwapped = true;
            Console.WriteLine();
            Console.WriteLine("Brutal force");
            Console.WriteLine("============");
            Stopwatch sw = Stopwatch.StartNew();
            while (anySwapped)
            {
                loops ++;
                anySwapped = false;
                for (int i = 0; i < sourceArray.Length - 1; i++)
                {
                    iterations ++;
                    if (sourceArray[i] > sourceArray[i + 1])
                    {
                        int buffer = sourceArray[i + 1];
                        sourceArray[i + 1] = sourceArray[i];
                        sourceArray[i] = buffer;
                        anySwapped = true;
                        swaps++;
                    }
                }
            }
            Console.WriteLine(sw.Elapsed);
            sw.Stop();
            Console.WriteLine($"Loops: {loops}");
            Console.WriteLine($"Swaps: {swaps}");
            Console.WriteLine($"Iterations: {iterations}");
        }
        public static void BrutalForce2(int _elements)
        {
            int[] sourceArray = SortingTest.GetTestArray(_elements);
            int loops = 0;
            int swaps = 0;
            int iterations = 0;
            int end = sourceArray.Length - 1;
            bool anySwapped = true;
            Console.WriteLine();
            Console.WriteLine("Brutal force optimized");
            Console.WriteLine("======================");
            Stopwatch sw = Stopwatch.StartNew();
            while (anySwapped)
            {
                loops ++;
                anySwapped = false;
                for (int i = 0; i < end; i++)
                {
                    iterations ++;
                    if (sourceArray[i] > sourceArray[i + 1])
                    {
                        int buffer = sourceArray[i + 1];
                        sourceArray[i + 1] = sourceArray[i];
                        sourceArray[i] = buffer;
                        anySwapped = true;
                        swaps++;
                    }
                }
                end--;
            }
            Console.WriteLine(sw.Elapsed);
            sw.Stop();
            Console.WriteLine($"Loops: {loops}");
            Console.WriteLine($"Swaps: {swaps}");
            Console.WriteLine($"Iterations: {iterations}");
        }
        public static void BrutalForce3(int _elements)
        {
            int[] sourceArray = SortingTest.GetTestArray(_elements);
            int loops = 0;
            int swaps = 0;
            int iterations = 0;
            int start = 0;
            int end = sourceArray.Length - 1;
            bool anySwapped = true;
            Console.WriteLine();
            Console.WriteLine("Brutal force another optimized");
            Console.WriteLine("==============================");
            Stopwatch sw = Stopwatch.StartNew();
            while (anySwapped)
            {
                loops ++;
                anySwapped = false;
                for (int i = start; i < end; i++)
                {
                    iterations ++;
                    if (sourceArray[i] > sourceArray[i + 1])
                    {
                        int buffer = sourceArray[i + 1];
                        sourceArray[i + 1] = sourceArray[i];
                        sourceArray[i] = buffer;
                        anySwapped = true;
                        swaps++;
                    }
                }
                end--;
                loops ++;
                anySwapped = false;
                for (int i = end; i > start; i--)
                {
                    iterations ++;
                    if (sourceArray[i] < sourceArray[i - 1])
                    {
                        int buffer = sourceArray[i - 1];
                        sourceArray[i - 1] = sourceArray[i];
                        sourceArray[i] = buffer;
                        anySwapped = true;
                        swaps++;
                    }
                }
                start++;
            }
            Console.WriteLine(sw.Elapsed);
            sw.Stop();
            Console.WriteLine($"Loops: {loops}");
            Console.WriteLine($"Swaps: {swaps}");
            Console.WriteLine($"Iterations: {iterations}");
        }
        public static void Splitting(int _elements)
        {
            int splits = 0;
            int[] doSort(int[] _array2sort)
            {
                int sourceLength = _array2sort.Length;
                int[] retArray = { };
                splits++;
                if (sourceLength == 1)
                {
                    return _array2sort;
                }
                int[] lowerArray = new int[sourceLength], equalArray = new int[sourceLength], higherArray = new int[sourceLength];
                int lowerPos = -1, equalPos = -1, higherPos = -1;
                int firstElement = _array2sort.ElementAt(0);
                int middleElement = _array2sort.ElementAt(sourceLength / 2);
                int lastElement = _array2sort.ElementAt(sourceLength - 1);
                int averageElement = (firstElement + middleElement + lastElement) / 3;
                int splitElement = firstElement;
                int distance = Math.Abs(averageElement - firstElement);
                if (Math.Abs(averageElement - middleElement) < distance)
                {
                    splitElement = middleElement;
                    distance = Math.Abs(averageElement - middleElement);
                }
                if (Math.Abs(averageElement - lastElement) < distance)
                {
                    splitElement = lastElement;
                }
                for (int i = 0; i < sourceLength; i++)
                {
                    if (_array2sort[i] < splitElement)
                    {
                        lowerArray[++lowerPos] = _array2sort[i];
                    }
                    else if (_array2sort[i] == splitElement)
                    {
                        equalArray[++equalPos] = _array2sort[i];
                    }
                    else
                    {
                        higherArray[++higherPos] = _array2sort[i];
                    }
                }
                if (lowerPos >= 0)
                {
                    retArray = retArray.Concat(doSort(lowerArray.Take(lowerPos + 1).ToArray())).ToArray();
                }
                if (equalPos >= 0)
                {
                    retArray = retArray.Concat(doSort(equalArray.Take(equalPos + 1).ToArray())).ToArray();
                }
                if (higherPos >= 0)
                {
                    retArray = retArray.Concat(doSort(higherArray.Take(higherPos + 1).ToArray())).ToArray();
                }
                return retArray;
            }
            int[] sourceArray = SortingTest.GetTestArray(_elements);
            Console.WriteLine();
            Console.WriteLine("Split sort");
            Console.WriteLine("==========");
            Stopwatch sw = Stopwatch.StartNew();
            doSort(sourceArray);
            Console.WriteLine(sw.Elapsed);
            sw.Stop();
            Console.WriteLine($"Splits: {splits}");
        }
        public static int[] GetTestArray(int _elements)
        {
            int[] ret = new int[_elements];
            for (int i = 0; i < _elements; i++)
            {
                ret[i] = _elements - i;
            }
            return ret;
        }
    }
}
