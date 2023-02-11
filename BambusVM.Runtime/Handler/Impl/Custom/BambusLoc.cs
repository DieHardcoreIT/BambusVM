using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom
{
    public class BambusLoc : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var str = instruction.Operand.ToString();
            var prefix = Helper.ReadPrefix(str);
            var idx = int.Parse(str.Substring(1));

            if (prefix == 0)
            {
                vmContext.Stack.Push(vmContext.Locals.Get(idx));
            }
            else
            {
                var item = vmContext.Stack.Pop();
                vmContext.Locals.Update(idx, item);
            }
        }
    }
}