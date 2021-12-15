using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PEA3
{
    public static class Algorithms
    {
        private static readonly Random Random = new();
        private static readonly Stopwatch Timer = new(); // create stopwatch
        private static Func<int[], int[], int[]> _crossbreed;
        private static Action<int[], int, int> _mutation;
        private static int _size;

        /// <summary>
        /// Main function with genetic algorithm
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="populationSize"></param>
        /// <param name="trialTime"></param>
        /// <param name="mutation"></param>
        /// <param name="mutationRate"></param>
        /// <param name="crossFunc"></param>
        /// <param name="crossRate"></param>
        /// <returns>best found permutation</returns>
        public static int[] GeneticAlgorithm(Matrix matrix, int populationSize, long trialTime,
            Action<int[], int, int> mutation, double mutationRate, Func<int[], int[], int[]> crossFunc,
            double crossRate)
        {
            var populationList = new List<int[]>(populationSize); // create population list
            var populationValue = new int[populationSize]; // array for every population cost
            var bestRoad = int.MaxValue; // 
            _crossbreed = crossFunc; // get selected crossover algorithm
            _mutation = mutation; // get selected mutation algorithm
            _size = matrix.Size; // get global problem size for other functions

            // create population for given size
            for (var i = 0; i < populationSize; i++)
            {
                populationList.Add(GenerateRandomPermutation(matrix.Size)); // create random permutation
                populationValue[i] = matrix.CalculateFullRoad(populationList[i]); // calculate permutation value
            }

            var bestPermutation = populationList[0]; // set first permutation as best
            Timer.Restart(); // restart timer

            // run for given time by user
            while (Timer.ElapsedMilliseconds <= trialTime * 1000)
            {
                // get from population random parents
                var parents = Selection(populationSize, matrix.Size, populationValue);

                // create new generation
                var newPopulation =
                    CreateNewGeneration(parents, populationSize, populationList, crossRate, mutationRate);

                populationList = newPopulation;
                for (var i = 0; i < populationSize; i++)
                {
                    populationValue[i] = matrix.CalculateFullRoad(populationList[i]);
                    if (populationValue[i] >= bestRoad) continue;
                    bestPermutation = populationList[i];
                    bestRoad = populationValue[i];
                }
            }

            return bestPermutation;
        }

        /// <summary>
        /// Selector for new random population to change
        /// </summary>
        /// <param name="populationSize"></param>
        /// <param name="size"></param>
        /// <param name="population"></param>
        /// <returns>selected population</returns>
        private static List<int> Selection(int populationSize, int size, IReadOnlyList<int> population)
        {
            var parents = new List<int>(size); // list of parents indexes

            // get parents equal to population size
            for (var i = 0; i < populationSize; i++)
            {
                var index1 = Random.Next(1, populationSize);
                int index2;
                while (true)
                {
                    index2 = Random.Next(1, populationSize);
                    if (index1 != index2)
                        break;
                }

                // get better index
                var minIndex = population[index1] < population[index2] ? index1 : index2;

                // save index into list
                parents.Add(minIndex);
            }

            return parents;
        }

        /// <summary>
        /// Change, mutate or crossbreed population
        /// </summary>
        /// <param name="parents"></param>
        /// <param name="populationSize"></param>
        /// <param name="population"></param>
        /// <param name="crossRate"></param>
        /// <param name="mutationRate"></param>
        /// <returns>changed population</returns>
        private static List<int[]> CreateNewGeneration(IReadOnlyList<int> parents, int populationSize,
            IReadOnlyList<int[]> population,
            double crossRate, double mutationRate)
        {
            var newPopulation = new List<int[]>(populationSize); // new population

            // do for every population
            for (var i = 0; i < populationSize; i += 2)
            {
                var parent1 = population[parents[i]]; // get first parent
                var parent2 = population[parents[i + 1]]; // get second parent
                int[] child1;
                int[] child2;
                var crossoverChance = Random.NextDouble(); // value <0,1>
                if (crossoverChance < crossRate) // do crossbreed
                {
                    child1 = _crossbreed(parent1, parent2);
                    child2 = _crossbreed(parent2, parent1);
                }
                else // or add unchanged parents
                {
                    child1 = parent1;
                    child2 = parent2;
                }

                var mutationChance = Random.Next(); // value <0,1>
                // mutation chance for first child
                if (mutationChance < mutationRate)
                    DoMutation(child1, _mutation);

                mutationChance = Random.Next(); // value <0,1>
                // mutation chance for second child
                if (mutationChance < mutationRate)
                    DoMutation(child2, _mutation);

                newPopulation.Add(child1); // add first changed child
                newPopulation.Add(child2); // add second changed child
            }

            return newPopulation;
        }

        /// <summary>
        /// Function for generating random index for mutation
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="mutation"></param>
        private static void DoMutation(int[] arr, Action<int[], int, int> mutation)
        {
            var index1 = Random.Next(1, arr.Length); // get first random index
            int index2;
            // get second random index, different from first
            while (true)
            {
                index2 = Random.Next(1, arr.Length);
                if (index1 != index2)
                    break;
            }

            // do mutation for random indexes
            mutation(arr, index1, index2);
        }

        public static int[] PartiallyMatchedCrossover(int[] parent1, int[] parent2)
        {
            var startPoint = Random.Next(1, parent1.Length - 2);
            var finishPoint = Random.Next(1, parent1.Length - 2);

            if (startPoint > finishPoint) // if start i bigger than finish := swap.
                (startPoint, finishPoint) = (finishPoint, startPoint);

            var child = new int[_size];
            var map = new int[_size];

            parent1.CopyTo(child, 0);

            // create genes map
            for (var i = 0; i < _size; i++)
            {
                map[child[i]] = i;
            }

            // mutate from start to finish point in child
            for (var i = startPoint; i < finishPoint; i++)
            {
                var value = parent2[i];
                Swap(child, i, map[value]);
                Swap(map, child[i], child[map[value]]);
            }

            return child;
        }

        public static int[] OrderCrossover(int[] parent1, int[] parent2)
        {
            // maybe length and start point depends on parent
            const int length = 4; // set copied fragment length
            const int startPoint = 4;


            var child = new int[_size]; // new child
            var used = new bool[_size]; // array for used fields, they will be skipped
            int i;

            // copy selected fragment from parent
            for (i = startPoint; i < startPoint + length; i++)
            {
                child[i] = parent1[i];
                used[parent1[i]] = true;
            }

            var j = startPoint + length;
            // iterate every non-used field
            while (i != startPoint)
            {
                // if field is not used
                if (!used[parent2[j]])
                {
                    child[i] = parent2[j]; // get unused field
                    i++; // move to the next
                }

                j++;

                if (i >= _size) // if we get out of bounds
                    i = 1; // back to beginning

                if (j >= _size) // if we get out of bounds
                    j = 1; // back to beginning
            }

            return child;
        }

        /// <summary>
        /// Generate random permutation for population
        /// </summary>
        /// <param name="size"></param>
        /// <returns>generated random permutation</returns>
        private static int[] GenerateRandomPermutation(int size)
        {
            var permutation = Enumerable.Range(0, size).ToArray();
            Shuffle(permutation);
            return permutation;
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
        /// Swap mutator
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
        /// Reverse mutator
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