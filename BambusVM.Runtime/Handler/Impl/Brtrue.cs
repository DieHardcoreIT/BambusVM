using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Brtrue : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var value = vmContext.Stack.Pop();
            int x;

            if (value is bool b)
                x = b ? 1 : 0;
            else
                x = int.Parse(value);

            if (x == 1)
                vmContext.Index = int.Parse(instruction.Operand) - 1; //-1 because we add +1 at the end of the vm execution
        }
    }
}