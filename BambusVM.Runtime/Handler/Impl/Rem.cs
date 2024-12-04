using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Rem : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var x = vmContext.Stack.Pop();
        var y = vmContext.Stack.Pop();

        vmContext.Stack.Push(x % y);
    }
}