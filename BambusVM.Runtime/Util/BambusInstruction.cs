using BambusVM.Runtime.Handler;

namespace BambusVM.Runtime.Util;

public class BambusInstruction
{
    /// <summary>
    /// Represents a Bambus instruction with an opcode and an optional operand.
    /// </summary>
    public BambusInstruction(BambusOpCodes opcode, dynamic value)
    {
        OpCode = opcode;
        Operand = value == null ? null : value;
    }

    /// <summary>
    /// Represents an instruction within the Bambus virtual machine. It encapsulates
    /// an operation code (opcode) and an optional operand, which together define the
    /// action that the virtual machine should take.
    /// </summary>
    public BambusInstruction(BambusOpCodes opcode)
    {
        OpCode = opcode;
        Operand = null;
    }

    /// <summary>
    /// Gets the operation code (opcode) of the instruction.
    /// </summary>
    /// <remarks>
    /// The OpCode property represents a specific operation to be performed by the virtual machine.
    /// It is a member of the BambusOpCodes enumeration and defines the action that should be executed.
    /// </remarks>
    public BambusOpCodes OpCode { get; }

    /// <summary>
    /// Gets the operand associated with this instruction.
    /// </summary>
    /// <remarks>
    /// The operand is a dynamic value which can represent various types of data
    /// used in the execution of a virtual machine instruction. It serves as input
    /// or reference data, which could be a method token, field information,
    /// operand values, etc. Depending on the context and the specific instruction,
    /// this operand provides the necessary information to execute the opcode logic
    /// correctly. The operand is set upon the creation of a BambusInstruction
    /// instance and can be null if no additional data is needed beyond the opcode.
    /// </remarks>
    public dynamic Operand { get; }
}