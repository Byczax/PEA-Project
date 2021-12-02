using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PEA2
{
    public static class Essentials
    {
        public static Matrix ReadFile(string filename)
        {
            if (filename == null || !File.Exists(filename))
            {
                Console.WriteLine("ERROR, Wrong filename");
                return null;
            }

            var lines = File.ReadAllLines(filename);
            var matrix = new Matrix(int.Parse(lines[0]));
            for (var i = 1; i <= int.Parse(lines[0]); i++)
            {
                var newLine = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                for (var j = 0; j < int.Parse(lines[0]); j++) matrix.Array[i - 1, j] = int.Parse(newLine[j]);
            }

            return matrix;
        }

        public static Matrix GenerateRandomGraph(int size)
        {
            var rand = new Random();
            var matrix = new Matrix(size);
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                if (i == j)
                    matrix.Array[i, j] = 9999;
                else
                    matrix.Array[i, j] = rand.Next() % 100;
            return matrix;
        }

        public static void DisplayList(List<int> list)
        {
            Console.Write(list[0]);
            foreach (var element in list.Skip(1)) Console.Write(", " + element);

            Console.WriteLine();
        }

        private static void DisplayArray(int[] array)
        {
            Console.Write(array[0]);
            foreach (var element in array.Skip(1)) Console.Write(", " + element);

            Console.WriteLine();
        }

        public static void DisplayFullArray(int[] array)
        {
            if (array == null)
            {
                Console.WriteLine("ERROR, EMPTY, something gone WRONG!");
                return;
            }

            Console.Write(array[0]);
            foreach (var element in array.Skip(1)) Console.Write(", " + element);

            Console.WriteLine(", " + array[0]);
        }

        public static void Display0List(IEnumerable<int> list)
        {
            Console.Write("0, ");
            foreach (var element in list) Console.Write(element + ", ");
            Console.WriteLine("0");
        }

        private static double MeasureTime(Func<Matrix, IList<int>> algorithm, Matrix matrix)
        {
            var timer = new Stopwatch();
            timer.Restart();
            algorithm(matrix);
            timer.Stop();
            return CalculateTimeMs(timer); // return ms
        }

        public static void MeasureAndPrint(Func<Matrix, int[]> algorithm, Matrix matrix)
        {
            var timer = new Stopwatch();
            timer.Restart();
            var bestPermutation = algorithm(matrix);
            timer.Stop();
            Console.WriteLine("best permutation:");
            DisplayArray(bestPermutation);
            Console.WriteLine("Road Value:\n" + matrix.CalculateRoad(bestPermutation));
            Console.WriteLine("Time:\n" + CalculateTimeMs(timer) + " ms");
        }

        private static double CalculateTimeMs(Stopwatch timer)
        {
            return (double)1000 * timer.ElapsedTicks / Stopwatch.Frequency;
        }

        public static double LoopAndSum(Func<Matrix, IList<int>> algorithm, Matrix matrix, int repetition)
        {
            double time = 0;
            for (var i = 0; i <= repetition; i++)
            {
                if (i == 0) continue;
                time += MeasureTime(algorithm, matrix);
            }

            return time / repetition / 1000;
        }

        public static void SaveToFile(Matrix matrix, string filename)
        {
            File.WriteAllText(filename, matrix.Size + "\n");
            for (var i = 0; i < matrix.Size; i++)
            {
                for (var j = 0; j < matrix.Size; j++) File.AppendAllText(filename, matrix.Array[i, j] + " ");
                File.AppendAllText(filename, "\n");
            }
        }
    }
}