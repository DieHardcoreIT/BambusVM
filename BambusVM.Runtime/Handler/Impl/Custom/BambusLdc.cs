using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusLdc : BambusOpCode
{
    /// <summary>
    /// This method pushes the operand of the given instruction onto the VM stack.
    /// </summary>
    /// <param name="vmContext">The current execution context of the Bambus virtual machine.</param>
    /// <param name="instruction">The instruction to be executed, containing the opcode and operand.</param>
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        vmContext.Stack.Push(instruction.Operand);
    }
}