using BambusVM.Runtime.Handler;

namespace BambusVM.Runtime.Util
{
    public class BambusInstruction
    {
        public BambusInstruction(BambusOpCodes opcode, dynamic value)
        {
            OpCode = opcode;
            Operand = value == null ? null : value;
        }

        public BambusInstruction(BambusOpCodes opcode)
        {
            OpCode = opcode;
            Operand = null;
        }

        public BambusOpCodes OpCode { get; }
        public dynamic Operand { get; }
    }
}