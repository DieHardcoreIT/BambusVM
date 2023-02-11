using BambusVM.Runtime.Handler;
using BambusVM.Runtime.Handler.Impl;
using BambusVM.Runtime.Handler.Impl.Custom;
using BambusVM.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambusVM.Runtime
{
    public class Context
    {
        public VmArgs Args;
        public Dictionary<BambusOpCodes, BambusOpCode> Handlers;
        public VmLocal Locals;
        public VmStack Stack;

        public Context(List<string> instrs, object[] args)
        {
            Index = 0;
            Instructions = instrs;
            Stack = new VmStack();
            Locals = new VmLocal();
            Args = new VmArgs(args);

            //create instruction set
            Handlers = new Dictionary<BambusOpCodes, BambusOpCode>
            {
                // custom
                { BambusOpCodes.HxCall, new BambusCall() },
                { BambusOpCodes.HxLdc, new BambusLdc() },
                { BambusOpCodes.HxArray, new BambusArray() },
                { BambusOpCodes.HxLoc, new BambusLoc() },
                { BambusOpCodes.HxArg, new BambusArg() },
                { BambusOpCodes.HxFld, new BambusFld() },
                { BambusOpCodes.HxConv, new BambusConv() },

                //not custom
                { BambusOpCodes.Or, new Or() },
                { BambusOpCodes.Null, new Null() },
                { BambusOpCodes.Newarr, new Newarr() },
                { BambusOpCodes.Ldnull, new Ldnull() },
                { BambusOpCodes.Ldloca, new Ldloca() },
                { BambusOpCodes.Ldlen, new Ldlen() },
                { BambusOpCodes.LdelemU1, new LdelemU1() },
                { BambusOpCodes.Ldc, new Ldc() },
                { BambusOpCodes.ConvU1, new ConvU1() },
                { BambusOpCodes.ConvI4, new ConvI4() },
                { BambusOpCodes.Cmp, new Cmp() },
                { BambusOpCodes.Clt, new Clt() },
                { BambusOpCodes.Cgt, new Cgt() },
                { BambusOpCodes.Neg, new Neg() },
                { BambusOpCodes.Not, new Not() },
                { BambusOpCodes.And, new And() },
                { BambusOpCodes.Shr, new Shr() },
                { BambusOpCodes.Shl, new Shl() },
                { BambusOpCodes.Xor, new Xor() },
                { BambusOpCodes.Rem, new Rem() },
                { BambusOpCodes.Ceq, new Ceq() },
                { BambusOpCodes.Mul, new Mul() },
                { BambusOpCodes.Nop, new Nop() },
                { BambusOpCodes.Add, new Add() },
                { BambusOpCodes.Sub, new Sub() },
                { BambusOpCodes.Ret, new Ret() },
                { BambusOpCodes.Pop, new Pop() },
                { BambusOpCodes.Len, new Len() },
                { BambusOpCodes.Dup, new Dup() },
                { BambusOpCodes.Div, new Div() },
                { BambusOpCodes.Ldtoken, new Ldtoken() },
                { BambusOpCodes.Br, new Br() },
                { BambusOpCodes.Brtrue, new Brtrue() },
                { BambusOpCodes.Brfalse, new Brfalse() },
                { BambusOpCodes.Box, new Box() },
                { BambusOpCodes.Newobj, new Newobj() }
            };
        }

        public List<string> Instructions { get; }

        public int Index { get; set; }

        public object Run()
        {
            do
            {
                var instruction = GetInstruction(Index);

                Handlers[instruction.OpCode].Execute(this, instruction);

                Index++;
            } while (Instructions.Count > Index);

            return Stack.Pop();
        }

        private BambusInstruction GetInstruction(int index)
        {
            //convert string to list
            var instructionsData = Instructions[index].Split(',').ToList();

            //the first part is an int which must be converted to an enum
            var opCode = (BambusOpCodes)int.Parse(instructionsData[0]);
            //the second part is the operand, which is a "string" that is converted to a "dynamic".
            var operand = Encoding.UTF8.GetString(Convert.FromBase64String(instructionsData[1]));

            return new BambusInstruction(opCode, operand);
        }
    }
}