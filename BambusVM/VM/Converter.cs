using BambusVM.Helper;
using BambusVM.Runtime.Handler;
using BambusVM.Runtime.Util;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambusVM.VM
{
    internal class Converter
    {
        internal static (string, string, string) ConvertAndEncrypt(List<BambusInstruction> convertedInstructions)
        {
            var convertedVmData = new StringBuilder();

            //now we take all the instructions and convert them into a simple string and encrypt it
            foreach (var convertedInstruction in convertedInstructions)
            {
                //fist we add the opcode
                convertedVmData.Append((int)convertedInstruction.OpCode + ",");

                //then we add the operand
                var operandString = Convert.ToBase64String(Encoding.UTF8.GetBytes((convertedInstruction.Operand ?? "").ToString()));

                convertedVmData.Append(operandString + ";");
            }

            var convertedVmDataString = convertedVmData.ToString();

            //remove the last char
            convertedVmDataString = convertedVmDataString.Substring(0, convertedVmDataString.Length - 1);

            //encrypt
            var encryptedConvertedVmDataString = AesHelper.EncryptDataWithAes(convertedVmDataString, out var key, out var iv);

            return (encryptedConvertedVmDataString, key, iv);
        }

        internal static List<BambusInstruction> TranslateToIlCode(MethodDef method)
        {
            //optimize everything to avoid problems
            method.Body.OptimizeBranches();
            method.Body.OptimizeMacros();
            method.Body.SimplifyBranches();
            method.Body.SimplifyMacros(method.Parameters);

            var instructions = method.Body.Instructions.ToList();
            var vmInstructions = new List<BambusInstruction>();

            //now we go through all the instructions and convert them
            foreach (var instruction in instructions)
            {
                if (instruction.IsLdcI4() || instruction.OpCode == OpCodes.Ldc_R4 ||
                    instruction.OpCode == OpCodes.Ldc_R8 ||
                    instruction.OpCode == OpCodes.Ldc_I8 || instruction.OpCode == OpCodes.Ldc_I4_M1 ||
                    instruction.OpCode == OpCodes.Ldnull ||
                    instruction.OpCode == OpCodes.Ldstr)
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxLdc,
                        instruction.OpCode == OpCodes.Ldnull ? null : instruction.Operand));
                }
                else if (instruction.OpCode == OpCodes.Ldtoken)
                {
                    // eh string is fine here, we can just do substring later
                    var value = "";
                    if (instruction.Operand is MethodDef def)
                        value = "0" + def.MDToken.ToInt32();
                    else if (instruction.Operand is MemberRef @ref)
                        value = "1" + @ref.MDToken.ToInt32();
                    else if (instruction.Operand is IField field)
                        value = "2" + field.MDToken.ToInt32();
                    else if (instruction.Operand is ITypeDefOrRef orRef)
                        value = "3" + orRef.MDToken.ToInt32();

                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.Ldtoken, value));
                }
                else if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    var op = (IMethod)instruction.Operand;

                    if (op.Name == ".ctor" || op.Name == ".cctor")
                        vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxCall,
                            (instruction.OpCode == OpCodes.Callvirt ? "0" : "1") + "0" + op.MDToken.ToInt32()));
                    else if (op.IsMethodDef)
                        vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxCall,
                            (instruction.OpCode == OpCodes.Callvirt ? "0" : "1") + "1" + op.MDToken.ToInt32()));
                    else
                        vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxCall,
                            (instruction.OpCode == OpCodes.Callvirt ? "0" : "1") + "2" + op.MDToken.ToInt32()));
                }
                else if (instruction.IsBr() || instruction.IsBrtrue() || instruction.IsBrfalse() ||
                         instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Leave_S)
                {
                    var val = (dynamic)method.Body.Instructions.IndexOf((Instruction)instruction.Operand);
                    if (instruction.IsBrtrue())
                        vmInstructions.Add(new BambusInstruction(BambusOpCodes.Brtrue, val));
                    else if (instruction.IsBrfalse())
                        vmInstructions.Add(new BambusInstruction(BambusOpCodes.Brfalse, val));
                    else
                        vmInstructions.Add(new BambusInstruction(BambusOpCodes.Br, val));
                }
                else if (instruction.OpCode == OpCodes.Box)
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.Box,
                        ((ITypeDefOrRef)instruction.Operand).FullName));
                    //   conv.Add(new BambusInstruction(BambusOpCodes.Box,  new Value(((ITypeDefOrRef)i.Operand).MDToken.ToInt32())));
                }
                else if (instruction.OpCode.Name.StartsWith("ldelem") || instruction.OpCode.Name.StartsWith("stelem"))
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxArray,
                        instruction.OpCode.Name.StartsWith("lde") ? 0 : 1));
                }
                else if (instruction.IsLdloc() || instruction.IsStloc())
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxLoc,
                        (instruction.IsLdloc() ? "0" : "1") +
                        method.Body.Variables.IndexOf((Local)instruction.Operand)));
                }
                else if (instruction.IsLdarg() || instruction.IsStarg())
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxArg,
                        (instruction.IsLdarg() ? "0" : "1") +
                        method.Parameters.IndexOf((Parameter)instruction.Operand)));
                }
                else if (instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldflda ||
                         instruction.OpCode == OpCodes.Ldsfld || instruction.OpCode == OpCodes.Ldsflda)
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxFld,
                        (instruction.OpCode.Name.StartsWith("ldf") ? "0" : "1") +
                        ((IField)instruction.Operand).MDToken.ToInt32()));
                }
                else if (instruction.OpCode == OpCodes.Conv_R4)
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxConv, 0));
                }
                else if (instruction.OpCode == OpCodes.Conv_R8)
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.HxConv, 1));
                }
                else if (instruction.OpCode == OpCodes.Newobj)
                {
                    vmInstructions.Add(new BambusInstruction(BambusOpCodes.Newobj,
                        ((IMethod)instruction.Operand).MDToken.ToInt32()));
                }
                else
                {
                    var retrievedOpCode = GetVmOpCode(instruction.OpCode.Name);

                    // simple stuff, no operand
                    if (retrievedOpCode != null)
                    {
                        var opc = (BambusOpCodes)Enum.Parse(typeof(BambusOpCodes), retrievedOpCode, true);
                        vmInstructions.Add(new BambusInstruction(opc));
                    }
                    else
                    {
                        Logger.LogWarning($"Skipped function \"{method.Name}\" due to an unsupported opcode: {instruction.OpCode}");
                        return null;
                    }
                }
            }

            return vmInstructions;
        }

        private static string GetVmOpCode(string opCodeName)
        {
            var vmOpCodes = Enum.GetNames(typeof(BambusOpCodes));

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var vmOpCode in vmOpCodes)
                if (string.Equals(opCodeName, vmOpCode, StringComparison.CurrentCultureIgnoreCase))
                    return vmOpCode;

            return null;
        }
    }
}