using BambusVM.Runtime.Util;
using System;

namespace BambusVM.Runtime.Handler.Impl.Custom
{
    public class BambusConv : BambusOpCode
    {
        public override void Execute(Context vmContext, BambusInstruction instruction)
        {
            var id = int.Parse(instruction.Operand);
            var item = vmContext.Stack.Pop();

            // should work
            switch (id)
            {
                case 0: // convert to float   Conv_R4
                    vmContext.Stack.Push(Convert.ToSingle(item));
                    break;

                case 1: // convert to double   Conv_R8
                    vmContext.Stack.Push(Convert.ToDouble(item));
                    break;
            }
        }
    }
}