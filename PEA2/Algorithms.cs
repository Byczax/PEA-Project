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
        private static readonly Random Random = new();
        private static readonly Stopwatch Timer = new(); // create stopwatch

        /// <summary>
        ///     Tabu Search Algorithm
        /// </summary>
        /// <param name="givenMatrix"></param>
        /// <param name="trialTime"></param>
        /// <param name="neighbour"></param>
        /// <param name="diversification"></param>
        /// <returns>the best solution found</returns>
        public static int[] TabuSearch(Matrix givenMatrix, double trialTime, Action<int[], int, int> neighbour,
            bool diversification)
        {
            void DecrementList(int[,] list)
            {
                var size = list.GetLength(0);
                for (var i = 1; i < size; i++)
                for (var j = i + 1; j < size; j++)
                    if (list[i, j] > 0)
                        --list[i, j];
            }

            _matrix = givenMatrix;
            var permutation = Enumerable.Range(0, _matrix.Size).ToArray(); // set start solution
            _bestPermutation = (int[])permutation.Clone(); // set start as best
            var currentRoad = _bestRoad = _matrix.CalculateFullRoad(permutation);
            var currentPermutation = new int[_matrix.Size];

            var tabuList = new int[_matrix.Size, _matrix.Size];
            var cadence = _matrix.Size;
            var criticalEvents = 0;

            var timer = new Stopwatch(); // create stopwatch
            trialTime *= 1000; // change to ms
            timer.Restart(); // start timer

            while (timer.ElapsedMilliseconds <= trialTime)
            {
                var bestI = 0;
                var bestJ = 0;
                var previousRoad = currentRoad;
                for (var i = 1; i < _matrix.Size; i++)
                for (var j = i + 1; j < _matrix.Size; j++)
                {
                    currentPermutation = (int[])permutation.Clone();
                    neighbour(currentPermutation, i, j);

                    var actualRoad = _matrix.CalculateFullRoad(currentPermutation);
                    if (actualRoad >= _bestRoad && (actualRoad >= currentRoad || tabuList[i, j] != 0)) continue;
                    currentRoad = actualRoad;
                    bestI = i;
                    bestJ = j;
                }

                neighbour(permutation, bestI, bestJ);
                tabuList[bestI, bestJ] = cadence;

                DecrementList(tabuList);

                if (currentRoad < _bestRoad)
                {
                    _bestPermutation = (int[])permutation.Clone();
                    _bestRoad = currentRoad;
                    criticalEvents = 0;
                }
                else if (diversification && currentRoad >= previousRoad)
                {
                    criticalEvents++;
                    if (criticalEvents < 20) continue;
                    Shuffle(currentPermutation);
                    currentRoad = _matrix.CalculateFullRoad(currentPermutation);
                    tabuList = new int[_matrix.Size, _matrix.Size];
                }
            }


            return _bestPermutation;
        }

        /// <summary>
        ///     Simulated Annealing algorithm
        /// </summary>
        /// <param name="givenMatrix"></param>
        /// <param name="trialTime"></param>
        /// <param name="neighbour"></param>
        /// <returns>the best solution found</returns>
        public static int[] SimulatedAnnealing(Matrix givenMatrix, double trialTime, Action<int[], int, int> neighbour)
        {
            static bool AnnealingFunction(int newCost, double temperature)
            {
                return Random.NextDouble() < Math.Exp(-(newCost - _bestRoad) / temperature);
            }


            _matrix = givenMatrix;
            var activePermutation = Enumerable.Range(0, _matrix.Size).ToArray(); // set start solution
            _bestPermutation = (int[])activePermutation.Clone(); // set start as best
            var localMinimumRoad =
                _bestRoad = _matrix.CalculateFullRoad(activePermutation); // set start road value as best
            var localMinimumPermutation = (int[])activePermutation.Clone();
            double temperature = _bestRoad * _matrix.Size; // set temperature
            const double alpha = 0.99;

            trialTime *= 1000; // change to ms
            Timer.Restart(); // start timer
            while (Timer.ElapsedMilliseconds <= trialTime)
            {
                for (var i = 0; i < 10 * _matrix.Size; i++)
                {
                    activePermutation = (int[])localMinimumPermutation.Clone();
                    var randomFirstVertex = Random.Next(1, _matrix.Size); // get first random vertex (skip 0)

                    int randomSecondVertex;
                    while (true) // get second random vertex different from first (skip 0)
                    {
                        randomSecondVertex = Random.Next(1, _matrix.Size);
                        if (randomFirstVertex != randomSecondVertex)
                            break;
                    }

                    neighbour(activePermutation, randomFirstVertex, randomSecondVertex);

                    var actualRoad = _matrix.CalculateFullRoad(activePermutation); // calculate road with end in 0
                    if (actualRoad < _bestRoad) // if we get better road
                    {
                        _bestPermutation = (int[])activePermutation.Clone();
                        _bestRoad = localMinimumRoad = actualRoad;
                        localMinimumPermutation = (int[])activePermutation.Clone();
                    }
                    else if (actualRoad < localMinimumRoad)
                    {
                        localMinimumPermutation = (int[])activePermutation.Clone();
                        localMinimumRoad = actualRoad;
                    }
                    else if (AnnealingFunction(actualRoad, temperature))
                    {
                        localMinimumPermutation = (int[])activePermutation.Clone();
                        localMinimumRoad = actualRoad;
                    }
                }

                temperature *= alpha;
            }

            return _bestPermutation;
        }

        /// <summary>
        ///     Random array shuffle generator
        /// </summary>
        /// <param name="arr"></param>
        private static void Shuffle(int[] arr)
        {
            for (var i = 1; i < arr.Length; i++) Swap(arr, i, Random.Next(1, arr.Length));
        }

        /// <summary>
        ///     fast swap
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public static void Swap(int[] arr, int index1, int index2)
        {
            var temp = arr[index1];
            arr[index1] = arr[index2];
            arr[index2] = temp;
        }

        /// <summary>
        ///     reverse list for given index-es
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public static void Reverse(int[] arr, int index1, int index2)
        {
            while (index1 < index2)
            {
                Swap(arr, index1, index2);
                ++index1;
                --index2;
            }
        }
    }
}