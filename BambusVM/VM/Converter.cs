using BambusVM.Helper;
using BambusVM.Runtime.Handler;
using BambusVM.Runtime.Util;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambusVM.VM;

internal class Converter
{
    /// <summary>
    /// Converts a list of Bambus instructions into a string representation, and encrypts the resulting string using AES encryption.
    /// </summary>
    /// <param name="instructions">A list of BambusInstruction objects that represent the instructions to be converted and encrypted.</param>
    /// <returns>A tuple containing the encrypted data string, the encryption key, and the initialization vector.</returns>
    internal static (string EncryptedData, string Key, string IV) ConvertAndEncrypt(
        List<BambusInstruction> instructions)
    {
        var convertedDataBuilder = new StringBuilder();

        // Convert each instruction into a string representation and append it to the StringBuilder
        foreach (var instruction in instructions) convertedDataBuilder.Append(ConvertInstructionToString(instruction));

        // Concatenate all converted instruction strings and trim the trailing semicolon
        var concatenatedData = convertedDataBuilder.ToString().TrimEnd(';');

        // Encrypt the concatenated instructions string using AES encryption
        var encryptedData =
            AesHelper.EncryptDataWithAes(concatenatedData, out var encryptionKey, out var initializationVector);
        return (encryptedData, encryptionKey, initializationVector);
    }

    /// <summary>
    /// Converts a BambusInstruction object into its string representation.
    /// </summary>
    /// <param name="instruction">The BambusInstruction object to be converted.</param>
    /// <returns>A string that represents the instruction, which includes the opcode as an integer value followed by the operand as a base64-encoded string.</returns>
    private static string ConvertInstructionToString(BambusInstruction instruction)
    {
        // Convert the OpCode to its integer value and format it as a string
        var opcodeString = $"{(int)instruction.OpCode},";

        // Convert the Operand to a base64-encoded string
        string operandString = Convert.ToBase64String(Encoding.UTF8.GetBytes((instruction.Operand ?? "").ToString())) +
                               ";";

        // Return the combined opcode and operand strings
        return opcodeString + operandString;
    }

