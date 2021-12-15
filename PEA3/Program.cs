using System;
using System.IO;

internal static class InfoString
{
    public const string ErrorNoMatrix = "ERROR, No matrix created/loaded";
    public const string GetFileName = "Write file name with extension: ";
    public const string ErrorWrongChoice = "ERROR, Wrong choice, try again";
    public const string Bye = "BYE!";

    public const string Menu = "0.Exit\n" +
                               "1.Manual testing\n" +
                               "2.Automated Tests\n";


    public const string ManualMenu = "0.Exit\n" +
                                     "1.Read Graph From File\n" +
                                     "2.Create Random Graph\n" +
                                     "3.Display Graph\n" +
                                     "4 Save Graph\n" +
                                     "5.Change Time\n" +
                                     "6.Change Population Size\n" +
                                     "7.Set Mutation Rate\n" +
                                     "8.Set Crossbreed Rate\n" +
                                     "9.Change Mutation Method\n" +
                                     "10.Change Crossbread Method\n" +
                                     "11.Run Algorithm\n";
}

namespace PEA3
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
                Console.WriteLine(InfoString.Menu);
                var input = int.Parse(Console.ReadLine() ?? string.Empty);
                switch (input)
                {
                    case 0:
                        exit = true;
                        Console.WriteLine(InfoString.Bye);
                        break;
                    case 1:
                        ProgramManualMenu();
                        break;
                    case 2:
                        Console.WriteLine("TODO Automated");
                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }
            }
        }

        private static void ProgramManualMenu()
        {
            Matrix matrix = null; // field for matrices
            var exit = false; // boolean for program work
            long trialTime = 1;
            var population = 10;
            var mutationChance = 0.01;
            var crossoverChance = 0.8;
            Action<int[], int, int> mutation = Algorithms.Swap;
            Func<int[], int[], int[]> crossbreed = Algorithms.PartiallyMatchedCrossover;

            void DisplayResults(int[] array)
            {
                Console.WriteLine("best permutation:");
                Essentials.DisplayFullArray(array);
                Console.WriteLine("Road Value:\n" + matrix.CalculateFullRoad(array));
            }

            void PrintParamsValues()
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=========================");
                Console.WriteLine($"Time [s]: {trialTime}");
                Console.WriteLine($"Population Size: {population}");
                Console.WriteLine($"Mutation Rate: {mutationChance}");
                Console.WriteLine($"Crossover Rate: {crossoverChance}");
                Console.Write("Mutation: ");
                Console.WriteLine(mutation == Algorithms.Swap ? "SWAP" : "REVERSE");
                Console.Write("Crossover: ");
                Console.WriteLine(crossbreed == Algorithms.PartiallyMatchedCrossover ? "PMX" : "OX");

                Console.WriteLine(matrix != null
                    ? $"Loaded matrix size: {matrix.Size}"
                    : "Loaded matrix size: NULL");
                Console.WriteLine("=========================");
                Console.ResetColor();
            }

            while (!exit)
            {
                PrintParamsValues();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(InfoString.ManualMenu); // print menu
                Console.ResetColor();
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
                    case 3: // Display matrix
                        if (matrix == null)
                        {
                            Console.WriteLine("ERROR");
                            break;
                        }

                        matrix.Display();
                        break;
                    case 4: // save to file
                        Console.Write(InfoString.GetFileName);
                        var filename = Console.ReadLine();
                        Essentials.SaveToFile(matrix, filename);
                        break;
                    case 5: // change time
                        Console.Write("Write wait time [s]: ");
                        trialTime = long.Parse(Console.ReadLine() ?? "1");
                        break;
                    case 6: // change start population
                        Console.Write("Write permutation size (even value): ");
                        population = int.Parse(Console.ReadLine() ?? "10");
                        if (population % 2 != 0)
                        {
                            Console.WriteLine("Error,odd population value, fix by adding 1.");
                            population += 1;
                        }

                        break;
                    case 7: // change mutation chance
                        mutationChance = double.Parse(Console.ReadLine() ?? "0");
                        break;
                    case 8: // change crossbreed chance
                        crossoverChance = double.Parse(Console.ReadLine() ?? "100");
                        break;
                    case 9: // change mutation method
                        mutation = mutation == Algorithms.Swap ? Algorithms.Reverse : Algorithms.Swap;
                        break;
                    case 10: // change crossbreed method
                        crossbreed = crossbreed == Algorithms.PartiallyMatchedCrossover
                            ? Algorithms.OrderCrossover
                            : Algorithms.PartiallyMatchedCrossover;
                        break;
                    case 11: //genetic algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        var bestPermutation = Algorithms.GeneticAlgorithm(matrix,
                            population,
                            trialTime,
                            mutation,
                            mutationChance,
                            crossbreed,
                            crossoverChance);
                        DisplayResults(bestPermutation);
                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }
            }
        }
    }
}