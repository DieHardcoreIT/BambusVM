using BambusVM.Helper;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System.IO;
using System.Linq;

namespace BambusVM.VM;

public class Virtualization
{
    /// <summary>
    /// Executes the virtualization process on the provided module and writes the transformed module data to the specified output path.
    /// This method processes each method in the module using the virtual machine entry point.
    /// </summary>
    /// <param name="module">The module to be processed and virtualized by the virtual machine.</param>
    /// <param name="outputPath">The file path where the transformed module data will be saved.</param>
    internal static void Execute(ModuleDefMD module, string outputPath)
    {
        // Retrieve the entry method for the virtual machine
        var virtualMachineEntry = GetVmEntry(module);
        if (virtualMachineEntry == null)
        {
            // Log an error and exit if no VM entry is found
            Logger.LogError("VM entry not found!");
            return;
        }

        // Iterate over each type in the module
        foreach (var type in module.Types)
            // Iterate over each method in the type
        foreach (var method in type.Methods)
            // Process the method using the VM entry point
            ProcessMethod(module, method, virtualMachineEntry);

        // Write the modified module data to the specified output path
        WriteDataToFile(module, outputPath);
    }

    /// <summary>
    /// Processes a given method within the module using the specified VM entry point.
    /// This involves translating the method's IL code into Bambus VM instructions,
    /// encrypting the converted instructions, and updating the method body
    /// with the necessary encryption instructions.
    /// </summary>
    /// <param name="module">The module containing the method to be processed.</param>
    /// <param name="method">The method to be processed and translated into VM instructions.</param>
    /// <param name="virtualMachineEntry">The entry point method for the virtual machine that facilitates processing.</param>
    private static void ProcessMethod(ModuleDefMD module, MethodDef method, IMethod virtualMachineEntry)
    {
        // Skip methods that do not need processing
        if (!ShouldProcessMethod(method))
            return;

        // Translate the method's IL code to Bambus VM instructions
        var convertedInstructions = Converter.TranslateToIlCode(method);
        if (convertedInstructions == null)
            return;

        // Encrypt the converted instructions
        var (encryptedVmCode, key, iv) = Converter.ConvertAndEncrypt(convertedInstructions);

        // Clear the method body and add encryption instructions
        method.Body = new CilBody();
        AddEncryptionInstructions(method, key, iv, encryptedVmCode, module, virtualMachineEntry);
    }

    /// <summary>
    /// Determines whether a given method should be processed by the virtualization system.
    /// A method is eligible for processing if it has a body, is not a constructor,
    /// is not a runtime method, is not virtual, is not a property accessor,
    /// and does not belong to the BambusVM namespace.
    /// </summary>
    /// <param name="method">The method to evaluate for processing.</param>
    /// <returns>True if the method should be processed; otherwise, false.</returns>
    private static bool ShouldProcessMethod(MethodDef method)
    {
        return method.HasBody && !method.IsConstructor && !method.IsRuntime &&
               !method.IsVirtual && !method.IsGetter && !method.IsSetter &&
               !method.FullName.Contains("BambusVM");
    }

