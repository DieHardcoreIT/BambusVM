using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Div : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var firstValue = vmContext.Stack.Pop();
            var secondValue = vmContext.Stack.Pop();

            //yes that looks strange but must be done so
            vmContext.Stack.Push(secondValue / firstValue);
        }
    }
}