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
                                     "5.Change Time\n" +
                                     "6.Change Start Population\n" +
                                     "7.Set Mutation Index\n" +
                                     "8.Set Crossbreed Index\n" +
                                     "9.Set Mutation Method\n" +
                                     "10.Run Algorithm\n";
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

            void DisplayResults(int[] array)
            {
                Console.WriteLine("best permutation:");
                Essentials.DisplayFullArray(array);
                Console.WriteLine("Road Value:\n" + matrix.CalculateFullRoad(array));
            }
            
            while (!exit)
            {
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
                    case 5: // change time
                        break;
                    case 6:// change start population
                        break;
                    case 7:// change mutation index
                        break;
                    case 8:// change crossbreed index
                        break;
                    case 9:// set mutation method
                        break;
                    case 10://genetic algorithm
                        if (matrix == null)
                        {
                            Console.WriteLine(InfoString.ErrorNoMatrix);
                            break;
                        }

                        Console.WriteLine("TODO");
                        break;
                    default:
                        Console.WriteLine(InfoString.ErrorWrongChoice);
                        break;
                }
            }
        }
    }
}