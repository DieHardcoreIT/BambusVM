using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System.Collections.Generic;
using System.IO;

namespace BambusVM.VM
{
    internal class VMInjector
    {
        internal static void InjectVmCode(ref ModuleDefMD module)
        {
            //load vm runtime dll
            var vmRuntimeAssemblyDef = AssemblyDef.Load("BambusVM.Runtime.dll");

            //here we store all types
            var typeDefs = new List<TypeDef>();

            //now we read all vm types from the dll
            foreach (var modules in vmRuntimeAssemblyDef.Modules)
                foreach (var vmRuntimeType in modules.Types)
                {
                    if (!vmRuntimeType.FullName.Contains("BambusVM"))
                        continue;
                    typeDefs.Add(vmRuntimeType);
                }

            //now we have to remove all types from the dll otherwise there will be an error.  I don't know how else to do it, but it works.
            foreach (var modules in vmRuntimeAssemblyDef.Modules)
                modules.Types.Clear();

            //now we can finally add all modules to the original exe
            foreach (var typeDef in typeDefs) 
                module.Types.Add(typeDef);

            WriteVmToDllAndReloadModule(ref module);
        }

        private static void WriteVmToDllAndReloadModule(ref ModuleDefMD module)
        {
            var modOpts = new ModuleWriterOptions(module)
            {
                //ignore all errors
                MetadataLogger = DummyLogger.NoThrowInstance
            };

            var mem = new MemoryStream();
            module.Write(mem, modOpts); //save the module.
            File.WriteAllBytes("BambusVM.tmp", mem.ToArray());

            //load the module
            module = ModuleDefMD.Load("BambusVM.tmp");
        }
    }
}