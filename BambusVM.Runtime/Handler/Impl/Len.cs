using BambusVM.Runtime.Util;
using System;

namespace BambusVM.Runtime.Handler.Impl;

public class Len : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var arr = (Array)vmContext.Stack.Pop();

        vmContext.Stack.Push(arr.Length);
    }
}