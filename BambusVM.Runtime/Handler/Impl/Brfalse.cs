using System;
using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Brfalse : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Pop the value from the stack for evaluation
        var value = vmContext.Stack.Pop();
        int x;

        // Check the type of value and convert to int accordingly
        if (value is bool b)
            x = b ? 1 : 0; // If it's a boolean, convert true to 1 and false to 0
        else // Otherwise, parse the value as an integer
            x = Convert.ToInt32(value);

        // If the evaluated integer is zero, adjust the instruction pointer
        if (x == 0)
            vmContext.Index = Convert.ToInt32(instruction.Operand) - 1; // -1 due to VM's +1 auto increment
    }
}