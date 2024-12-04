using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

internal class Ldlen : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var bytes = (byte[])vmContext.Stack.Pop();

        vmContext.Stack.Push(bytes.Length);
    }
}