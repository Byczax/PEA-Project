using System;
using System.Diagnostics;
using System.Linq;

namespace PEA2
{
    public static class Algorithms
    {
        private static Matrix _matrix;
        private static int _bestRoad;
        private static int[] _bestPermutation;
        private static readonly Random Random = new Random();

        public static int[] TabuSearch(Matrix givenMatrix)
        {
            _matrix = givenMatrix;
            return _bestPermutation;
        }

        public static int[] SimulatedAnnealing(Matrix givenMatrix, int trialTime)
        {
            // fast swap
            static void Swap(int[] arr, int index1, int index2)
            {
                var temp = arr[index1];
                arr[index1] = arr[index2];
                arr[index2] = temp;
            }

            static void Reverse(int[] arr, int index1, int index2)
            {
                while (index1 < index2)
                {
                    Swap(arr, index1, index2);
                    index1++;
                    index2--;
                }
            }

            _matrix = givenMatrix;
            var permutation = Enumerable.Range(0, _matrix.Size).ToArray(); // set start solution
            _bestRoad = _matrix.CalculateRoadArray(permutation);
            double temperature = _bestRoad * _matrix.Size; // set temperature
            const double alpha = 0.99;
            var timer = new Stopwatch();
            trialTime *= 1000; // change to ms
            timer.Start();
            while (timer.ElapsedMilliseconds <= trialTime)
            {
                var randomStart = Random.Next(0, _matrix.Size);
                int randomNext;
                while (true)
                {
                    randomNext = Random.Next(0, _matrix.Size);
                    if (randomStart != randomNext)
                        break;
                }

                Reverse(permutation, randomNext, randomStart);
                var actualCost = _matrix.CalculateRoadArray(permutation);
                if (actualCost < _bestRoad)
                {
                    _bestPermutation = (int[])permutation.Clone();
                    _bestRoad = actualCost;
                }
                else if (Random.Next() < Math.Exp(-(actualCost - _bestRoad) / temperature))
                {
                    _bestPermutation = (int[])permutation.Clone();
                    _bestRoad = actualCost;
                }

                temperature = alpha * temperature;
            }

            return _bestPermutation;
        }
    }
}