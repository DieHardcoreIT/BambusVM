using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

internal class LdelemU1 : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        var value = vmContext.Stack.Pop();
        var array = (byte[])vmContext.Stack.Pop();

        vmContext.Stack.Push(array[value]);
    }
}