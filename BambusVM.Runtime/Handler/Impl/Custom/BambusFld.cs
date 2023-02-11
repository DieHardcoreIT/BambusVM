using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom
{
    public class BambusFld : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var str = instruction.Operand.ToString();

            var id = Helper.ReadPrefix(str);
            var mdToken = int.Parse(str.Substring(1));

            var fi = ForceResolveField(mdToken);
            var v = fi.GetValue(id == 0 ? vmContext.Stack.Pop() : null);
            vmContext.Stack.Push(v);
        }
    }
}