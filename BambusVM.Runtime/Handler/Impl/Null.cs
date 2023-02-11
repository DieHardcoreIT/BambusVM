using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    internal class Null : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            vmContext.Stack.Push(null);
        }
    }
}