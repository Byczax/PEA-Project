using System;
using System.Collections.Generic;
using Medallion.Collections;

// using PEA_1;

namespace PEA1
{
    /// <summary>
    ///     Class for Branch and Bound Deep First Search
    /// </summary>
    public static class BranchAndBound
    {
        /// <summary>
        ///     Get Upper Bound for matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>matrix upper bound</returns>
        private static int GetUpperBound(Matrix matrix)
        {
            var upperBound = 0;
            var vertex = 0;
            var bitMask = 1;

            for (var i = 0; i < matrix.Size - 1; i++) // iterate for every vertex in matrix
            {
                var min = int.MaxValue;
                var newVertex = 0;
                for (var j = 0; j < matrix.Size; j++)
                {
                    // if vertex is already visited or vertex connection is bigger than min - skip
                    if (((1 << j) & bitMask) != 0 || matrix.Array[vertex, j] >= min) continue;
                    min = matrix.Array[vertex, j];
                    newVertex = j;
                }

                upperBound += min;
                vertex = newVertex;
                bitMask |= 1 << newVertex; // save visited vertex
            }

            upperBound += matrix.Array[vertex, 0] + 1; // always end where started
            return upperBound;
        }

        /// <summary>
        ///     Get Lower Bound for vertex
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vertex"></param>
        /// <returns>vertex lower bound</returns>
        private static int GetLowerBound(Matrix matrix, int vertex)
        {
            var minOut = int.MaxValue;
            var minIn = int.MaxValue;
            for (var i = 0; i < matrix.Size; i++)
            {
                if (matrix.Array[i, vertex] < minIn) minIn = matrix.Array[i, vertex];

                if (matrix.Array[vertex, i] < minOut) minOut = matrix.Array[vertex, i];
            }

            return (minIn + minOut) / 2;
        }

        /// <summary>
        ///     Branch and Bound Depth First algorithm
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>best permutation path</returns>
        public static List<int> BranchAndBoundDepthFirst(Matrix matrix)
        {
            var stack = new Stack<BnBVertex>();
            stack.Push(new BnBVertex(matrix, 0, 0)); // put root on stack
            var bestPermutation = new List<int>(matrix.Size + 1);

            var upperBound = GetUpperBound(matrix);

            while (stack.TryPop(out var vertex)) // try get vertex from stack
                // vertex.LowerBound = GetLowerBound(matrix, vertex);
                if (vertex.Vertices.Count == matrix.Size) // if we have complete road
                {
                    vertex.Vertices.Add(0); // add to end 0
                    var newRoad = matrix.CalculateRoad(vertex.Vertices);
                    // check if lower and upper bound are fulfilled
                    if (newRoad >= upperBound) continue;
                    upperBound = newRoad;
                    bestPermutation = vertex.Vertices;
                }
                else
                {
                    for (var i = 1; i < matrix.Size; i++)
                    {
                        // if vertex is visited - skip
                        if (((1 << i) & vertex.BitMask) != 0) continue;
                        // get next vertex
                        var childVertex = new BnBVertex(matrix, i, vertex.BitMask | (1 << i),
                            vertex.Vertices);
                        var bound = matrix.CalculateRoad(childVertex.Vertices); // calculate road
                        if (bound + vertex.LowerBound < upperBound) // if road for vertex is within bound
                            stack.Push(childVertex); // add to stack
                    }
                }

            return bestPermutation;
        }

        /// <summary>
        ///     Branch and Bound Breath first algorithm
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>best permutation path</returns>
        public static List<int> BranchAndBoundBreathFirst(Matrix matrix)
        {
            var queue = new PriorityQueue<BnBVertexPriority>(); // create priority Queue
            queue.Enqueue(new BnBVertexPriority(matrix, 0, 0, 0)); // enqueue root
            var bestPermutation = new List<int>(matrix.Size + 1);
            var upperBound = GetUpperBound(matrix); // get upper bound for matrix

            while (queue.Count != 0) // do while vertices in queue
            {
                var vertex = queue.Dequeue();
                // vertex.LowerBound = GetLowerBound(matrix, vertex);
                if (vertex.Vertices.Count == matrix.Size)
                {
                    vertex.Vertices.Add(0);
                    var newRoad = matrix.CalculateRoad(vertex.Vertices);
                    if (newRoad >= upperBound) continue;
                    upperBound = newRoad;
                    bestPermutation = vertex.Vertices;
                }
                else
                {
                    for (var i = 1; i < matrix.Size; i++)
                    {
                        if (((1 << i) & vertex.BitMask) != 0) continue;
                        var newRoad = vertex.RoadValue + matrix.Array[vertex.Vertices[^1], i];
                        var childVertex = new BnBVertexPriority(matrix, i, vertex.BitMask | (1 << i), newRoad,
                            vertex.Vertices);
                        if (newRoad + vertex.LowerBound < upperBound) queue.Enqueue(childVertex);
                    }
                }
            }

            return bestPermutation;
        }

        private class BnBVertex
        {
            public readonly int BitMask;
            public readonly int LowerBound;
            public readonly List<int> Vertices;

            public BnBVertex(Matrix matrix, int newVertex, int bitMask, List<int> vertices = null)
            {
                Vertices = new List<int>(matrix.Size);
                if (vertices != null) // if vertex is not the root
                    foreach (var vertex in vertices) // get all parent vertices
                        Vertices.Add(vertex);

                LowerBound = GetLowerBound(matrix, newVertex);

                Vertices.Add(newVertex); // add start vertex
                BitMask = bitMask; // save visited vertices bitMask
            }
        }

        /// <summary>
        ///     Class for Branch And Bound Breath Search
        /// </summary>
        private class BnBVertexPriority : IComparable<BnBVertexPriority>
        {
            public readonly int BitMask;
            public readonly int LowerBound;
            public readonly int RoadValue; // road value for IComparable interface
            public readonly List<int> Vertices;

            public BnBVertexPriority(Matrix matrix, int newVertex, int bitMask, int roadValue,
                List<int> vertices = null)
            {
                Vertices = new List<int>(matrix.Size);
                if (vertices != null) // if vertex is not the root
                    // Vertices = new List<int>(vertices);
                    foreach (var vertex in vertices) // get all parent vertices
                        Vertices.Add(vertex);

                LowerBound = GetLowerBound(matrix, newVertex);

                RoadValue = roadValue; // save actual road value

                Vertices.Add(newVertex); // add start vertex
                BitMask = bitMask; // save visited vertices bitMask
            }

            public int CompareTo(BnBVertexPriority other) // set priority for Queue
            {
                return other.RoadValue - RoadValue;
            }
        }
    }
}