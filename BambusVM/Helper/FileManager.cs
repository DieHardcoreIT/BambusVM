using dnlib.DotNet;
using System;

namespace BambusVM.Helper;

internal static class FileManager
{
    /// <summary>
    /// Prompts the user to input a file path and loads the corresponding module.
    /// </summary>
    /// <returns>
    /// A tuple containing the loaded module as a <see cref="ModuleDefMD"/> and the input file path as a <see cref="string"/>.
    /// </returns>
    public static (ModuleDefMD, string) GetInputFileAndModule()
    {
        // waiting for user input
        var inputFilePath = GetInputFilePath();

        // load the module from the file
        var module = LoadModule(inputFilePath);

        return (module, inputFilePath);
    }

    /// <summary>
    /// Prompts the user to input a file path through the console.
    /// </summary>
    /// <returns>
    /// A string representing the user-provided file path with any quotes removed.
    /// If the input is null, returns an empty string.
    /// </returns>
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

    /// <summary>
    /// Loads a .NET module from the specified file path and returns a ModuleDefMD instance.
    /// </summary>
    /// <param name="inputFilePath">The file path of the .NET assembly to be loaded.</param>
    /// <returns>
    /// Returns a ModuleDefMD object representing the loaded module if successful;
    /// otherwise, returns null in case of an error while loading.
    /// </returns>
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