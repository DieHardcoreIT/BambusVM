using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    internal class ConvI4 : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var x = int.Parse(vmContext.Stack.Pop());

            vmContext.Stack.Push(x);
        }
    }
}