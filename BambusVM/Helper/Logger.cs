using System;

namespace BambusVM.Helper;

internal class Logger
{
    internal static void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("X");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("] ");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(message);
    }

    internal static void LogInfo(object message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("i");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("] ");

        Console.WriteLine(message);
    }

    internal static void LogGood(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("+");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("] ");

        Console.WriteLine(message);
    }

    internal static void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("!");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("] ");

        Console.WriteLine(message);
    }
}