using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    internal class Newarr : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var number = vmContext.Stack.Pop();

            vmContext.Stack.Push(new byte[number]);
        }
    }
}