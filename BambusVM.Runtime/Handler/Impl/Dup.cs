using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Dup : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            vmContext.Stack.Push(vmContext.Stack.Peek());
        }
    }
}