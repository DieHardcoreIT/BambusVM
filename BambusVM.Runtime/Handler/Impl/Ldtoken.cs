using BambusVM.Runtime.Util;
using System.Reflection;

namespace BambusVM.Runtime.Handler.Impl;

public class Ldtoken : BambusOpCode
{
    public override void Execute(Context vmContext, BambusInstruction instruction)
    {
        // Convert the operand to a string
        var str = instruction.Operand.ToString();

        // Extract prefix and metadata token from the operand string
        var prefix = Helper.ReadPrefix(str);
        var mdtoken = int.Parse(str.Substring(1));

        // Resolve based on the prefix type
        switch (prefix)
        {
            // Resolve method using metadata token
            case 0:
                vmContext.Stack.Push(typeof(Ldtoken).Module.ResolveMethod(mdtoken));
                break;

            // Resolve member reference and push its method handle
            case 1:
                vmContext.Stack.Push(((MethodBase)typeof(Ldtoken).Module.ResolveMember(mdtoken)).MethodHandle);
                break;

            // Resolve type using metadata token
            case 2:
                vmContext.Stack.Push(typeof(Ldtoken).Module.ResolveType(mdtoken));
                break;

            // Resolve field and push its field handle
            case 3:
                vmContext.Stack.Push(typeof(Ldtoken).Module.ResolveField(mdtoken).FieldHandle);
                break;
        }
    }
}