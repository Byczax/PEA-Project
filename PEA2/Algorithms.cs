using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PEA2
{
    public static class Algorithms
    {
        private static int _bestRoad;
        private static int[] _bestPermutation;
        private static readonly Random Random = new();
        private static readonly Stopwatch Timer = new(); // create stopwatch

        /// <summary>
        ///     Tabu Search Algorithm
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="trialTime"></param>
        /// <param name="neighbour"></param>
        /// <param name="diversification"></param>
        /// <param name="testIt"></param>
        /// <param name="testFileName"></param>
        /// <returns>the best solution found</returns>
        public static int[] TabuSearch(Matrix matrix, double trialTime, Action<int[], int, int> neighbour,
            bool diversification, bool testIt, string testFileName = "")
        {
            void DecrementList(int[,] list)
            {
                var size = list.GetLength(0);
                for (var i = 1; i < size; i++)
                for (var j = i + 1; j < size; j++)
                    if (list[i, j] > 0)
                        --list[i, j];
            }

            var permutation = Enumerable.Range(0, matrix.Size).ToArray(); // set start solution
            _bestPermutation = (int[])permutation.Clone(); // set start as best
            var currentRoad = _bestRoad = matrix.CalculateFullRoad(permutation); // calculate generated road
            var currentPermutation = new int[matrix.Size];

            var tabuList = new int[matrix.Size, matrix.Size];
            var cadence = Convert.ToInt32(Math.Sqrt(matrix.Size)); //user input??
            var criticalEvents = 0;
            var minValue = _bestRoad; // value for result display

            trialTime *= 1000; // change to ms
            Timer.Restart(); // start timer
            if (testIt)
                File.AppendAllText("results/" + testFileName + ".csv",
                    "Time[ticks];Local value;Global value\n");

            while (Timer.ElapsedMilliseconds <= trialTime)
            {
                var bestI = 0;
                var bestJ = 0;
                var previousRoad = currentRoad;
                if (testIt)
                    File.AppendAllText("results/" + testFileName + ".csv",
                        $"{Timer.ElapsedTicks};{minValue};{_bestRoad}\n");

                for (var i = 1; i < matrix.Size; i++)
                for (var j = i + 1; j < matrix.Size; j++)
                {
                    currentPermutation = (int[])permutation.Clone();
                    neighbour(currentPermutation, i, j);
                    var actualRoad = matrix.CalculateFullRoad(currentPermutation);

                    if (actualRoad >= _bestRoad && (actualRoad >= currentRoad || tabuList[i, j] != 0)) continue;
                    currentRoad = actualRoad;
                    bestI = i;
                    bestJ = j;
                }

                neighbour(permutation, bestI, bestJ); // save best local permutation
                tabuList[bestI, bestJ] = cadence; // set permutation in tabuList

                DecrementList(tabuList);

                if (testIt) minValue = previousRoad;

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
                    currentRoad = matrix.CalculateFullRoad(currentPermutation);
                    tabuList = new int[matrix.Size, matrix.Size];
                }
            }

            return _bestPermutation;
        }

        /// <summary>
        ///     Simulated Annealing algorithm
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="trialTime"></param>
        /// <param name="neighbour"></param>
        /// <param name="testIt"></param>
        /// <param name="testFileName"></param>
        /// <returns>the best solution found</returns>
        public static int[] SimulatedAnnealing(Matrix matrix, double trialTime, Action<int[], int, int> neighbour,
            bool testIt,
            string testFileName = "")
        {
            static bool AnnealingFunction(int newCost, double temperature)
            {
                return Random.NextDouble() < Math.Exp(-(newCost - _bestRoad) / temperature);
            }

            var activePermutation = Enumerable.Range(0, matrix.Size).ToArray(); // set start solution
            _bestPermutation = (int[])activePermutation.Clone(); // set start as best
            var localMinimumRoad = _bestRoad = matrix.CalculateFullRoad(activePermutation);
            var localMinimumPermutation = (int[])activePermutation.Clone();
            double temperature = _bestRoad * matrix.Size; // set temperature
            const double alpha = 0.99; //think about other method for cooling

            var minValue = _bestRoad; // test display value

            trialTime *= 1000; // change to ms
            Timer.Restart(); // start timer

            if (testIt)
                File.AppendAllText("results/" + testFileName + ".csv",
                    "Time[ticks];Local value;Global value\n");

            while (Timer.ElapsedMilliseconds <= trialTime)
            {
                if (testIt)
                {
                    File.AppendAllText("results/" + testFileName + ".csv",
                        $"{Timer.ElapsedTicks};{minValue};{_bestRoad}\n");
                    minValue = int.MaxValue;
                }

                for (var i = 0; i < 10 * matrix.Size; i++)
                {
                    activePermutation = (int[])localMinimumPermutation.Clone();
                    var randomFirstVertex = Random.Next(1, matrix.Size); // get first random vertex (skip 0)

                    int randomSecondVertex;
                    while (true) // get second random vertex different from first (skip 0)
                    {
                        randomSecondVertex = Random.Next(1, matrix.Size);
                        if (randomFirstVertex != randomSecondVertex)
                            break;
                    }

                    neighbour(activePermutation, randomFirstVertex, randomSecondVertex);

                    var actualRoad = matrix.CalculateFullRoad(activePermutation); // calculate road with end in 0
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

                    if (minValue > actualRoad)
                        minValue = actualRoad;
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