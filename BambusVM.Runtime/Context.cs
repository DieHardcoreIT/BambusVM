using BambusVM.Runtime.Handler;
using BambusVM.Runtime.Handler.Impl;
using BambusVM.Runtime.Handler.Impl.Custom;
using BambusVM.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambusVM.Runtime;

public class Context
{
    /// Gets the arguments passed to the Bambus virtual machine.
    public readonly VmArgs Args;

    /// Gets the dictionary mapping operation codes to their corresponding Bambus handler implementations.
    private readonly Dictionary<BambusOpCodes, BambusOpCode> Handlers;

    /// Gets the local variables for the virtual machine.
    public readonly VmLocal Locals;

    /// Gets the stack used for execution in the virtual machine.
    public readonly VmStack Stack;

    /// Gets the list of instructions to be executed in the virtual machine.
    private List<string> Instructions { get; }

    /// Gets or sets the current instruction index.
    public int Index { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Context"/> class.
    /// </summary>
    /// <param name="instructions">The list of instruction strings to execute.</param>
    /// <param name="argumentArray">The array of arguments provided to the virtual machine.</param>
    public Context(List<string> instructions, object[] argumentArray)
    {
        Instructions = instructions;
        Stack = new VmStack();
        Locals = new VmLocal();
        Args = new VmArgs(argumentArray);
        Handlers = InitializeHandlers();
        Index = 0;
    }

    /// <summary>
    /// Executes the Bambus instructions.
    /// </summary>
    /// <returns>The result of executing the instructions, usually the top of the stack.</returns>
    public object Run()
    {
        do
        {
            var instruction = GetInstruction(Index);
            Handlers[instruction.OpCode].Execute(this, instruction);
            Index++;
        } while (Instructions.Count > Index);

        return Stack.Pop();
    }

    /// <summary>
    /// Initializes and returns the dictionary of operation code handlers.
    /// </summary>
    /// <returns>A dictionary mapping Bambus operation codes to their handlers.</returns>
    private Dictionary<BambusOpCodes, BambusOpCode> InitializeHandlers()
    {
        return new Dictionary<BambusOpCodes, BambusOpCode>
        {
            // custom
            { BambusOpCodes.HxCall, new BambusCall() },
            { BambusOpCodes.HxLdc, new BambusLdc() },
            { BambusOpCodes.HxArray, new BambusArray() },
            { BambusOpCodes.HxLoc, new BambusLoc() },
            { BambusOpCodes.HxArg, new BambusArg() },
            { BambusOpCodes.HxFld, new BambusFld() },
            { BambusOpCodes.HxConv, new BambusConv() },

            // not custom
            { BambusOpCodes.Or, new Or() },
            { BambusOpCodes.Null, new Null() },
            { BambusOpCodes.Newarr, new Newarr() },
            { BambusOpCodes.Ldnull, new Ldnull() },
            { BambusOpCodes.Ldloca, new Ldloca() },
            { BambusOpCodes.Ldlen, new Ldlen() },
            { BambusOpCodes.LdelemU1, new LdelemU1() },
            { BambusOpCodes.Ldc, new Ldc() },
            { BambusOpCodes.ConvU1, new ConvU1() },
            { BambusOpCodes.ConvI4, new ConvI4() },
            { BambusOpCodes.Cmp, new Cmp() },
            { BambusOpCodes.Clt, new Clt() },
            { BambusOpCodes.Cgt, new Cgt() },
            { BambusOpCodes.Neg, new Neg() },
            { BambusOpCodes.Not, new Not() },
            { BambusOpCodes.And, new And() },
            { BambusOpCodes.Shr, new Shr() },
            { BambusOpCodes.Shl, new Shl() },
            { BambusOpCodes.Xor, new Xor() },
            { BambusOpCodes.Rem, new Rem() },
            { BambusOpCodes.Ceq, new Ceq() },
            { BambusOpCodes.Mul, new Mul() },
            { BambusOpCodes.Nop, new Nop() },
            { BambusOpCodes.Add, new Add() },
            { BambusOpCodes.Sub, new Sub() },
            { BambusOpCodes.Ret, new Ret() },
            { BambusOpCodes.Pop, new Pop() },
            { BambusOpCodes.Len, new Len() },
            { BambusOpCodes.Dup, new Dup() },
            { BambusOpCodes.Div, new Div() },
            { BambusOpCodes.Ldtoken, new Ldtoken() },
            { BambusOpCodes.Br, new Br() },
            { BambusOpCodes.Brtrue, new Brtrue() },
            { BambusOpCodes.Brfalse, new Brfalse() },
            { BambusOpCodes.Box, new Box() },
            { BambusOpCodes.Newobj, new Newobj() }
        };
    }

    /// <summary>
    /// Retrieves and parses a Bambus instruction from the instructions list at the specified index.
    /// </summary>
    /// <param name="index">The index of the instruction to parse.</param>
    /// <returns>A <see cref="BambusInstruction"/> representing the parsed instruction.</returns>
    private BambusInstruction GetInstruction(int index)
    {
        var instructionsData = Instructions[index].Split(',').ToList();
        var opCode = (BambusOpCodes)int.Parse(instructionsData[0]);
        var operand = Encoding.UTF8.GetString(Convert.FromBase64String(instructionsData[1]));
        return new BambusInstruction(opCode, operand);
    }
}