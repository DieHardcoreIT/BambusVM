using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    internal class Ldlen : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            byte[] bytes = (byte[])vmContext.Stack.Pop();

            vmContext.Stack.Push(bytes.Length);
        }
    }
}