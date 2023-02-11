using System.Collections.Generic;
using System.Linq;

namespace BambusVM.Runtime.Util
{
    public class VmStack
    {
        public VmStack()
        {
            Stack = new Stack<dynamic>();
        }

        public Stack<dynamic> Stack { get; }

        public int Count => Stack.Count;

        public void Push(dynamic value)
        {
            Stack.Push(value);
        }

        public dynamic Peek()
        {
            return Stack.Peek();
        }

        public dynamic Pop()
        {
            return Stack.Pop();
        }
    }
}