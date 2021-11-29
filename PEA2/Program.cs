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
                               "2.Automated tests\n";


    public const string ManualMenu = "0.Exit\n" +
                                     "1.Read graph from file\n" +
                                     "2.Create random graph\n" +
                                     "3.Display graph\n" +
                                     "4 Save graph\n" +
                                     "5.Change parameters (Time, Neighbourhood, Diversification)\n" +
                                     "6.Simulated Annealing\n" +
                                     "7.Tabu Search\n";
}

namespace PEA2
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
            var diversification = false;
            Action<int[], int, int> neighbourhood = Algorithms.Swap;
            var trialTime = 1.0;

            void DisplayResults(int[] array)
            {
                Console.WriteLine("best permutation:");
                Essentials.DisplayFullArray(array);
                Console.WriteLine("Road Value:\n" + matrix.CalculateFullRoad(array));
            }

            void PrintParamsValues()
            {
                Console.WriteLine("=========================");
                Console.WriteLine($"Time [s]: {trialTime}");
                Console.Write("Neighbourhood: ");
                Console.WriteLine(neighbourhood == Algorithms.Swap ? "SWAP" : "REVERSE");
                Console.Write("Diversification: ");
                Console.WriteLine(diversification ? "ON" : "OFF");
                Console.WriteLine(matrix != null
                    ? $"Loaded matrix size: {matrix.Size}"
                    : "Loaded matrix size: NULL");
                Console.WriteLine("=========================");
            }


            while (!exit)
            {
                PrintParamsValues();
                Console.WriteLine(InfoString.ManualMenu); // print menu
                var input = int.Parse(Console.ReadLine() ?? string.Empty); // get value from user
                int[] bestPermutation;
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
                    case 5: // change params
                        Console.Write("Write wait time [s]: ");
                        trialTime = double.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                        Console.Write("Neighbourhood ");
                        Console.WriteLine(neighbourhood == Algorithms.Swap ? "SWAP" : "REVERSE");
                        Console.WriteLine("Change?: (write something)");
                        if (Console.ReadLine() != null)
                        {
                            if (neighbourhood == Algorithms.Swap)
                                neighbourhood = Algorithms.Reverse;
                            else
                                neighbourhood = Algorithms.Swap;
                        }

                        Console.Write("Diversification ");
                        Console.WriteLine(diversification ? "ON" : "OFF");
                        Console.WriteLine("Change?: (write something)");
                        if (Console.ReadLine() != null) diversification = !diversification;


                        break;
                    case 6: // Simulated Annealing Algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        bestPermutation = Algorithms.SimulatedAnnealing(matrix, trialTime, neighbourhood);
                        DisplayResults(bestPermutation);
                        break;
                    case 7: // Tabu Search Algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        bestPermutation = Algorithms.TabuSearch(matrix, trialTime, neighbourhood, diversification);
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
            var diversification = false;
            Action<int[], int, int> neighbourhood = Algorithms.Swap;
            var trialTime = 1;
            const string filename = "results.csv";
            const int repeatTime = 5;

            var correctRoad = 0;

            void PrintParamsValues()
            {
                Console.WriteLine("=========================");
                Console.WriteLine($"Time [s]: {trialTime}");
                Console.Write("Neighbourhood: ");
                Console.WriteLine(neighbourhood == Algorithms.Swap ? "SWAP" : "REVERSE");
                Console.Write("Diversification: ");
                Console.WriteLine(diversification ? "Diversification: ON" : "Diversification: OFF");
                Console.WriteLine(matrix != null
                    ? $"Loaded matrix size: {matrix.Size}"
                    : "Loaded matrix size: NULL");
                Console.WriteLine("Correct Road: ");
                Console.WriteLine("=========================");
            }

            void PrintToFileData(int road, int time)
            {
                var relativeError = (road - correctRoad) / (correctRoad * 1.0);
                File.AppendAllText(filename, $"Algorithm:TabuSearch;Time[s]:{time};Neighbourhood:");
                File.AppendAllText(filename, neighbourhood == Algorithms.Swap ? "SWAP" : "REVERSE");
                File.AppendAllText(filename, diversification ? ";Diversification:ON" : ";Diversification:OFF");
                File.AppendAllText(filename,
                    $";Road:{road};BestRoad:{correctRoad};RelativeError[%]:{relativeError}\n");
            }

            void TestTabuSearchAlgorithm()
            {
                if (matrix == null) return;
                trialTime = matrix.Size switch
                {
                    < 20 => 1,
                    < 100 => 2,
                    _ => 10
                };
                for (var i = trialTime; i <= trialTime * 5; i += trialTime)
                {
                    for (var j = 0; j < repeatTime; j++)
                    {
                        var road = Algorithms.TabuSearch(matrix, i, neighbourhood, diversification);
                        var roadValue = matrix.CalculateFullRoad(road);
                        PrintToFileData(roadValue, i);
                    }
                }
            }

            void TestAnnealingAlgorithm()
            {
                if (matrix == null) return;
                trialTime = matrix.Size switch
                {
                    < 20 => 1,
                    < 100 => 2,
                    _ => 10
                };
                for (var i = trialTime; i <= trialTime * 5; i += trialTime)
                {
                    for (var j = 0; j < repeatTime; j++)
                    {
                        var road = Algorithms.SimulatedAnnealing(matrix, i, neighbourhood);
                        var roadValue = matrix.CalculateFullRoad(road);
                        PrintToFileData(roadValue, i);
                    }
                }
            }

            while (!exit)
            {
                PrintParamsValues();
                Console.WriteLine("0.Exit\n" +
                                  "1.Read file and set time\n" +
                                  "2. Test Annealing\n" +
                                  "3. Test Tabu Search\n"); // print menu
                var input = int.Parse(Console.ReadLine() ?? string.Empty); // get value from user
                switch (input)
                {
                    case 0: // exit
                        exit = true;
                        break;
                    case 1: // read given file
                        Console.Write(InfoString.GetFileName);
                        matrix = Essentials.ReadFile(Console.ReadLine());
                        Console.Write("Write correct road: ");
                        correctRoad = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                        // Console.Write("Write wait time [s]: ");
                        // trialTime = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

                        break;
                    case 2: // test Annealing
                        neighbourhood = Algorithms.Swap;
                        TestAnnealingAlgorithm();
                        neighbourhood = Algorithms.Reverse;
                        TestAnnealingAlgorithm();
                        break;
                    case 3:
                        neighbourhood = Algorithms.Swap;
                        diversification = false;
                        TestTabuSearchAlgorithm();
                        neighbourhood = Algorithms.Reverse;
                        TestTabuSearchAlgorithm();
                        neighbourhood = Algorithms.Swap;
                        diversification = true;
                        TestTabuSearchAlgorithm();
                        neighbourhood = Algorithms.Reverse;
                        TestTabuSearchAlgorithm();
                        break;


                    // Console.Write("Write wait time [s]: ");
                    // trialTime = double.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                    // Console.Write("Neighbourhood ");
                    // Console.WriteLine(neighbourhood == Algorithms.Swap ? "SWAP" : "REVERSE");
                    // Console.WriteLine("Change?: (write something)");
                    // if (Console.ReadLine() != null)
                    // {
                    //     if (neighbourhood == Algorithms.Swap)
                    //         neighbourhood = Algorithms.Reverse;
                    //     else
                    //         neighbourhood = Algorithms.Swap;
                    // }
                    //
                    // Console.Write("Diversification ");
                    // Console.WriteLine(diversification ? "ON" : "OFF");
                    // Console.WriteLine("Change?: (write something)");
                    // if (Console.ReadLine() != null) diversification = !diversification;
                }
            }
        }
    }
}