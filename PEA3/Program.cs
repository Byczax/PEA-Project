using System;
using System.IO;

internal static class InfoString
{
    internal const string ErrorNoMatrix = "ERROR, No matrix created/loaded";
    internal const string GetFileName = "Write file name with extension: ";
    internal const string ErrorWrongChoice = "ERROR, Wrong choice, try again";
    internal const string Bye = "BYE!";

    internal const string Menu = "0.Exit\n" +
                                 "1.Manual testing\n" +
                                 "2.Automated Tests\n";


    internal const string ManualMenu = "0.Exit\n" +
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

    internal const string AutomatedMenu = "0. Exit\n" +
                                          "1. Read Graph From File\n" +
                                          "2. Examine Population Implact\n" +
                                          "3. Examine Mutation Rate\n" +
                                          "4. Examine Crossbreed Rate\n" +
                                          "5. Test All Above";
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
                        ProgramAutomatedMenu();
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
                            crossoverChance, false, "");
                        DisplayResults(bestPermutation);
                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }
            }
        }

        private static void ProgramAutomatedMenu()
        {
            Matrix matrix = null; // field for matrices
            var exit = false; // boolean for program work
            const long trialTime = 60;
            int population;
            double mutationChance;
            double crossoverChance;
            var correctRoad = 0;

            void PrintParamsValues()
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=========================");
                Console.WriteLine(matrix != null
                    ? $"Loaded matrix size: {matrix.Size}"
                    : "Loaded matrix size: NULL");
                Console.WriteLine(correctRoad > 0 ? $"Correct Road: {correctRoad}" : "Correct Road: NULL");
                Console.WriteLine("=========================");
                Console.ResetColor();
            }

            Action<int[], int, int> mutation;
            Func<int[], int[], int[]> crossbreed;
            while (!exit)
            {
                PrintParamsValues();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(InfoString.AutomatedMenu); // print menu
                Console.ResetColor();
                var input = int.Parse(Console.ReadLine() ?? string.Empty); // get value from user
                switch (input)
                {
                    case 0: // Program exit
                        exit = true;
                        break;
                    case 1: // Get file from user
                        Console.Write(InfoString.GetFileName);
                        matrix = Essentials.ReadFile(Console.ReadLine());
                        if (matrix != null)
                        {
                            Console.Write("Write correct road: ");
                            correctRoad = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                        }

                        break;
                    case 2: // Examine population
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        TestPopulation();
                        break;
                    case 3: // Examine mutation 
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        TestMutation();
                        break;
                    case 4: // Examine crossbreed
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        TestCrossbreed();
                        break;
                    case 5: // test all
                        (string, int)[] files = { ("tsp_17.txt", 39), ("tsp_100.txt", 36230), ("tsp_443.txt", 2720) };
                        foreach (var (item1, item2) in files)
                        {
                            matrix = Essentials.ReadFile(item1);
                            correctRoad = item2;
                            TestPopulation();
                            TestMutation();
                            TestCrossbreed();
                        }

                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }
            }

            void TestGenetic(Matrix givenMatrix, int givenPopulation, long givenTrialTime,
                Action<int[], int, int> givenMutation, double givenMutationRate,
                Func<int[], int[], int[]> givenCrossbreed,
                double givenCrossRate, string folder)
            {
                var mutationStr = givenMutation == Algorithms.Swap ? "S" : "R";
                var crossStr = givenCrossbreed == Algorithms.OrderCrossover ? "OX" : "PMX";
                var filename = "results/" + folder +
                               $"/{givenMatrix.Size}-{givenPopulation}-{givenMutationRate}-{givenCrossRate}-{mutationStr}-{crossStr}+csv";
                filename = filename.Replace(".", "_");
                filename = filename.Replace("+", ".");
                File.AppendAllText(filename, $"{correctRoad}\n\n");
                Console.WriteLine(filename);

                // for (var i = 0; i < 5; i++)
                {
                    Algorithms.GeneticAlgorithm(givenMatrix,
                        givenPopulation,
                        givenTrialTime,
                        givenMutation,
                        givenMutationRate,
                        givenCrossbreed,
                        givenCrossRate, true, filename);
                }
            }

            void TestPopulation()
            {
                int[] populationValues = { 100, 300, 500, 700, 1000 };
                mutationChance = 0.01;
                crossoverChance = 0.8;
                const string folder = "pop";
                foreach (var populationValue in populationValues)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\n" + populationValue + "\n");
                    Console.ResetColor();
                    mutation = Algorithms.Swap;
                    crossbreed = Algorithms.PartiallyMatchedCrossover;
                    Console.WriteLine("0%");
                    TestGenetic(matrix, populationValue, trialTime, mutation, mutationChance, crossbreed,
                        crossoverChance, folder);
                    mutation = Algorithms.Reverse;
                    Console.WriteLine("25%");
                    TestGenetic(matrix, populationValue, trialTime, mutation, mutationChance, crossbreed,
                        crossoverChance, folder);
                    mutation = Algorithms.Swap;
                    crossbreed = Algorithms.OrderCrossover;
                    Console.WriteLine("50%");
                    TestGenetic(matrix, populationValue, trialTime, mutation, mutationChance, crossbreed,
                        crossoverChance, folder);
                    mutation = Algorithms.Reverse;
                    Console.WriteLine("75%");
                    TestGenetic(matrix, populationValue, trialTime, mutation, mutationChance, crossbreed,
                        crossoverChance, folder);
                    Console.WriteLine("100%");
                }
            }

            void TestMutation()
            {
                double[] mutationValues = {0.05, 0.1 };
                population = 300;
                crossoverChance = 0.8;
                const string folder = "mut";
                foreach (var mutationValue in mutationValues)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\n" + mutationValue + "\n");
                    Console.ResetColor();
                    mutation = Algorithms.Swap;
                    crossbreed = Algorithms.PartiallyMatchedCrossover;
                    Console.WriteLine("0%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationValue, crossbreed,
                        crossoverChance, folder);
                    mutation = Algorithms.Reverse;
                    Console.WriteLine("25%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationValue, crossbreed,
                        crossoverChance, folder);
                    mutation = Algorithms.Swap;
                    crossbreed = Algorithms.OrderCrossover;
                    Console.WriteLine("50%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationValue, crossbreed,
                        crossoverChance, folder);
                    mutation = Algorithms.Reverse;
                    Console.WriteLine("75%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationValue, crossbreed, crossoverChance,
                        folder);
                    Console.WriteLine("100%");
                }
            }

            void TestCrossbreed()
            {
                double[] crossbreedValues = { 0.5, 0.7, 0.9 };
                population = 300;
                mutationChance = 0.1;
                const string folder = "cross";
                foreach (var crossbreedValue in crossbreedValues)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\n" + crossbreedValue + "\n");
                    Console.ResetColor();
                    mutation = Algorithms.Swap;
                    crossbreed = Algorithms.PartiallyMatchedCrossover;
                    Console.WriteLine("0%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationChance, crossbreed, crossbreedValue,
                        folder);
                    mutation = Algorithms.Reverse;
                    Console.WriteLine("25%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationChance, crossbreed, crossbreedValue,
                        folder);
                    mutation = Algorithms.Swap;
                    crossbreed = Algorithms.OrderCrossover;
                    Console.WriteLine("50%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationChance, crossbreed, crossbreedValue,
                        folder);
                    mutation = Algorithms.Reverse;
                    Console.WriteLine("75%");
                    TestGenetic(matrix, population, trialTime, mutation, mutationChance, crossbreed, crossbreedValue,
                        folder);
                    Console.WriteLine("100%");
                }
            }
        }
    }
}