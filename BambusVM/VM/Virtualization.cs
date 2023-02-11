using BambusVM.Helper;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System.IO;
using System.Linq;

namespace BambusVM.VM
{
    public class Virtualization
    {
        internal static void Execute(ModuleDefMD module, string outPath)
        {
            //first we need to find the function where the vm is starting
            var vmEntry = GetVmEntry(module);

            //if this is "null" we can not continue
            if (vmEntry == null)
            {
                Logger.LogError("VM entry not found!");
                return;
            }

            //now we go through all the functions
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    //we ignore here all functions which are e.g. empty
                    if (!method.HasBody || method.IsConstructor || method.IsRuntime || method.IsVirtual ||
                        method.IsGetter || method.IsSetter)
                        continue;

                    //don't virtualize vm runtime
                    if (method.FullName.Contains("BambusVM"))
                        continue;

                    //now we convert the IlCode
                    var convertedInstructions = Converter.TranslateToIlCode(method);

                    //if the function is not supported because an opcode is not present then "null" is returned
                    if (convertedInstructions == null)
                        continue;

                    //instructions to encrpyted string
                    var (encryptedVmCode, key, iv) = Converter.ConvertAndEncrypt(convertedInstructions);

                    //create new method to override the old method
                    method.Body = new CilBody();

                    //string -> key
                    method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, key));

                    //string -> iv
                    method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, iv));

                    //string -> encryptedVMCode
                    method.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, encryptedVmCode));

                    method.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, method.Parameters.Count));
                    method.Body.Instructions.Add(OpCodes.Newarr.ToInstruction(module.CorLibTypes.Object));

                    for (var i = 0; i < method.Parameters.Count; i++)
                    {
                        method.Body.Instructions.Add(new Instruction(OpCodes.Dup));
                        method.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, i));
                        method.Body.Instructions.Add(new Instruction(OpCodes.Ldarg,
                            method.Parameters[i]));
                        method.Body.Instructions.Add(new Instruction(OpCodes.Box,
                            method.Parameters[i].Type.ToTypeDefOrRef()));
                        method.Body.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
                    }

                    method.Body.Instructions.Add(new Instruction(OpCodes.Call, vmEntry));

                    if (method.HasReturnType)
                        method.Body.Instructions.Add(new Instruction(OpCodes.Unbox_Any,
                            method.ReturnType.ToTypeDefOrRef()));
                    else
                        method.Body.Instructions.Add(OpCodes.Pop.ToInstruction());

                    method.Body.Instructions.Add(new Instruction(OpCodes.Ret));

                    //function must be optimized otherwise there may be errors
                    method.Body.UpdateInstructionOffsets();
                    method.Body.OptimizeBranches();
                    method.Body.OptimizeMacros();
                }
            }

            WriteDataToFile(module, outPath);
        }

        private static void WriteDataToFile(ModuleDefMD module, string outPath)
        { 
            //writer options
            var modOpts = new ModuleWriterOptions(module)
            {
                MetadataOptions =
                {
                    Flags = MetadataFlags.PreserveAll //we want all metadata to be the same
                },
                MetadataLogger = DummyLogger.NoThrowInstance //dnlib should not throw any errors
            };

            var mem = new MemoryStream();
            module.Write(mem, modOpts); //save the module.
            File.WriteAllBytes(outPath, mem.ToArray());
        }

        private static MethodDef GetVmEntry(ModuleDefMD module)
        {
            var types = module.Types.ToList();
            foreach (var type in types)
            {
                if (!type.FullName.Contains("BambusVM"))
                    continue;

                foreach (var method in type.Methods)
                    if (method.FullName.Contains("Execute("))
                        return method;
            }

            return null;
        }
    }
}