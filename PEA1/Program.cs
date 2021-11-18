using System;
using System.Diagnostics;
using System.IO;
using PEA1;

internal static class InfoString
{
    public const string ErrorNoMatrix = "ERROR, No matrix created/loaded";
    public const string GetFileName = "Write file name with extension: ";

    public const string Menu = "0.Exit\n" +
                               "1.Read graph from file\n" +
                               "2.Create random graph\n" +
                               "3.Brute Force\n" +
                               "4.Dynamic Programming\n" +
                               "5.Branch and Bound Best First\n" +
                               "6.Branch and Bound Depth First\n" +
                               "7.Display graph\n" +
                               "8 Save graph\n";
}


namespace PEA_1
{
    internal static class Program
    {
        private static void Main()
        {
            ProgramMenu();
        }

        private static void ProgramMenu()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("0.Exit\n" +
                                  "1.Manual testing\n" +
                                  "2.Automated tests\n");
                var input = int.Parse(Console.ReadLine() ?? string.Empty);
                switch (input)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        ProgramManualMenu();
                        break;
                    case 2:
                        ProgramAutomatedMenu();
                        break;
                    default:
                        Console.WriteLine("ERROR, wrong choice, try again");
                        break;
                }

                Console.WriteLine("BYE!");
            }
        }

        private static void ProgramManualMenu()
        {
            Matrix matrix = null; // field for matrices
            var exit = false;
            var timer = new Stopwatch(); // set timer
            while (!exit)
            {
                Console.WriteLine(InfoString.Menu); // print menu
                var input = int.Parse(Console.ReadLine() ?? string.Empty); // get value from user
                switch (input)
                {
                    case 0: // exit
                        exit = true;
                        break;
                    case 1: // read given file
                        Console.Write(InfoString.GetFileName);
                        matrix = Essentials.ReadFile(Console.ReadLine());
                        break;
                    case 2: // create random graph
                        Console.Write("Write random graph size: ");
                        var graphSize = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                        matrix = Essentials.GenerateRandomGraph(graphSize);
                        break;
                    case 3: // BruteForce Algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        timer.Restart();
                        var bestPermutationBruteForce = Algorithms.TheBestBruteForce(matrix);
                        timer.Stop();
                        Console.WriteLine("best permutation:");
                        Essentials.Display0List(bestPermutationBruteForce);
                        Console.WriteLine("Road Value:\n" + matrix.CalculateFullRoad(bestPermutationBruteForce));
                        Console.WriteLine("Time:\n" + Essentials.CalculateTimeMs(timer) + " ms");
                        break;
                    case 4: // DynamicProgramming Algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        Console.WriteLine("Dynamic Programming 2 Arrays:");
                        timer.Restart();
                        var bestPermutation = Algorithms.DynamicProgramming(matrix);
                        timer.Stop();
                        Console.WriteLine("best permutation:");
                        Essentials.DisplayList(bestPermutation);
                        Console.WriteLine("Road Value:\n" + matrix.CalculateRoad(bestPermutation));
                        Console.WriteLine("Time:\n" + Essentials.CalculateTimeMs(timer) + " ms");
                        Console.WriteLine("Dynamic Programming 1 Array:");
                        timer.Restart();
                        bestPermutation = Algorithms.DynamicProgrammingOneList(matrix);
                        timer.Stop();
                        Console.WriteLine("best permutation:");
                        Essentials.DisplayList(bestPermutation);
                        Console.WriteLine("Road Value:\n" + matrix.CalculateRoad(bestPermutation));
                        Console.WriteLine("Time:\n" + Essentials.CalculateTimeMs(timer) + " ms");
                        break;
                    case 5: // Branch And Bound Best First
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        timer.Restart();
                        bestPermutation = BranchAndBound.BranchAndBoundBreathFirst(matrix);

                        timer.Stop();
                        Console.WriteLine("best permutation:");
                        Essentials.DisplayList(bestPermutation);
                        Console.WriteLine("Road Value:\n" + matrix.CalculateRoad(bestPermutation));
                        Console.WriteLine("Time:\n" + Essentials.CalculateTimeMs(timer) + " ms");
                        break;
                    case 6: // Branch and Bound Depth First
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        timer.Restart();
                        bestPermutation = BranchAndBound.BranchAndBoundDepthFirst(matrix);
                        timer.Stop();
                        Console.WriteLine("best permutation:");
                        Essentials.DisplayList(bestPermutation);
                        Console.WriteLine("Road Value:\n" + matrix.CalculateRoad(bestPermutation));
                        Console.WriteLine("Time:\n" + Essentials.CalculateTimeMs(timer) + " ms");
                        break;
                    case 7:
                        if (matrix == null)
                        {
                            Console.WriteLine("ERROR");
                            break;
                        }

                        matrix.Display();
                        break;
                    case 8:
                        Console.Write(InfoString.GetFileName);
                        var filename = Console.ReadLine();
                        Essentials.SaveToFile(matrix, filename);
                        break;
                    default:
                        Console.WriteLine("ERROR, Wrong choice");
                        break;
                }
            }
        }

        private static void ProgramAutomatedMenu()
        {
            var repetition = 100;
            const int minSize = 3;
            const int maxSizeDynamicProgramming = 20;
            const int maxSizeBranchAndBound = 16;
            const int maxSizeBruteForce = 12;

            int Percent(int maxValue, int minValue, int value)
            {
                return (value - minValue) * 100 / (maxValue - minValue);
            }

            void TestBruteForce()
            {
                for (var i = minSize; i <= maxSizeBruteForce; i++)
                {
                    for (var j = 0; j < repetition; j++)
                    {
                        File.AppendAllText("BruteForce.csv",
                            $"{i};{Essentials.MeasureTime(Algorithms.TheBestBruteForce, Essentials.GenerateRandomGraph(i))}\n");
                    }

                    Console.WriteLine($"{Percent(maxSizeBruteForce, minSize, i)}%");
                }

            }

            void TestDynamicProgramming()
            {
                for (var i = minSize; i <= maxSizeDynamicProgramming; i++)
                {
                    for (var j = 0; j < repetition; j++)
                    {
                        File.AppendAllText("DynamicProgramming.csv",
                            $"{i};{Essentials.MeasureTime(Algorithms.DynamicProgramming, Essentials.GenerateRandomGraph(i))}\n");
                    }

                    Console.WriteLine($"{Percent(maxSizeDynamicProgramming, minSize, i)}%");
                }

                Console.WriteLine("100%");
            }

            void TestBranchAndBoundBreath()
            {
                for (var i = minSize; i <= maxSizeBranchAndBound; i++)
                {
                    for (var j = 0; j < repetition; j++)
                    {
                        File.AppendAllText("BranchAndBoundBreathSearch.csv",
                            $"{i};{Essentials.MeasureTime(BranchAndBound.BranchAndBoundBreathFirst, Essentials.GenerateRandomGraph(i))}\n");
                    }

                    Console.WriteLine($"{Percent(maxSizeBranchAndBound, minSize, i)}%");
                }

                Console.WriteLine("100%");
            }

            void TestBranchAndBoundDeep()
            {
                for (var i = minSize; i <= maxSizeBranchAndBound; i++)
                {
                    for (var j = 0; j < repetition; j++)
                    {
                        File.AppendAllText("BranchAndBoundDeepSearch.csv",
                            $"{i};{Essentials.MeasureTime(BranchAndBound.BranchAndBoundDepthFirst, Essentials.GenerateRandomGraph(i))}\n");
                    }

                    Console.WriteLine($"{Percent(maxSizeBranchAndBound, minSize, i)}%");
                }

                Console.WriteLine("100%");
            }


            var exit = false;
            while (!exit)
            {
                Console.WriteLine("0.Exit\n" +
                                  "1.Set parameters for testing\n" +
                                  "2.Test Brute Force\n" +
                                  "3.Test Dynamic Programming\n" +
                                  "4.Test Branch & Bound Breath Search\n" +
                                  "5.Test Branch & Bound Deep Search\n" +
                                  "6.Test ALL");


                var input = int.Parse(Console.ReadLine() ?? string.Empty);
                switch (input)
                {
                    case 0: // exit
                        exit = true;
                        break;
                    case 1: // get data
                        Console.Write("How many times repeat:");
                        repetition = int.Parse(Console.ReadLine() ?? string.Empty);
                        break;
                    case 2: // BruteForce
                        TestBruteForce();
                        break;
                    case 3: // DynamicProgramming
                        TestDynamicProgramming();
                        break;
                    case 4: // BranchAndBound Breath
                        TestBranchAndBoundBreath();
                        break;
                    case 5: // BranchAndBound Deep
                        TestBranchAndBoundDeep();
                        break;
                    case 6: // All
                        Console.WriteLine("BruteForce");
                        TestBruteForce();
                        Console.WriteLine("DynamicProgramming");
                        TestDynamicProgramming();
                        Console.WriteLine("Branch And Bound Breath Search");
                        TestBranchAndBoundBreath();
                        Console.WriteLine("Branch And Bound Deep Search");
                        TestBranchAndBoundDeep();
                        Console.WriteLine("Completed!");
                        break;
                    default:// Wrong choice
                        Console.WriteLine("ERROR, Wrong choice");
                        break;
                }
            }
        }
    }
}