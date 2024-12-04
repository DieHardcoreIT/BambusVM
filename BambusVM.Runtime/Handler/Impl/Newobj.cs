using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl;

public class Newobj : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Resolve the constructor using the operand from the instruction
        var constructor = ForceResolveConstructor(int.Parse(instruction.Operand));

        // Retrieve the parameters required for the constructor invocation
        var methodParameters = Helper.GetMethodParameters(vmContext, constructor.GetParameters());

        // Invoke the constructor with the obtained parameters
        var inst = constructor.Invoke(methodParameters);

        // If the instance is created successfully, push it onto the VM stack
        if (inst != null)
            vmContext.Stack.Push(inst);
    }
}