using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Ret : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        vmContext.Stack.Push(vmContext.Stack.Count == 0 ? null : vmContext.Stack.Pop());
    }
}