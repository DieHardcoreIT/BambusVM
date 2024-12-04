using BambusVM.Runtime.Util;
using System;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusConv : BambusOpCode
{
    /// <summary>
    /// Executes a conversion operation specified by the given instruction
    /// within the provided virtual machine context.
    /// </summary>
    /// <param name="vmContext">The virtual machine context in which the operation is performed.</param>
    /// <param name="instruction">The instruction containing the operation ID for the conversion.</param>
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Parse the instruction operand to determine the conversion operation ID.
        int operationId = int.Parse(instruction.Operand);

        // Pop the top item from the virtual machine stack for conversion.
        var stackItem = vmContext.Stack.Pop();

        // Perform the conversion based on the operation ID.
        PerformConversion(vmContext, operationId, stackItem);
    }

    /// <summary>
    /// Performs a conversion operation on the top item of the virtual machine stack based on the specified operation ID.
    /// </summary>
    /// <param name="vmContext">The current context of the virtual machine, which includes the stack and execution information.</param>
    /// <param name="operationId">The ID representing the type of conversion to be performed (e.g., to float or double).</param>
    /// <param name="stackItem">The item from the top of the stack to be converted.</param>
    /// <exception cref="InvalidOperationException">Thrown if the operation ID is unknown or unsupported.</exception>
    private void PerformConversion(Context vmContext, int operationId, dynamic stackItem)
    {
        switch (operationId)
        {
            case 0: // ID for converting to float
                // Convert the stack item to a Single (float) type and push back to the stack.
                vmContext.Stack.Push(Convert.ToSingle(stackItem));
                break;
            case 1: // ID for converting to double
                // Convert the stack item to a Double type and push back to the stack.
                vmContext.Stack.Push(Convert.ToDouble(stackItem));
                break;
            default:
                // Throw an exception if the operation ID is unknown.
                throw new InvalidOperationException("Unknown operation ID.");
        }
    }
}