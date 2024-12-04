using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusLoc : BambusOpCode
{
    /// <summary>
    /// This method converts the operand of the instruction
    /// into a string and performs operations on the local variables of the virtual machine
    /// context based on a prefix value.
    /// </summary>
    /// <param name="vmContext">The current execution context containing the stack and local variables.</param>
    /// <param name="instruction">The instruction being executed, which holds the operand to be processed.</param>
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Convert the instruction's operand to string.
        var str = instruction.Operand.ToString();

        // Read and convert the prefix character to an integer.
        var prefix = Helper.ReadPrefix(str);

        // Parse the remaining characters of the string to get the index.
        var idx = int.Parse(str.Substring(1));

        // Check if the prefix is 0.
        if (prefix == 0)
        {
            // If prefix is 0, push the local variable at 'idx' onto the stack.
            vmContext.Stack.Push(vmContext.Locals.Get(idx));
        }
        else
        {
            // If prefix is not 0, pop an item from the stack and update the local variable at 'idx'.
            var item = vmContext.Stack.Pop();
            vmContext.Locals.Update(idx, item);
        }
    }
}