    /// <summary>
    /// Injects and configures a sequence of instructions in a method to facilitate
    /// execution under a virtual machine. This involves loading encrypted
    /// instructions, keys, and parameters onto the stack, and invoking the
    /// virtual machine's entry method.
    /// </summary>
    /// <param name="method">The method to modify by adding encryption instructions.</param>
    /// <param name="key">The encryption key utilized to decrypt the virtual machine code.</param>
    /// <param name="iv">The initialization vector used in the encryption process.</param>
    /// <param name="encryptedVmCode">The encrypted virtual machine instructions.</param>
    /// <param name="module">The module that contains the method being modified.</param>
    /// <param name="virtualMachineEntry">The entry point method of the virtual machine.</param>
    private static void AddEncryptionInstructions(MethodDef method, string key, string iv,
        string encryptedVmCode, ModuleDefMD module,
        IMethod virtualMachineEntry)
    {
        // Load the encryption key onto the evaluation stack
        method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, key));
        // Load the initialization vector onto the evaluation stack
        method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, iv));
        // Load the encrypted VM code onto the evaluation stack
        method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, encryptedVmCode));
        // Load the number of method parameters onto the evaluation stack
        method.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, method.Parameters.Count));
        // Create an array of objects to store the parameters
        method.Body.Instructions.Add(OpCodes.Newarr.ToInstruction(module.CorLibTypes.Object));

        // Loop through each parameter and store it in the array
        for (var i = 0; i < method.Parameters.Count; i++)
        {
            // Duplicate the array reference on the stack
            method.Body.Instructions.Add(new Instruction(OpCodes.Dup));
            // Load the index onto the evaluation stack
            method.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, i));
            // Load the method argument at the current index
            method.Body.Instructions.Add(new Instruction(OpCodes.Ldarg, method.Parameters[i]));
            // Box the argument if it's a value type
            method.Body.Instructions.Add(new Instruction(OpCodes.Box,
                method.Parameters[i].Type.ToTypeDefOrRef()));
            // Store the argument into the array
            method.Body.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
        }

        // Call the virtual machine entry method with the prepared stack
        method.Body.Instructions.Add(new Instruction(OpCodes.Call, virtualMachineEntry));

        // If the method has a return type, unbox the result; otherwise, discard the result
        if (method.HasReturnType)
            method.Body.Instructions.Add(new Instruction(OpCodes.Unbox_Any, method.ReturnType.ToTypeDefOrRef()));
        else
            method.Body.Instructions.Add(OpCodes.Pop.ToInstruction());

        // Return from the method
        method.Body.Instructions.Add(new Instruction(OpCodes.Ret));

        // Optimize the instruction offsets, branches, and macros
        method.Body.UpdateInstructionOffsets();
        method.Body.OptimizeBranches();
        method.Body.OptimizeMacros();
    }

    /// <summary>
    /// Writes the data from the given module to a file specified by the output path.
    /// This method creates a memory stream to which the module's data is written,
    /// utilizing custom writer options to preserve metadata information.
    /// </summary>
    /// <param name="module">The module whose data is to be written to the file.</param>
    /// <param name="outPath">The file path where the module's data will be saved.</param>
    private static void WriteDataToFile(ModuleDefMD module, string outPath)
    {
        var modOpts = CreateModuleWriterOptions(module);
        using var memoryStream = new MemoryStream();
        module.Write(memoryStream, modOpts);
        File.WriteAllBytes(outPath, memoryStream.ToArray());
    }

    /// <summary>
    /// Creates a <see cref="ModuleWriterOptions"/> for the specified module with predefined settings.
    /// The options are configured to preserve all metadata and utilize a dummy metadata logger
    /// that suppresses exceptions.
    /// </summary>
    /// <param name="module">The <see cref="ModuleDefMD"/> for which to create the writer options.</param>
    /// <returns>A <see cref="ModuleWriterOptions"/> instance containing the specified settings for metadata preservation and logging.</returns>
    private static ModuleWriterOptions CreateModuleWriterOptions(ModuleDefMD module)
    {
        return new ModuleWriterOptions(module)
        {
            MetadataOptions =
            {
                Flags = MetadataFlags.PreserveAll
            },
            MetadataLogger = DummyLogger.NoThrowInstance
        };
    }

    /// <summary>
    /// Identifies and retrieves the entry method for the virtual machine within the specified module.
    /// The VM entry method is determined by searching for a method whose full name contains "Execute("
    /// within types whose full names contain "BambusVM".
    /// </summary>
    /// <param name="module">The module in which to search for the virtual machine entry method.</param>
    /// <returns>
    /// The method definition representing the virtual machine entry point if found; otherwise, returns null
    /// if no appropriate entry method is identified within the module.
    /// </returns>
    private static MethodDef GetVmEntry(ModuleDefMD module)
    {
        return (from type in module.Types
                where type.FullName.Contains("BambusVM")
                select type.Methods.FirstOrDefault(method => method.FullName.Contains("Execute(")))
            .FirstOrDefault(method => method != null);
    }
}