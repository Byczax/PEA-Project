using System;
using System.Collections.Generic;

namespace PEA_1
{
    public class Matrix
    {
        public Matrix(int size)
        {
            Array = new int[size, size];
            Size = size;
        }

        public readonly int[,] Array;

        public readonly int Size;

        public int CalculateRoad(List<int> list)
        {
            var road = 0;
            for (var i = 0; i < list.Count - 1; i++) road += Array[list[i], list[i + 1]];
            return road;
        }

        public int CalculateFullRoad(int[] list)
        {
            var road = 0;
            for (var i = 0; i < list.Length - 1; i++) road += Array[list[i], list[i + 1]];
            return Array[0, list[0]] + road + Array[list[^1], 0];
        }

        public void Display()
        {
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++) Console.Write(Array[i, j] + "\t");
                Console.WriteLine();
            }
        }
    }
}