using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                                     "5.Turn ON/OFF Diversification\n" +
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
                        break;
                    case 1:
                        ProgramManualMenu();
                        break;
                    case 2:
                        Console.WriteLine("TODO Automated tests");
                        // ProgramAutomatedMenu(); TODO Automated menu
                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }

                Console.WriteLine(InfoString.Bye);
            }
        }

        private static void ProgramManualMenu()
        {
            Matrix matrix = null; // field for matrices
            var exit = false; // boolean for program work
            // var timer = new Stopwatch(); // set timer
            // var numberOfTrials = 1000;
            var diversification = false;
            while (!exit)
            {
                Console.WriteLine(InfoString.ManualMenu); // print menu
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
                    case 5: // diversification
                        diversification = !diversification;
                        Console.WriteLine(diversification ? "Diversification ON" : "Diversification OFF");

                        break;
                    case 6: // Simulated Annealing Algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }
                        // var timer = new Stopwatch();
                        Console.Write("Write time for Annealing [s]: ");
                        var trialTime = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                        // timer.Restart();
                        var bestPermutation = Algorithms.SimulatedAnnealing(matrix, trialTime);
                        // timer.Stop();
                        Console.WriteLine("best permutation:");
                        Essentials.DisplayArray(bestPermutation);
                        Console.WriteLine("Road Value:\n" + matrix.CalculateRoad(bestPermutation));
                        // Console.WriteLine("Time:\n" + Essentials.CalculateTimeMs(timer) + " ms");

                        Console.WriteLine("TODO Simulated Annealing");
                        break;
                    case 7: // Tabu Search Algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        // Console.WriteLine("TODO Tabu Search");
                        Essentials.MeasureAndPrint(Algorithms.TabuSearch, matrix);
                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }
            }
        }
    }
}