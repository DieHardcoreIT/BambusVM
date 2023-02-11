using BambusVM.Runtime.Util;
using System;

namespace BambusVM.Runtime.Handler.Impl.Custom
{
    public class BambusArray : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var prefix = int.Parse(instruction.Operand);

            switch (prefix)
            {
                case 0:
                    {
                        var idx = int.Parse(vmContext.Stack.Pop());
                        var arr = (Array)vmContext.Stack.Pop();

                        vmContext.Stack.Push(arr.GetValue(idx));
                        break;
                    }
                case 1:
                    {
                        var obj = vmContext.Stack.Pop();
                        var idx = int.Parse(vmContext.Stack.Pop());
                        var arr = (Array)vmContext.Stack.Pop();

                        arr.SetValue(obj, idx);
                        break;
                    }
            }
        }
    }
}