    /// <summary>
    /// Translates the Intermediate Language (IL) code of a given method into a list of Bambus VM instructions.
    /// </summary>
    /// <param name="method">The method whose IL code is to be translated.</param>
    /// <returns>A list of BambusInstruction objects representing the translated VM instructions of the method.</returns>
    internal static List<BambusInstruction> TranslateToIlCode(MethodDef method)
    {
        // Optimize the method body before processing
        OptimizeMethodBody(method);
        var instructions = method.Body.Instructions.ToList(); // Get the IL instructions of the method
        var vmInstructions = new List<BambusInstruction>(); // Initialize the list to hold VM instructions

        foreach (var instruction in instructions)
            if (IsLdcInstruction(instruction)) // Check if it's a load constant instruction
            {
                AddLdcInstruction(vmInstructions, instruction); // Convert and add LDC instruction
            }
            else if (instruction.OpCode == OpCodes.Ldtoken) // Check if it's a Ldtoken instruction
            {
                // Convert and add Ldtoken instruction with value
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.Ldtoken, GetLdtokenValue(instruction)));
            }
            else if (IsCallInstruction(instruction)) // Check if it's a call or callvirt instruction
            {
                AddCallInstruction(vmInstructions, instruction); // Convert and add call instruction
            }
            else if (IsBranchInstruction(instruction)) // Check if it's a branch instruction
            {
                AddBranchInstruction(vmInstructions, method, instruction); // Convert and add branch instruction
            }
            else if (instruction.OpCode == OpCodes.Box) // Check if it's a box instruction
            {
                // Convert and add box instruction
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.Box,
                    ((ITypeDefOrRef)instruction.Operand).FullName));
            }
            else if (instruction.OpCode.Name.StartsWith("ldelem") ||
                     instruction.OpCode.Name.StartsWith("stelem")) // Check for array operations
            {
                // Convert and add array handling instruction
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxArray,
                    instruction.OpCode.Name.StartsWith("lde") ? 0 : 1));
            }
            else if (instruction.IsLdloc() || instruction.IsStloc()) // Check for local variable operations
            {
                // Convert and add local variable handling instruction
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxLoc,
                    (instruction.IsLdloc() ? "0" : "1") + method.Body.Variables.IndexOf((Local)instruction.Operand)));
            }
            else if (instruction.IsLdarg() || instruction.IsStarg()) // Check for argument operations
            {
                // Convert and add argument handling instruction
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxArg,
                    (instruction.IsLdarg() ? "0" : "1") + method.Parameters.IndexOf((Parameter)instruction.Operand)));
            }
            else if (IsFieldInstruction(instruction)) // Check for field operations
            {
                AddFieldInstruction(vmInstructions, instruction); // Convert and add field instruction
            }
            else if (instruction.OpCode == OpCodes.Conv_R4 ||
                     instruction.OpCode == OpCodes.Conv_R8) // Check for conversion operations
            {
                // Convert and add conversion instruction
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxConv,
                    instruction.OpCode == OpCodes.Conv_R4 ? 0 : 1));
            }
            else if (instruction.OpCode == OpCodes.Newobj) // Check for new object instantiation
            {
                // Convert and add new object instruction
                vmInstructions.Add(new BambusInstruction(BambusOpCodes.Newobj,
                    ((IMethod)instruction.Operand).MDToken.ToInt32()));
            }
            else
            {
                // Try to add a simple instruction, log if not supported
                if (!TryAddSimpleInstruction(vmInstructions, instruction))
                {
                    Logger.LogWarning(
                        $"Skipped function \"{method.Name}\" due to an unsupported opcode: {instruction.OpCode}");
                    return null; // Return null in case of unsupported opcode
                }
            }

        return vmInstructions; // Return the converted list of VM instructions
    }

    /// <summary>
    /// Optimizes the IL method body by simplifying branches and optimizing macro instructions.
    /// </summary>
    /// <param name="method">The method definition whose body is to be optimized.</param>
    private static void OptimizeMethodBody(MethodDef method)
    {
        method.Body.OptimizeBranches();
        method.Body.OptimizeMacros();
        method.Body.SimplifyBranches();
        method.Body.SimplifyMacros(method.Parameters);
    }

    /// <summary>
    /// Determines whether a given instruction is a load constant (LDC) instruction.
    /// </summary>
    /// <param name="instruction">The instruction to evaluate.</param>
    /// <returns>True if the instruction is a load constant instruction; otherwise, false.</returns>
    private static bool IsLdcInstruction(Instruction instruction)
    {
        return instruction.IsLdcI4() ||
               instruction.OpCode == OpCodes.Ldc_R4 ||
               instruction.OpCode == OpCodes.Ldc_R8 ||
               instruction.OpCode == OpCodes.Ldc_I8 ||
               instruction.OpCode == OpCodes.Ldc_I4_M1 ||
               instruction.OpCode == OpCodes.Ldnull ||
               instruction.OpCode == OpCodes.Ldstr;
    }

    /// <summary>
    /// Converts a load constant (LDC) Intermediate Language instruction into a BambusInstruction and adds it to the specified list.
    /// </summary>
    /// <param name="vmInstructions">The list of BambusInstruction objects to which the converted instruction will be added.</param>
    /// <param name="instruction">The IL instruction to be converted and added as a Bambus LDC instruction.</param>
    private static void AddLdcInstruction(List<BambusInstruction> vmInstructions, Instruction instruction)
    {
        vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxLdc,
            instruction.OpCode == OpCodes.Ldnull ? null : instruction.Operand));
    }

    /// <summary>
    /// Retrieves the token value of an operand from a Ldtoken IL instruction as a string representation.
    /// </summary>
    /// <param name="instruction">The IL instruction containing the operand for which the token value is to be retrieved.</param>
    /// <returns>A string representation of the token value, prefixed with a digit indicating the type of the operand.</returns>
    private static string GetLdtokenValue(Instruction instruction)
    {
        return instruction.Operand switch
        {
            MethodDef def => "0" + def.MDToken.ToInt32(),
            MemberRef @ref => "1" + @ref.MDToken.ToInt32(),
            IField field => "2" + field.MDToken.ToInt32(),
            ITypeDefOrRef orRef => "3" + orRef.MDToken.ToInt32(),
            _ => throw new InvalidOperationException("Unsupported Ldtoken operand type")
        };
    }

    /// <summary>
    /// Determines whether the specified instruction is a call or callvirt instruction.
    /// </summary>
    /// <param name="instruction">The MSIL instruction to evaluate.</param>
    /// <returns>True if the instruction is either a call or callvirt instruction; otherwise, false.</returns>
    private static bool IsCallInstruction(Instruction instruction)
    {
        return instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt;
    }

    /// <summary>
    /// Adds a call or callvirt instruction to the list of Bambus instructions,
    /// determining its prefix based on the instruction type and its suffix based on whether the method is defined or referenced.
    /// </summary>
    /// <param name="vmInstructions">The list of BambusInstruction objects to which the call instruction will be added.</param>
    /// <param name="instruction">The IL instruction to be converted into a BambusInstruction call operation.</param>
    private static void AddCallInstruction(List<BambusInstruction> vmInstructions, Instruction instruction)
    {
        var op = (IMethod)instruction.Operand;
        var prefix = instruction.OpCode == OpCodes.Callvirt ? "0" : "1";
        var suffix = op.IsMethodDef ? "1" : "2";

        vmInstructions.Add(new BambusInstruction(
            BambusOpCodes.HxCall,
            op.Name == ".ctor" || op.Name == ".cctor"
                ? $"{prefix}0{op.MDToken.ToInt32()}"
                : $"{prefix}{suffix}{op.MDToken.ToInt32()}"));
    }

    /// <summary>
    /// Determines whether the specified instruction is a branch instruction.
    /// </summary>
    /// <param name="instruction">The instruction to check for branch type.</param>
    /// <returns>A boolean value indicating whether the given instruction is a branch instruction.</returns>
    private static bool IsBranchInstruction(Instruction instruction)
    {
        return instruction.IsBr() || instruction.IsBrtrue() || instruction.IsBrfalse() ||
               instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Leave_S;
    }

    /// <summary>
    /// Converts a branch instruction from a method's IL code into a BambusInstruction and adds it to the list of VM instructions.
    /// </summary>
    /// <param name="vmInstructions">The list of BambusInstruction objects to which the converted branch instruction will be added.</param>
    /// <param name="method">The MethodDef object representing the method containing the branch instruction to be converted.</param>
    /// <param name="instruction">The IL branch instruction to be converted into a BambusInstruction.</param>
    private static void AddBranchInstruction(List<BambusInstruction> vmInstructions, MethodDef method,
        Instruction instruction)
    {
        var index = (dynamic)method.Body.Instructions.IndexOf((Instruction)instruction.Operand);
        if (instruction.IsBrtrue())
            vmInstructions.Add(new BambusInstruction(BambusOpCodes.Brtrue, index));
        else if (instruction.IsBrfalse())
            vmInstructions.Add(new BambusInstruction(BambusOpCodes.Brfalse, index));
        else
            vmInstructions.Add(new BambusInstruction(BambusOpCodes.Br, index));
    }

    /// <summary>
    /// Determines whether the specified instruction is a field-related operation, such as loading a field or a field address.
    /// </summary>
    /// <param name="instruction">The instruction to evaluate for field operations.</param>
    /// <returns>True if the instruction is a field operation, otherwise false.</returns>
    private static bool IsFieldInstruction(Instruction instruction)
    {
        return instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldflda ||
               instruction.OpCode == OpCodes.Ldsfld || instruction.OpCode == OpCodes.Ldsflda;
    }

    /// <summary>
    /// Converts a given field operation IL instruction into a corresponding Bambus VM instruction and adds it to the list of VM instructions.
    /// </summary>
    /// <param name="vmInstructions">The list where the converted Bambus VM instruction will be added.</param>
    /// <param name="instruction">The IL instruction representing a field operation to be converted.</param>
    private static void AddFieldInstruction(List<BambusInstruction> vmInstructions, Instruction instruction)
    {
        var prefix = instruction.OpCode.Name.StartsWith("ldf") ? "0" : "1";
        vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxFld,
            prefix + ((IField)instruction.Operand).MDToken.ToInt32()));
    }

    /// <summary>
    /// Attempts to add a simple instruction to the list of Bambus instructions based on the provided IL instruction.
    /// </summary>
    /// <param name="vmInstructions">A list of BambusInstruction objects to which the new instruction will be added if it is supported.</param>
    /// <param name="instruction">The IL instruction to be converted into a BambusInstruction and added to the list.</param>
    /// <returns>True if the instruction was successfully added to the list; otherwise, false if the instruction is not supported.</returns>
    private static bool TryAddSimpleInstruction(List<BambusInstruction> vmInstructions, Instruction instruction)
    {
        var opcodeName = instruction.OpCode.Name;
        var retrievedOpCode = GetVmOpCode(opcodeName);
        if (retrievedOpCode == null) return false;

        var opc = (BambusOpCodes)Enum.Parse(typeof(BambusOpCodes), retrievedOpCode, true);
        vmInstructions.Add(new BambusInstruction(opc));
        return true;
    }

    /// <summary>
    /// Retrieves the virtual machine opcode that corresponds to a given opcode name.
    /// </summary>
    /// <param name="opCodeName">The name of the opcode to find a corresponding VM opcode for.</param>
    /// <returns>The VM opcode as a string if a match is found; otherwise, null.</returns>
    private static string GetVmOpCode(string opCodeName)
    {
        var bambusOpCodeNames = Enum.GetNames(typeof(BambusOpCodes));
        return bambusOpCodeNames.FirstOrDefault(vmOpCode =>
            string.Equals(opCodeName, vmOpCode, StringComparison.CurrentCultureIgnoreCase));
    }
}