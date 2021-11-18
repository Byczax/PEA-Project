using System;
using System.Collections.Generic;

namespace PEA1
{
    public static class Algorithms
    {
        private static Matrix _matrix;
        private static int _bestRoad;
        private static int[] _bestPermutation;

        private static void GetPer(int[] list, int k, int m)
        {
            if (k == m)
            {
                var road = _matrix.CalculateFullRoad(list);
                if (road >= _bestRoad) return;
                _bestRoad = road;
                _bestPermutation = (int[])list.Clone();
            }
            else
            {
                for (var i = k; i <= m; ++i)
                {
                    var temp = list[k];
                    list[k] = list[i];
                    list[i] = temp;

                    GetPer(list, k + 1, m);

                    temp = list[k];
                    list[k] = list[i];
                    list[i] = temp;
                }
            }
        }

        public static int[] TheBestBruteForce(Matrix givenMatrix)
        {
            _matrix = givenMatrix;
            var permutation = new int[_matrix.Size - 1];
            _bestRoad = int.MaxValue;
            _bestPermutation = new int[_matrix.Size];
            for (var i = 1; i < _matrix.Size; ++i) permutation[i - 1] = i;

            GetPer(permutation, 0, permutation.Length - 1);
            return _bestPermutation;
        }

        public static List<int> DynamicProgramming(Matrix givenMatrix)
        {
            _matrix = givenMatrix;

            int DpGetMinimum(Matrix graph, int[,] solutions, int[,] vertices,
                int bitMask, int vertex)
            {
                // if problem was solved earlier, we get this value
                if (solutions[bitMask, vertex] != 0) return solutions[bitMask, vertex];

                var min = int.MaxValue;
                for (var i = 1; i < graph.Size; i++)
                {
                    // check if city is already visited
                    var iPow = 1 << i; // bit shift
                    var logicAnd = bitMask & iPow;
                    if (logicAnd == 0) continue; // if not visited, get deeper
                    var newMin = graph.Array[vertex, i] +
                                 DpGetMinimum(graph, solutions, vertices, bitMask - iPow, i);
                    if (newMin >= min) continue; // if new road is better, save
                    min = newMin;
                    // save road
                    vertices[bitMask, vertex] = i;
                }

                // save solution
                solutions[bitMask, vertex] = min;
                return min;
            }

            void DpReturnPath(ICollection<int> bestPermutationList, int[,] vertices,
                int bitMask, int vertex)
            {
                while (true)
                {
                    var nextVertex = vertices[bitMask, vertex]; // get vertex
                    bestPermutationList.Add(nextVertex); // save on list
                    bitMask -= 1 << nextVertex; // move to the next
                    if (bitMask != 0) // if we get all vertices, stop loop
                    {
                        vertex = nextVertex;
                        continue;
                    }

                    break;
                }
            }

            // better than Math.pow(), the same as 2^(matrix.Size);
            var pow = 1 << _matrix.Size;

            // create help variables, pow - 1 because we dont need city nr. 0
            var solutions = new int[pow - 1, _matrix.Size];
            var vertices = new int[pow - 1, _matrix.Size];
            var bestPermutationList = new List<int>(_matrix.Size);

            // trivial problems, set every vertex as first in every combination
            for (var i = 1; i < _matrix.Size; i++) solutions[0, i] = _matrix.Array[i, 0];
            bestPermutationList.Add(0); // add 0 because we start here
            DpGetMinimum(_matrix, solutions, vertices, pow - 2, 0);
            DpReturnPath(bestPermutationList, vertices, pow - 2, 0);
            bestPermutationList.Add(0); // add 0 because we end here
            return bestPermutationList;
        }

        public static List<int> DynamicProgrammingOneList(Matrix givenMatrix)
        {
            _matrix = givenMatrix;

            int DpGetMinimum(Matrix graph, Tuple<int, int>[,] solutions,
                int bitMask, int vertex)
            {
                // if problem was solved earlier, we get this value
                if (solutions[bitMask, vertex] != null) return solutions[bitMask, vertex].Item1;

                var min = int.MaxValue;
                for (var i = 1; i < graph.Size; i++)
                {
                    // check if city is already visited
                    var iPow = 1 << i; // bit shift
                    var logicAnd = bitMask & iPow;
                    if (logicAnd == 0) continue; // if not visited, get deeper
                    var newMin = graph.Array[vertex, i] + DpGetMinimum(graph, solutions, bitMask - iPow, i);
                    if (newMin >= min) continue; // if new road is better, save
                    min = newMin;
                    // save road
                    solutions[bitMask, vertex] = new Tuple<int, int>(0, i);
                    // vertices[bitMask, vertex] = i;
                }

                solutions[bitMask, vertex] = new Tuple<int, int>(min, solutions[bitMask, vertex].Item2);
                // save solution
                // solutions[bitMask, vertex] = min;
                return min;
            }

            void DpReturnPath(ICollection<int> bestPermutationList, Tuple<int, int>[,] vertices,
                int bitMask, int vertex)
            {
                while (true)
                {
                    var nextVertex = vertices[bitMask, vertex].Item2; // get vertex
                    bestPermutationList.Add(nextVertex); // save on list
                    bitMask -= 1 << nextVertex; // move to the next
                    if (bitMask != 0) // if we get all vertices, stop loop
                    {
                        vertex = nextVertex;
                        continue;
                    }

                    break;
                }
            }

            // tuples[0, 0] = Tuple.Create(1,1);
            // better than Math.pow(), the same as 2^(matrix.Size);
            var pow = 1 << _matrix.Size;
            var solutions = new Tuple<int, int>[pow - 1, _matrix.Size];

            // create help variables, pow - 1 because we dont need city nr. 0
            // var solutions = new int[pow - 1, _matrix.Size];
            // var vertices = new int[pow - 1, _matrix.Size];
            var bestPermutationList = new List<int>(_matrix.Size);

            // trivial problems, set every vertex as first in every combination
            for (var i = 1; i < _matrix.Size; i++) solutions[0, i] = new Tuple<int, int>(_matrix.Array[i, 0], 0);
            bestPermutationList.Add(0); // add 0 because we start here
            DpGetMinimum(_matrix, solutions, pow - 2, 0);
            DpReturnPath(bestPermutationList, solutions, pow - 2, 0);
            bestPermutationList.Add(0); // add 0 because we end here
            return bestPermutationList;
        }
    }
}