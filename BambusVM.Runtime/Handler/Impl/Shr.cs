using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Shr : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            dynamic x = vmContext.Stack.Pop();
            dynamic y = vmContext.Stack.Pop();

            vmContext.Stack.Push(x >> y);
        }
    }
}