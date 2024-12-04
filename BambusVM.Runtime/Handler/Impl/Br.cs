using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Br : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        vmContext.Index = int.Parse(instruction.Operand) - 1; // -1 due to VM's +1 auto increment
    }
}