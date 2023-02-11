using BambusVM.Runtime.Util;

namespace BambusVM.Runtime.Handler.Impl.Custom
{
    public class BambusCall : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var str = instruction.Operand.ToString();

            //var type = Helper.ReadPrefix(str);
            var prefix = Helper.ReadPrefix(str, 1);
            var mdToken = int.Parse(str.Substring(2));

            switch (prefix)
            {
                // constructor
                case 0:
                    {
                        var m = ForceResolveConstructor(mdToken);
                        var pm = Helper.GetMethodParameters(vmContext, m.GetParameters());
                        var inst = m.Invoke(pm);

                        if (inst != null)
                            vmContext.Stack.Push(inst);
                        break;
                    }

                //  method
                case 1:
                    {
                        var m = ForceResolveMethod(mdToken);
                        var pm = Helper.GetMethodParameters(vmContext, m.GetParameters());

                        object target = null;
                        if (!m.IsStatic)
                            target = vmContext.Stack.Pop();

                        var ret = m.Invoke(target, pm);

                        if (ret != null)
                            vmContext.Stack.Push(ret);

                        break;
                    }

                // member ref
                case 2:
                    {
                        var m = ForceResolveMember(mdToken);
                        var pm = Helper.GetMethodParameters(vmContext, m.GetParameters());

                        object target = null;
                        if (!m.IsStatic)
                            target = vmContext.Stack.Pop();

                        var ret = m.Invoke(target, pm);

                        if (ret != null)
                            vmContext.Stack.Push(ret);

                        break;
                    }
            }
        }
    }
}