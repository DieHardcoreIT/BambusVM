using BambusVM.Runtime.Util;
using System.Reflection;

namespace BambusVM.Runtime.Handler;

public abstract class BambusOpCode
{
    /// <summary>
    /// Executes a given instruction within the specified virtual machine context.
    /// </summary>
    /// <param name="vmContext">The context of the virtual machine, providing access to its current state, stack, and variables.</param>
    /// <param name="instruction">The instruction to be executed, containing the operation code and associated operand.</param>
    public abstract void Execute(Context vmContext, BambusInstruction instruction);

    /// <summary>
    /// Resolves a method using the provided metadata token by searching through the modules of the entry assembly.
    /// </summary>
    /// <param name="mdtoken">The metadata token of the method to be resolved.</param>
    /// <returns>
    /// The <see cref="MethodInfo"/> that matches the given metadata token, or null if no matching method is found.
    /// </returns>
    protected MethodInfo ForceResolveMethod(int mdtoken)
    {
        foreach (var module in Assembly.GetEntryAssembly().Modules)
            try
            {
                var mi = (MethodInfo)module.ResolveMethod(mdtoken);
                return mi;
            }
            catch
            {
                // ignored
            }

        return null;
    }

    /// <summary>
    /// Attempts to resolve and return a <see cref="ConstructorInfo"/> object associated with the specified metadata token.
    /// </summary>
    /// <param name="mdtoken">An integer representing the metadata token for which the method attempts to resolve a constructor.</param>
    /// <returns>A <see cref="ConstructorInfo"/> object if a constructor corresponding to the specified metadata token is found; otherwise, null.
    /// This method iterates through all modules of the entry assembly and tries to resolve the metadata token to a constructor.
    protected ConstructorInfo ForceResolveConstructor(int mdtoken)
    {
        foreach (var module in Assembly.GetEntryAssembly().Modules)
            try
            {
                var mi = (ConstructorInfo)module.ResolveMethod(mdtoken);
                return mi;
            }
            catch
            {
                // ignored
            }

        return null;
    }

    /// <summary>
    /// Attempts to resolve a method or constructor from the provided metadata token within the entry assembly's modules,
    /// returning the corresponding MethodBase instance if successful, or null if not found.
    /// </summary>
    /// <param name="mdtoken">The metadata token corresponding to the method or constructor to be resolved.</param>
    /// <returns>
    /// A MethodBase instance representing the resolved method or constructor, or null if the resolution fails.
    /// </returns>
    protected MethodBase ForceResolveMember(int mdtoken)
    {
        foreach (var module in Assembly.GetEntryAssembly().Modules)
            try
            {
                var mi = (MethodBase)module.ResolveMember(mdtoken);
                return mi;
            }
            catch
            {
                // ignored
            }

        return null;
    }

    /// <summary>
    /// Resolves and returns a FieldInfo object for a given metadata token by searching through the modules
    /// of the entry assembly. If the field cannot be resolved, the method returns null.
    /// </summary>
    /// <param name="mdtoken">The metadata token that identifies the field to be resolved.</param>
    /// <returns>A FieldInfo object representing the resolved field or null if the field cannot be resolved.</returns>
    protected FieldInfo ForceResolveField(int mdtoken)
    {
        foreach (var module in Assembly.GetEntryAssembly().Modules)
            try
            {
                var mi = module.ResolveField(mdtoken);
                return mi;
            }
            catch
            {
                // ignored
            }

        return null;
    }
}