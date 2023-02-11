using BambusVM.Runtime.Util;
using System.Reflection;

namespace BambusVM.Runtime.Handler
{
    public abstract class BambusOpCode
    {
        public abstract void Execute(Context vmContext, BambusInstruction instruction);

        //probably useless but lets keep it like this
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
}