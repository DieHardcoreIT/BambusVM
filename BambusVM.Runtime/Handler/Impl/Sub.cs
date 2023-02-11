using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Sub : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var y = vmContext.Stack.Pop();
            var x = vmContext.Stack.Pop();

            vmContext.Stack.Push(y - x);
        }
    }
}