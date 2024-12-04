using BambusVM.Runtime.Util;
using System;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusArray : BambusOpCode
{
    /// <summary>
    /// Executes the specified instruction within the given virtual machine context,
    /// determining the type of operation to perform on an array based on the instruction's operand.
    /// </summary>
    /// <param name="vmContext">The context of the virtual machine containing execution state and resources.</param>
    /// <param name="instruction">The instruction to be executed, which includes the operation code and operand.</param>
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Parse the prefix from the instruction's operand to determine the operation type
        int prefix = int.Parse(instruction.Operand);

        // Execute the operation based on the prefix value
        if (prefix == 0)
            ExecuteArrayGet(vmContext);
        else if (prefix == 1) ExecuteArraySet(vmContext);
    }

    /// <summary>
    /// Executes an operation to retrieve an element from an array, using the index specified on the stack,
    /// and pushes the element onto the stack.
    /// </summary>
    /// <param name="vmContext">The current execution context of the Bambus virtual machine,
    /// containing the stack and other execution details.</param>
    private void ExecuteArrayGet(Context vmContext)
    {
        // Pop the index from the stack and parse it as an integer
        var index = PopAndParseInt(vmContext.Stack);

        // Pop the array from the stack
        var array = (Array)vmContext.Stack.Pop();

        // Get the array element at the specified index and push it onto the stack
        vmContext.Stack.Push(array.GetValue(index));
    }

    /// <summary>
    /// Sets a value in an array at a specified index on the virtual machine stack.
    /// </summary>
    /// <param name="vmContext">The context of the virtual machine, including the stack and other execution details.</param>
    private void ExecuteArraySet(Context vmContext)
    {
        // Pop the value to be set from the stack
        object value = vmContext.Stack.Pop();

        // Pop the index from the stack and parse it as an integer
        var index = PopAndParseInt(vmContext.Stack);

        // Pop the array from the stack
        var array = (Array)vmContext.Stack.Pop();

        // Set the array element at the specified index to the given value
        array.SetValue(value, index);
    }


    /// <summary>
    /// Pops an element from the virtual machine stack and parses it as an integer.
    /// </summary>
    /// <param name="stack">The virtual machine stack from which the element is popped.</param>
    /// <returns>The integer value parsed from the popped stack element.</returns>
    private int PopAndParseInt(VmStack stack)
    {
        return int.Parse(stack.Pop().ToString());
    }
}