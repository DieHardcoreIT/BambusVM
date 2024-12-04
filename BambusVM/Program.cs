using BambusVM.Helper;
using BambusVM.VM;
using dnlib.DotNet;
using System;

namespace BambusVM;

internal class Program
{
    // ReSharper disable once UnusedParameter.Local
    private static void Main(string[] args)
    {
        ShowLogoAndPrepareConsole();

        //first we need from user a file that we want to use
        var (module, inputFilePath) = FileManager.GetInputFileAndModule();

        //now we protect the file and let us return the output path
        var outputPath = ProtectAndSave(module, inputFilePath);

        Logger.LogInfo(outputPath);
        Logger.LogInfo("Press a key to close this application.");
        Console.ReadKey();
    }

    private static void ShowLogoAndPrepareConsole()
    {
        Console.SetWindowSize(102, 22);
        Console.SetBufferSize(102, 9000);

        var logo = @"
__________               ___.                ____   _________
\______   \_____    _____\_ |__  __ __  _____\   \ /   /     \
 |    |  _/\__  \  /     \| __ \|  |  \/  ___/\   Y   /  \ /  \
 |    |   \ / __ \|  Y Y  \ \_\ \  |  /\___ \  \     /    Y    \
 |______  /(____  /__|_|  /___  /____//____  >  \___/\____|__  /
        \/      \/      \/    \/           \/                \/
";

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(logo);

        Console.Title = "BambusVM";
    }

    private static object ProtectAndSave(ModuleDefMD module, string inputFilePath)
    {
        //first we convert the input path into an output path
        var outputPath = inputFilePath.Substring(0, inputFilePath.Length - 4) + "-BambusVM" +
                         inputFilePath.Substring(inputFilePath.Length - 4, 4);

        try
        {
            //before virtualizing the code we must first inject the vm into the target module
            VMInjector.InjectVmCode(ref module);

            //launch virtualization
            Virtualization.Execute(module, outputPath);

            Logger.LogGood("Your file has been protected successfully.");
        }
        catch (Exception e)
        {
            Logger.LogError("An error occurred while saving the module.");
            Logger.LogError(e.Message);
            Console.ReadKey();
            Environment.Exit(0);
        }

        return outputPath;
    }
}