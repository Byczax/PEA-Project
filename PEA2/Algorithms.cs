using System;
using System.Linq;

namespace PEA2
{
    public class Algorithms
    {
        private static Matrix _matrix;
        private static int _bestRoad;
        private static int[] _bestPermutation;
        private static Random random = new Random();

        public static int[] TabuSearch(Matrix givenMatrix)
        {
            _matrix = givenMatrix;
            return _bestPermutation;
        }

        public static int[] SimulatedAnnealing(Matrix givenMatrix, int numberOfTrials)
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
            var permutation = Enumerable.Range(0, _matrix.Size).ToArray();
            _bestRoad = _matrix.CalculateRoadArray(permutation);
            var trial = 0;
            double temperature = _bestRoad * _matrix.Size;
            const double alpha = 0.99;
            while (trial < numberOfTrials)
            {
                var randomStart = random.Next(0, _matrix.Size);
                int randomNext;
                while (true)
                {
                    randomNext = random.Next(0, _matrix.Size);
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
                else if (random.Next() < Math.Exp(-(actualCost - _bestRoad) / temperature))
                {
                    _bestPermutation = (int[])permutation.Clone();
                    _bestRoad = actualCost;
                }

                temperature = alpha * temperature;
                ++trial;
            }

            return _bestPermutation;
        }
    }
}