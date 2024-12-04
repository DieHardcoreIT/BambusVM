using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System.Collections.Generic;
using System.IO;

namespace BambusVM.VM;

internal class VMInjector
{
    /// <summary>
    /// Injects the BambusVM runtime code into the specified module.
    /// This method loads the VM runtime DLL, extracts all types that contain
    /// 'BambusVM', clears them from the runtime DLL to avoid errors,
    /// and then adds them to the given module.
    /// </summary>
    /// <param name="module">
    /// The module where the VM runtime code will be injected.
    /// This parameter is passed by reference and will be modified
    /// to include the VM types.
    /// </param>
    internal static void InjectVmCode(ref ModuleDefMD module)
    {
        // Load the VM runtime DLL
        var vmRuntimeAssemblyDef = AssemblyDef.Load("BambusVM.Runtime.dll");

        // Store all types from the VM
        var typeDefs = new List<TypeDef>();

        // Read all VM types that contain 'BambusVM' from the DLL
        foreach (var modules in vmRuntimeAssemblyDef.Modules)
        foreach (var vmRuntimeType in modules.Types)
        {
            if (!vmRuntimeType.FullName.Contains("BambusVM"))
                continue;
            typeDefs.Add(vmRuntimeType);
        }

        // Clear all types from the DLL to avoid errors
        foreach (var modules in vmRuntimeAssemblyDef.Modules)
            modules.Types.Clear();

        // Add all VM types to the original module
        foreach (var typeDef in typeDefs)
            module.Types.Add(typeDef);

        WriteVmToDllAndReloadModule(ref module);
    }

    /// <summary>
    /// Writes the given module to a temporary DLL file and reloads the module from this file.
    /// This allows any changes made to the module to be saved and reloaded for further processing.
    /// </summary>
    /// <param name="module">
    /// A reference to the <see cref="ModuleDefMD"/> instance representing the module to be written and reloaded.
    /// </param>
    private static void WriteVmToDllAndReloadModule(ref ModuleDefMD module)
    {
        var modOpts = new ModuleWriterOptions(module)
        {
            // Ignore all errors during the write process
            MetadataLogger = DummyLogger.NoThrowInstance
        };

        var mem = new MemoryStream();
        module.Write(mem, modOpts); // Write the module to memory
        File.WriteAllBytes("BambusVM.tmp", mem.ToArray()); // Save the module to a temporary file

        // Reload the module from the temporary file
        module = ModuleDefMD.Load("BambusVM.tmp");
    }
}