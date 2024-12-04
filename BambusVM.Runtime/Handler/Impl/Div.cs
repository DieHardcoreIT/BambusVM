using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Div : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var firstValue = vmContext.Stack.Pop();
        var secondValue = vmContext.Stack.Pop();

        // Swap the order to correctly divide secondValue by firstValue
        vmContext.Stack.Push(secondValue / firstValue);
    }
}