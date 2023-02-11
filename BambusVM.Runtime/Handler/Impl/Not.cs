using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Not : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var x = vmContext.Stack.Pop();

            vmContext.Stack.Push(~x);
        }
    }
}