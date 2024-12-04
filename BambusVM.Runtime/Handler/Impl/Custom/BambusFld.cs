using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusFld : BambusOpCode
{
    /// <summary>
    /// Executes the BambusFld opcode, resolving and retrieving the value of a specified field,
    /// and then pushing this value onto the virtual machine's stack.
    /// </summary>
    /// <param name="vmContext">
    /// The current execution context of the virtual machine, providing access to the stack, locals, and other necessary execution information.
    /// </param>
    /// <param name="instruction">
    /// The current instruction to execute, which includes the operand containing metadata needed to resolve the field.
    /// </param>
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Convert the operand to a string to determine field information
        var operandString = instruction.Operand.ToString();

        // Read the prefix from the operand string to determine if a value should be popped from the stack
        var prefixId = Helper.ReadPrefix(operandString);

        // Parse the metadata token from the operand string, skipping the prefix
        var metaDataToken = int.Parse(operandString.Substring(1));

        // Resolve the field information using the metadata token
        var fieldInfo = ForceResolveField(metaDataToken);

        // Retrieve the value of the field; pop from stack if prefixId is 0, otherwise null
        var value = fieldInfo.GetValue(prefixId == 0 ? vmContext.Stack.Pop() : null);

        // Push the retrieved value onto the stack
        vmContext.Stack.Push(value);
    }
}