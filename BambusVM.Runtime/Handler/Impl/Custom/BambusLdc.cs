using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom
{
    public class BambusLdc : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            vmContext.Stack.Push(instruction.Operand);
        }
    }
}