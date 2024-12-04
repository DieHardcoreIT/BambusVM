using BambusVM.Runtime.Util;
using System;

namespace BambusVM.Runtime.Handler.Impl;

internal class ConvU1 : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var value = vmContext.Stack.Pop();
        var bytes = int.Parse(Convert.ToByte(value));

        vmContext.Stack.Push(bytes);
    }
}