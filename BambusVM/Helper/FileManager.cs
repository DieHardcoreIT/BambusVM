using dnlib.DotNet;
using System;

namespace BambusVM.Helper
{
    internal static class FileManager
    {
        public static (ModuleDefMD, string) GetInputFileAndModule()
        {
            //waiting for user input
            var inputFilePath = GetInputFilePath();

            //load the module from the file
            var module = LoadModule(inputFilePath);

            return (module, inputFilePath);
        }

        private static string GetInputFilePath()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(">");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Path: ");

            var inputFile = Console.ReadLine();

            return inputFile?.Replace("\"", "") ?? "";
        }

        private static ModuleDefMD LoadModule(string inputFilePath)
        {
            try
            {
                return ModuleDefMD.Load(inputFilePath);
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occurred while loading the module.");
                Logger.LogError(ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }

            return null;
        }
    }
}