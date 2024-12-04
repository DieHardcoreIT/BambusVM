using System.Collections.Generic;
using System.Linq;

namespace BambusVM.Runtime.Util;

public class VmStack
{
    /// <summary>
    /// Represents a dynamic stack used by the virtual machine that can store any type of object.
    /// </summary>
    /// <remarks>
    /// This stack allows for pushing, peeking, and popping of elements, and provides a count of the elements contained within it.
    /// </remarks>
    private Stack<dynamic> Stack { get; }

    /// <summary>
    /// Gets the number of elements contained in the stack.
    /// </summary>
    /// <returns>
    /// An integer representing the number of elements currently in the stack.
    /// </returns>
    public int Count => Stack.Count;

    /// <summary>
    /// Represents a stack data structure used in the Bambus virtual machine.
    /// </summary>
    public VmStack()
    {
        Stack = new Stack<dynamic>();
    }

    /// <summary>
    /// Pushes a dynamic value onto the virtual machine stack.
    /// </summary>
    /// <param name="value">The dynamic value to be pushed onto the stack.</param>
    public void Push(dynamic value)
    {
        Stack.Push(value);
    }

    /// <summary>
    /// Returns the object at the top of the stack without removing it.
    /// </summary>
    /// <returns>
    /// The object at the top of the stack. If the stack is empty, an InvalidOperationException is thrown.
    /// </returns>
    public dynamic Peek()
    {
        return Stack.Peek();
    }

    /// Removes and returns the object at the top of the stack.
    /// <returns>The object removed from the top of the stack.</returns>
    public dynamic Pop()
    {
        return Stack.Pop();
    }
}