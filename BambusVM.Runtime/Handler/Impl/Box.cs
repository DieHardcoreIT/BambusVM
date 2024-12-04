using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Box : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var value = vmContext.Stack.Pop();

        object boxedValue = value;

        vmContext.Stack.Push(boxedValue);
    }
}