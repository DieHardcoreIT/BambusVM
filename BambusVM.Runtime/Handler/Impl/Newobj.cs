using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Newobj : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var constructor = ForceResolveConstructor(int.Parse(instruction.Operand));
            var methodParameters = Helper.GetMethodParameters(vmContext, constructor.GetParameters());

            var inst = constructor.Invoke(methodParameters);
            if (inst != null)
                vmContext.Stack.Push(inst);
        }
    }
}