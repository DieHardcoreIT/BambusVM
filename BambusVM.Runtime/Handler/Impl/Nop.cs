using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl
{
    public class Nop : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            //do nothing
        }
    }
}