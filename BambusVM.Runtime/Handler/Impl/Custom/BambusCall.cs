using System.Reflection;
using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom;

public class BambusCall : BambusOpCode
{
    /// <summary>
    /// Executes the BambusCall operation, determining what specific method or constructor
    /// should be invoked based on the prefix and metadata token extracted from the instruction.
    /// </summary>
    /// <param name="vmContext">The virtual machine context that contains the state and other relevant information for execution.</param>
    /// <param name="instruction">The instruction containing the operand that specifies which method, constructor, or member to invoke.</param>
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Convert the operand to a string format and read the prefix
        var operandString = instruction.Operand.ToString();
        var prefix = Helper.ReadPrefix(operandString, 1);
        // Parse the metadata token from the operand string
        var metadataToken = int.Parse(operandString.Substring(2));

        // Determine what operation to perform based on the prefix
        switch (prefix)
        {
            case 0:
            {
                // Resolve the constructor using the metadata token
                var constructor = ForceResolveConstructor(metadataToken);
                // Invoke the method associated with the constructor
                InvokeMethod(vmContext, constructor);
                break;
            }
            case 1:
            case 2:
            {
                // Resolve method or member based on the prefix and metadata token
                var method = prefix == 1 ? ForceResolveMethod(metadataToken) : ForceResolveMember(metadataToken);
                // Invoke the method associated with the resolved method or member
                InvokeMethod(vmContext, method);
                break;
            }
        }
    }

    /// <summary>
    /// Invokes a method using the provided virtual machine context and method base.
    /// Handles passing the parameters to the method and manages the target object for instance methods.
    /// </summary>
    /// <param name="vmContext">The current virtual machine context containing the stack and other execution state.</param>
    /// <param name="method">The method base representing the method to be invoked, which may be static or instance-based.</param>
    private void InvokeMethod(Context vmContext, MethodBase method)
    {
        // Retrieve method parameters based on the method's parameter info
        var parameters = Helper.GetMethodParameters(vmContext, method.GetParameters());
        object target = null;

        // If the method is not static, pop the target object from the stack
        if (!method.IsStatic)
            target = vmContext.Stack.Pop();

        // Invoke the method and push the result back onto the stack if it's not null
        var result = method.Invoke(target, parameters);
        if (result != null)
            vmContext.Stack.Push(result);
    }
}