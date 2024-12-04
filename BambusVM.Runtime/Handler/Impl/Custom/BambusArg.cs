using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusArg : BambusOpCode
{
    /// <summary>
    /// Executes the specified instruction within the given virtual machine context.
    /// </summary>
    /// <param name="context">The context of the virtual machine where the execution takes place, containing Stack and Args.</param>
    /// <param name="instruction">The instruction to be executed, containing the operand which identifies the action to perform.</param>
    public override void Execute(Context context, BambusInstruction instruction)
    {
        // Convert the operand to a string to extract prefix and index
        var operandString = instruction.Operand.ToString();

        // Extract the prefix and index from the operand string
        ExtractPrefixAndIndex(operandString, out int prefixValue, out int idx);

        // Determine if the operation is a push operation based on the prefix
        var isPushOperation = prefixValue == 0;

        if (isPushOperation)
        {
            // If it's a push operation, push the argument at index onto the stack
            context.Stack.Push(context.Args.Get(idx));
        }
        else
        {
            // Otherwise, pop the stack and update the argument at index
            var item = context.Stack.Pop();
            context.Args.Update(idx, item);
        }
    }

    /// <summary>
    /// Extracts the prefix and index from a given operand string used in a Bambus VM instruction.
    /// </summary>
    /// <param name="operandString">The string representation of the operand from which the prefix and index are extracted.</param>
    /// <param name="prefix">An output parameter that will hold the extracted integer value of the prefix.</param>
    /// <param name="index">An output parameter that will hold the extracted integer value of the index.</param>
    private void ExtractPrefixAndIndex(string operandString, out int prefix, out int index)
    {
        // Read the prefix value from the string
        prefix = Helper.ReadPrefix(operandString);

        // Parse the index from the remaining part of the string
        index = int.Parse(operandString.Substring(1));
    }
}